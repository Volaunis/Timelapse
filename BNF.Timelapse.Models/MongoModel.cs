using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BNF.Timelapse.Models;

public class MongoModel
{
    [BsonId]
    public ObjectId Id { get; set; }
}