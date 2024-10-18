using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ibdal.Models.Abstractions;

public abstract class BaseModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = null!;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}