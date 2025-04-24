using BNF.Timelapse.Repositories.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BNF.Timelapse.Web.Components.Pages;

public partial class Create
{
    private readonly ITimelapseDbRepository _timelapseDbRepository;
    private readonly NavigationManager _navigationManager;

    private bool _duplicateName;

    public Create(ITimelapseDbRepository timelapseDbRepository, NavigationManager navigationManager)
    {
        _timelapseDbRepository = timelapseDbRepository;
        _navigationManager = navigationManager;
    }

    private void AddTimelapse(EditContext args)
    {
        var timelapse = (Models.Timelapse)args.Model;

        var existingTimelapse = _timelapseDbRepository.GetTimelapseByName(timelapse.Name);

        if (existingTimelapse != null)
        {
            _duplicateName = true;
            return;
        }

        _timelapseDbRepository.CreateTimelapse(timelapse);

        _navigationManager.NavigateTo("/");
    }

    [SupplyParameterFromForm]
    private Models.Timelapse Timelapse { get; set; } = new()
    {
        Name = $"LAN {DateTime.Now:yyyy-MM-dd}"
    };
}