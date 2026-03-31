using System;
using Volo.Abp.Domain.Entities;

namespace MshNawy.Domain.Shared;

/// <summary>
/// Stores idempotency key records for financial endpoints.
/// Per Constitution III: All financial operations must be idempotent with UUID keys.
/// </summary>
public class IdempotencyRecord : Entity<Guid>
{
    public Guid IdempotencyKey { get; private set; }
    public string ResponseBody { get; private set; } = string.Empty;
    public int StatusCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    protected IdempotencyRecord() { }

    public IdempotencyRecord(Guid id, Guid idempotencyKey, string responseBody, int statusCode, DateTime createdAt)
        : base(id)
    {
        IdempotencyKey = idempotencyKey;
        ResponseBody = responseBody;
        StatusCode = statusCode;
        CreatedAt = createdAt;
        ExpiresAt = createdAt.AddHours(24);
    }
}
