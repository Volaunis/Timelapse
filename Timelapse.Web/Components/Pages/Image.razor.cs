using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MongoDB.Bson;
using Timelapse.Repositories.Repositories;

namespace Timelapse.Web.Components.Pages;

public partial class Image
{
    private readonly IJSRuntime _js;
    private readonly ISettingsDbRepository _settingsDbRepository;
    private readonly ITimelapseDbRepository _timelapseDbRepository;
    private readonly ICameraRepository _cameraRepository;
    private readonly NavigationManager _navigationManager;

    public Image(IJSRuntime js, ISettingsDbRepository settingsDbRepository, ITimelapseDbRepository timelapseDbRepository, ICameraRepository cameraRepository,
        NavigationManager navigationManager)
    {
        _js = js;
        _settingsDbRepository = settingsDbRepository;
        _timelapseDbRepository = timelapseDbRepository;
        _cameraRepository = cameraRepository;
        _navigationManager = navigationManager;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender == false) return;

        Stream? imageStream;
        if (Id != null)
        {
            var timelapse = _timelapseDbRepository.GetTimelapseById(new ObjectId(Id));

            if (timelapse == null) return;

            var settings = _settingsDbRepository.GetSettings();

            var baseDirectory = Path.Join(settings.Path, timelapse.BaseName);

            var filename = Path.Join(baseDirectory, $"{timelapse.Index:000000}.jpg");

            imageStream = File.OpenRead(filename);
        }
        else
        {
            imageStream = _cameraRepository.GetImage();
        }

        if (imageStream == null)
            return;

        var strRef = new DotNetStreamReference(imageStream);

        await _js.InvokeVoidAsync("setSource", "image", strRef, "image/png", "Snapshot");
    }

    private void ReloadPage()
    {
        _navigationManager.Refresh(forceReload: true);
    }

    [Parameter]
    public string? Id { get; set; }
}