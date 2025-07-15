using System.Linq.Expressions;
using BNF.Timelapse.Repositories.Services;
using MongoDB.Bson;

namespace BNF.Timelapse.Repositories.Repositories;

public interface ITimelapseDbRepository
{
    void CreateTimelapse(Models.Timelapse timelapse);
    List<Models.Timelapse> GetTimelapses();
    void StopTimelapse(Models.Timelapse timelapse);

    Task<Models.Timelapse?> GetActiveTimelapseAsync();
    Models.Timelapse? GetTimelapseById(ObjectId objectId);
    Models.Timelapse? GetTimelapseByName(string? name);
    Models.Timelapse? GetActiveTimelapse();
    void IncreaseIndex(Models.Timelapse activeTimelapse);
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

    public Models.Timelapse? GetTimelapseById(ObjectId objectId)
    {
        return _mongoDbService
            .GetList<Models.Timelapse>(x => x.Id == objectId)
            .SingleOrDefault();
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

    public void IncreaseIndex(Models.Timelapse activeTimelapse)
    {
        activeTimelapse.Index++;

        var updateFields = new Dictionary<Expression<Func<Models.Timelapse, object>>, object>
        {
            { x => x.Index, activeTimelapse.Index }
        };

        _mongoDbService.Update(activeTimelapse, updateFields);
    }
}