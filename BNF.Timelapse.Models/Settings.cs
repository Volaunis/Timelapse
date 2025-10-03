using BNF.Timelapse.Models.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace BNF.Timelapse.Models;

[BsonIgnoreExtraElements]
public class Settings : MongoModel
{
    public Resolution Resolution { get; set; }
    public int CameraIndex { get; set; }
    public string Path { get; set; } = string.Empty;
}