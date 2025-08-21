using BNF.Timelapse.Repositories.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MongoDB.Bson;

namespace BNF.Timelapse.Web.Components.Pages;

public partial class Image
{
    private readonly IJSRuntime _js;
    private readonly ISettingsDbRepository _settingsDbRepository;
    private readonly ITimelapseDbRepository _timelapseDbRepository;

    public Image(IJSRuntime js, ISettingsDbRepository settingsDbRepository, ITimelapseDbRepository timelapseDbRepository)
    {
        _js = js;
        _settingsDbRepository = settingsDbRepository;
        _timelapseDbRepository = timelapseDbRepository;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender == false) return;

        var timelapse = _timelapseDbRepository.GetTimelapseById(new ObjectId(Id));

        if (timelapse == null) return;

        var settings = _settingsDbRepository.GetSettings();

        var baseDirectory = Path.Join(settings.Path, timelapse.BaseName);

        var filename = Path.Join(baseDirectory, $"{timelapse.Index:000000}.jpg");

        var imageStream = File.OpenRead(filename);

        var strRef = new DotNetStreamReference(imageStream);

        await _js.InvokeVoidAsync("setSource", "image", strRef, "image/png", "Snapshot");
    }

    [Parameter]
    public string? Id { get; set; }
}