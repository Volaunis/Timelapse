using BNF.Timelapse.Models.Enums;

namespace BNF.Timelapse.Models;

public class Settings : MongoModel
{
    public CameraType CameraType { get; set; }
    public Resolution Resolution { get; set; }
    public string Path { get; set; } = string.Empty;
}