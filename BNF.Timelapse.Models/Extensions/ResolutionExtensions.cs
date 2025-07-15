using BNF.Timelapse.Models.Enums;

namespace BNF.Timelapse.Models.Extensions;

public static class ResolutionExtensions
{
    public static string GetResolutionString(this Resolution resolution)
    {
        return resolution switch
        {
            Resolution.Res720P => "1280x720",
            Resolution.Res1080P => "1920x1080",
            Resolution.Res4K => "3840x2160",
            _ => throw new NotImplementedException()
        };
    }

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
}