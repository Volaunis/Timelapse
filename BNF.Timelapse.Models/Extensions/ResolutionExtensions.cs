using BNF.Timelapse.Models.Enums;

namespace BNF.Timelapse.Models.Extensions;

public static class ResolutionExtensions
{
    public static string GetResolutionDisplay(this Resolution resolution)
    {
        return resolution switch
        {
            Resolution.None => "None",
            Resolution.Res720P => "720p",
            Resolution.Res1080P => "1080p",
            Resolution.Res4K => "4K",
            _ => throw new NotImplementedException()
        };
    }

    public static (int width, int height) GetResolutionValues(this Resolution resolution)
    {
        return resolution switch
        {
            Resolution.None => (0, 0),
            Resolution.Res720P => (1280, 720),
            Resolution.Res1080P => (1920, 1080),
            Resolution.Res4K => (3840, 2160),
            _ => throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null)
        };
    }
}