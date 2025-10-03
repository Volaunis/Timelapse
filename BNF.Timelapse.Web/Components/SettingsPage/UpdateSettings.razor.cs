using BNF.Timelapse.Models;
using BNF.Timelapse.Models.Extensions;
using BNF.Timelapse.Repositories.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BNF.Timelapse.Web.Components.SettingsPage;

public partial class UpdateSettings
{
    private readonly ISettingsDbRepository _settingsDbRepository;
    private readonly ICameraRepository _cameraRepository;
    private readonly NavigationManager _navigationManager;
    private Settings Settings { get; set; }

    public UpdateSettings(ISettingsDbRepository settingsDbRepository, ICameraRepository cameraRepository, NavigationManager navigationManager)
    {
        _settingsDbRepository = settingsDbRepository;
        _cameraRepository = cameraRepository;
        _navigationManager = navigationManager;

        Settings = _settingsDbRepository.GetSettings();
    }

    private void UpdateSettingsAction(EditContext args)
    {
        var settings = (Settings)args.Model;
        _settingsDbRepository.UpdateSettings(settings);

        var (width, height) = settings.Resolution.GetResolutionValues();
        _cameraRepository.SetCamera(width, height, settings.CameraIndex);

        _navigationManager.NavigateTo("/");
    }
}