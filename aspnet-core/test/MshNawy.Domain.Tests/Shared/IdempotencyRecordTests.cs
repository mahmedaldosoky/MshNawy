using System;
using MshNawy.Domain.Shared;
using Xunit;

namespace MshNawy.Domain.Tests.Shared;

public class IdempotencyRecordTests
{
    [Fact]
    public void Constructor_SetsExpiresAt_To24HoursAfterCreation()
    {
        var createdAt = new DateTime(2026, 3, 26, 10, 0, 0, DateTimeKind.Utc);
        var record = new IdempotencyRecord(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "{\"result\":\"ok\"}",
            200,
            createdAt
        );

        Assert.Equal(createdAt.AddHours(24), record.ExpiresAt);
    }

    [Fact]
    public void Record_NotExpired_WhenWithin24Hours()
    {
        var createdAt = DateTime.UtcNow;
        var record = new IdempotencyRecord(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "{\"result\":\"ok\"}",
            200,
            createdAt
        );

        Assert.True(record.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public void Record_Expired_WhenPast24Hours()
    {
        var createdAt = DateTime.UtcNow.AddHours(-25);
        var record = new IdempotencyRecord(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "{\"result\":\"ok\"}",
            200,
            createdAt
        );

        Assert.True(record.ExpiresAt < DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_StoresAllProperties()
    {
        var id = Guid.NewGuid();
        var key = Guid.NewGuid();
        var body = "{\"accessToken\":\"jwt\"}";
        var status = 201;
        var createdAt = DateTime.UtcNow;

        var record = new IdempotencyRecord(id, key, body, status, createdAt);

        Assert.Equal(id, record.Id);
        Assert.Equal(key, record.IdempotencyKey);
        Assert.Equal(body, record.ResponseBody);
        Assert.Equal(status, record.StatusCode);
        Assert.Equal(createdAt, record.CreatedAt);
    }
}
