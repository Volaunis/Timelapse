using BNF.Timelapse.Models;
using BNF.Timelapse.Repositories.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BNF.Timelapse.Web.Components.SettingsPage;

public partial class UpdateSettings
{
    private readonly ISettingsDbRepository _settingsDbRepository;
    private readonly NavigationManager _navigationManager;
    private Settings Settings { get; set; }

    public UpdateSettings(ISettingsDbRepository settingsDbRepository, NavigationManager navigationManager)
    {
        _settingsDbRepository = settingsDbRepository;
        _navigationManager = navigationManager;

        Settings = _settingsDbRepository.GetSettings();
    }

    private void UpdateSettingsAction(EditContext args)
    {
        var settings = (Settings)args.Model;
        _settingsDbRepository.UpdateSettings(settings);
        _navigationManager.NavigateTo("/");
    }
}