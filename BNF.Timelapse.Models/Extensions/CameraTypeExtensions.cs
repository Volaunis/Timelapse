using BNF.Timelapse.Models.Enums;

namespace BNF.Timelapse.Models.Extensions;

public static class CameraTypeExtensions
{
    public static string GetCameraString(this CameraType cameraType)
    {
        return cameraType switch
        {
            CameraType.LinuxVideo0 => "-f v4l2 -i /dev/video0",
            CameraType.WindowsLogitechBrio => "-f dshow -i \"video=Logitech BRIO\"",
            _ => throw new NotImplementedException()
        };
    }

    public static string GetCameraDisplay(this CameraType cameraType)
    {
        return cameraType switch
        {
            CameraType.None => "None",
            CameraType.LinuxVideo0 => "Video 0 on Linux",
            CameraType.WindowsLogitechBrio => "Logitech BRIO on windows",
            _ => throw new ArgumentOutOfRangeException(nameof(cameraType), cameraType, null)
        };
    }
}