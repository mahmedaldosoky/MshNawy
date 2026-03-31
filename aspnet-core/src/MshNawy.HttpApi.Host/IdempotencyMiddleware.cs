using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MshNawy.Domain.Shared;
using MshNawy.EntityFrameworkCore;

namespace MshNawy.HttpApi.Host;

/// <summary>
/// Idempotency middleware for financial endpoints.
/// Per Constitution III: All financial operations must be atomic and idempotent with UUID keys.
/// Reads X-Idempotency-Key header, returns cached response on duplicate.
/// </summary>
public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly string[] FinancialPathPrefixes =
    {
        "/api/app/wallet/",
        "/api/app/deposits/",
        "/api/app/withdrawals/",
        "/api/app/orders/"
    };

    public IdempotencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!IsFinancialMutatingRequest(context.Request))
        {
            await _next(context);
            return;
        }

        var keyHeader = context.Request.Headers["X-Idempotency-Key"].ToString();
        if (string.IsNullOrWhiteSpace(keyHeader) || !Guid.TryParse(keyHeader, out var idempotencyKey))
        {
            await _next(context);
            return;
        }

        var dbContext = context.RequestServices.GetRequiredService<MshNawyDbContext>();

        var existing = await dbContext.IdempotencyRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.IdempotencyKey == idempotencyKey);

        if (existing != null)
        {
            if (existing.ExpiresAt > DateTime.UtcNow)
            {
                context.Response.StatusCode = existing.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(existing.ResponseBody, Encoding.UTF8);
                return;
            }

            // Expired — remove and allow re-processing
            dbContext.IdempotencyRecords.Remove(existing);
            await dbContext.SaveChangesAsync();
        }

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        responseBody.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(responseBody).ReadToEndAsync();

        var record = new IdempotencyRecord(
            Guid.NewGuid(),
            idempotencyKey,
            responseText,
            context.Response.StatusCode,
            DateTime.UtcNow
        );

        dbContext.IdempotencyRecords.Add(record);
        await dbContext.SaveChangesAsync();

        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
        context.Response.Body = originalBodyStream;
    }

    private static bool IsFinancialMutatingRequest(HttpRequest request)
    {
        var method = request.Method;
        if (method != HttpMethods.Post && method != HttpMethods.Put)
        {
            return false;
        }

        var path = request.Path.Value ?? string.Empty;
        foreach (var prefix in FinancialPathPrefixes)
        {
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
