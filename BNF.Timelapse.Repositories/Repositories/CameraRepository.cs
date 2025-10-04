using BNF.Timelapse.Models.Extensions;
using Microsoft.Extensions.Logging;
using OpenCvSharp;

namespace BNF.Timelapse.Repositories.Repositories;

public interface ICameraRepository
{
    void SetCamera(int width, int height, int index);
    void SaveImage(string filename);
    Stream? GetImage();
}

public class CameraRepository : ICameraRepository
{
    private readonly ILogger<CameraRepository> _logger;
    private VideoCapture? _videoCapture;

    public CameraRepository(ISettingsDbRepository settingsDbRepository, ILogger<CameraRepository> logger)
    {
        _logger = logger;
        var settings = settingsDbRepository.GetSettings();
        var (width, height) = settings.Resolution.GetResolutionValues();

        SetCamera(width, height, settings.CameraIndex);
    }

    public void SetCamera(int width, int height, int index)
    {
        try
        {
            if (_videoCapture != null)
            {
                _videoCapture.Dispose();
                _videoCapture = null;
            }

            _videoCapture = VideoCapture.FromCamera(index);

            _videoCapture.Set(VideoCaptureProperties.FrameWidth, width);
            _videoCapture.Set(VideoCaptureProperties.FrameHeight, height);

            _videoCapture.Grab();

            _videoCapture.RetrieveMat();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed starting webcam");
            _videoCapture = null;
        }
    }

    public void SaveImage(string filename)
    {
        if (_videoCapture == null)
        {
            _logger.LogWarning($"Attempted to save an image without an active camera");
            return;
        }

        var image = _videoCapture.RetrieveMat();
        image.SaveImage(filename);
    }

    public Stream? GetImage()
    {
        if (_videoCapture == null)
            return null;

        var image = _videoCapture.RetrieveMat();
        return image.ToMemoryStream();
    }
}