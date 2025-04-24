using BNF.Timelapse.Repositories.Repositories;

namespace BNF.Timelapse.Web.Components.Pages;

public partial class Index
{
    private readonly ITimelapseDbRepository _timelapseDbRepository;

    public Index(ITimelapseDbRepository timelapseDbRepository)
    {
        _timelapseDbRepository = timelapseDbRepository;
        _timelapses = _timelapseDbRepository.GetTimelapses().AsQueryable();
    }

    private IQueryable<Models.Timelapse>? _timelapses;

    private void StopTimelapse(Models.Timelapse timelapse)
    {
        _timelapseDbRepository.StopTimelapse(timelapse);
    }
}