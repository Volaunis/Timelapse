using BNF.Timelapse.Repositories.Services;

namespace BNF.Timelapse.Repositories.Repositories;

public interface ITimelapseDbRepository
{
    void CreateTimelapse(Models.Timelapse timelapse);
    List<Models.Timelapse> GetTimelapses();
    void StopTimelapse(Models.Timelapse timelapse);

    Task<Models.Timelapse?> GetActiveTimelapseAsync();
    Models.Timelapse? GetTimelapseByName(string? name);
    Models.Timelapse? GetActiveTimelapse();
}

public class TimelapseDbRepository : ITimelapseDbRepository
{
    private readonly IMongoDbService _mongoDbService;

    public TimelapseDbRepository(IMongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    public void CreateTimelapse(Models.Timelapse timelapse)
    {
        _mongoDbService.Insert(timelapse);
    }

    public List<Models.Timelapse> GetTimelapses()
    {
        return _mongoDbService.GetList<Models.Timelapse>(x => true, sort => sort.Created, false);
    }

    public void StopTimelapse(Models.Timelapse timelapse)
    {
        timelapse.Running = false;
        timelapse.Stopped = DateTime.Now;
        _mongoDbService.Replace(timelapse);
    }

    public async Task<Models.Timelapse?> GetActiveTimelapseAsync()
    {
        return (await _mongoDbService.GetListAsync<Models.Timelapse>(x => x.Running)).SingleOrDefault();
    }

    public Models.Timelapse? GetTimelapseByName(string? name)
    {
        return _mongoDbService
            .GetList<Models.Timelapse>(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase))
            .SingleOrDefault();
    }

    public Models.Timelapse? GetActiveTimelapse()
    {
        return _mongoDbService.GetList<Models.Timelapse>(x => x.Running).SingleOrDefault();
    }
}