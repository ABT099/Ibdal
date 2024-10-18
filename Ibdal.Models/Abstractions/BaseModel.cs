using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ibdal.Models.Abstractions;

public abstract class BaseModel
{
    [BsonId]
    public ObjectId Id { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}