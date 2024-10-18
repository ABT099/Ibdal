using MongoDB.Bson.Serialization.Attributes;

namespace Ibdal.Models.Abstractions;

public abstract class BaseModel<TKey>
{
    [BsonId]
    public TKey Id { get; init; } = default!;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}