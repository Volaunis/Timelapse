using System.Diagnostics;

namespace BNF.Timelapse.Repositories.Repositories;

public interface ISnapshotRepository
{
    bool TakeSnapshot(Models.Timelapse? timelapse);
    void CreateVideo(Models.Timelapse timelapse);
    (Stream stream, string filename) GetVideo(Models.Timelapse timelapse);
}

public class SnapshotRepository : ISnapshotRepository
{
    private readonly ISettingsDbRepository _settingsDbRepository;
    private readonly ICameraRepository _cameraRepository;

    public SnapshotRepository(ISettingsDbRepository settingsDbRepository, ICameraRepository cameraRepository)
    {
        _settingsDbRepository = settingsDbRepository;
        _cameraRepository = cameraRepository;
    }

    public bool TakeSnapshot(Models.Timelapse? timelapse)
    {
        var settings = _settingsDbRepository.GetSettings();

        if (timelapse == null)
            return false;

        var baseDirectory = Path.Join(settings.Path, timelapse.BaseName);

        if (Directory.Exists(baseDirectory) == false)
            Directory.CreateDirectory(baseDirectory);


        var filename = Path.Join(baseDirectory, $"{timelapse.Index:000000}.jpg");

        if (File.Exists(filename))
        {
            Console.WriteLine($"Filename {filename} already exists, skipping");
            return true; // We need to advance the index, since the file already exists
        }

        Console.WriteLine($"Saving {filename}");

        _cameraRepository.SaveImage(filename);

        var fileExists = File.Exists(filename);

        if (fileExists == false)
            Console.WriteLine("Image was not saved");

        return fileExists;

        //var process = new Process();
        //process.StartInfo.FileName = "fswebcam";
        //process.StartInfo.Arguments = $"--delay 1 --resolution {settings.Resolution.GetResolutionString()} --no-banner --frames 1 --jpeg 50 --device /dev/video0 {filename}";
        //process.StartInfo.RedirectStandardError = true;
        //process.Start();
        //process.WaitForExit();

        //process.StartInfo.Arguments = $"-n {settings.CameraType.GetCameraString()} -s {settings.Resolution.GetResolutionString()} -v:f 1 -frames:v 10 {filename}";


        // fswebcam --delay 1 --resolution 1920x1080 --no-banner --frames 1 --jpeg 50 --device /dev/video0 test6.jpg

        // ffmpeg -f v4l2 -i /dev/video0 -frames:v 10 -update 1 test.jpg

        // ffmpeg -f dshow -i "video=Logitech BRIO" -s 3840x2160 -frames:v 1 -update 1 test11.jpg
        // ffmpeg -i timelapses/$1/%06d.jpeg -vcodec mpeg4 timelapses/$1.mp4
        // ffmpeg -i %06d.jpg -vcodec mpeg4 test.mp4

        // ffmpeg -i /var/timelapse.app/LAN_2025-07-15_4/%6d.jpg -c:v libx264 video.mp4
    }

    public void CreateVideo(Models.Timelapse timelapse)
    {
        var settings = _settingsDbRepository.GetSettings();
        var baseDirectory = Path.Join(settings.Path, timelapse.BaseName);

        var filenameMask = Path.Join(baseDirectory, "%6d.jpg");
        var videoFilename = Path.Join(baseDirectory, $"{timelapse.BaseName}.mp4");

        if (File.Exists(videoFilename))
        {
            Console.WriteLine($"File {videoFilename} already exists, deleting");
            File.Delete(videoFilename);
        }

        Console.WriteLine($"Rendering {videoFilename}");

        var process = new Process();
        process.StartInfo.FileName = "ffmpeg";
        process.StartInfo.Arguments = $"-i {filenameMask} -c:v libx264 {videoFilename}";
        //process.StartInfo.RedirectStandardError = true;
        process.Start();
        process.WaitForExit();

        Console.WriteLine($"Finalized render");
    }

    public (Stream stream, string filename) GetVideo(Models.Timelapse timelapse)
    {
        var settings = _settingsDbRepository.GetSettings();
        var baseDirectory = Path.Join(settings.Path, timelapse.BaseName);

        var baseFilename = $"{timelapse.BaseName}.mp4";

        var videoFilename = Path.Join(baseDirectory, baseFilename);

        return (File.OpenRead(videoFilename), baseFilename);
    }
}