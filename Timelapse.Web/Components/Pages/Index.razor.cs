using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Timelapse.Repositories.Repositories;

namespace Timelapse.Web.Components.Pages;

public partial class Index
{
    private readonly IJSRuntime _js;
    private readonly ITimelapseDbRepository _timelapseDbRepository;
    private readonly ISnapshotRepository _snapshotRepository;
    private readonly NavigationManager _navigationManager;

    public Index(IJSRuntime js, ITimelapseDbRepository timelapseDbRepository, ISnapshotRepository snapshotRepository, NavigationManager navigationManager)
    {
        _js = js;
        _timelapseDbRepository = timelapseDbRepository;
        _snapshotRepository = snapshotRepository;
        _navigationManager = navigationManager;
        _timelapses = _timelapseDbRepository.GetTimelapses().AsQueryable();
    }

    private IQueryable<Models.Timelapse>? _timelapses;

    private void PauseTimelapse(Models.Timelapse timelapse)
    {
        _timelapseDbRepository.PauseTimelapse(timelapse);
    }

    private void ResumeTimelapse(Models.Timelapse timelapse)
    {
        _timelapseDbRepository.ResumeTimelapse(timelapse);
    }

    private void StopTimelapse(Models.Timelapse timelapse)
    {
        _timelapseDbRepository.StopTimelapse(timelapse);
    }

    private (Stream stream, string filename) GetFileStream(Models.Timelapse timelapse)
    {
        return _snapshotRepository.GetVideo(timelapse);
    }

    private async Task DownloadFileFromStream(Models.Timelapse timelapse)
    {
        var (stream, filename) = GetFileStream(timelapse);


        using var streamRef = new DotNetStreamReference(stream: stream);

        await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
    }

    private void ReloadPage()
    {
        _navigationManager.Refresh(forceReload: true);
    }
}