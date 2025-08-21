using System.Linq.Expressions;
using BNF.Timelapse.Models;
using BNF.Timelapse.Repositories.Services;
using MongoDB.Bson;

namespace BNF.Timelapse.Repositories.Repositories;

public interface ITimelapseDbRepository
{
    void CreateTimelapse(Models.Timelapse timelapse);
    List<Models.Timelapse> GetTimelapses();
    void PauseTimelapse(Models.Timelapse timelapse);
    void ResumeTimelapse(Models.Timelapse timelapse);
    void StopTimelapse(Models.Timelapse timelapse);
    void FinalizeTimelapse(Models.Timelapse timelapse);

    Task<Models.Timelapse?> GetActiveTimelapseAsync();
    Task<Models.Timelapse?> GetBeforeTimelapseAsync();
    Models.Timelapse? GetTimelapseById(ObjectId objectId);
    Models.Timelapse? GetTimelapseByName(string? name);
    Models.Timelapse? GetActiveTimelapse();
    void UpdateIndex(Models.Timelapse activeTimelapse);
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

    public void PauseTimelapse(Models.Timelapse timelapse)
    {
        timelapse.State = TimelapseState.Paused;

        var updateFields = new Dictionary<Expression<Func<Models.Timelapse, object>>, object>
        {
            { x => x.State, TimelapseState.Paused }
        };

        _mongoDbService.Update(timelapse, updateFields);
    }

    public void ResumeTimelapse(Models.Timelapse timelapse)
    {
        timelapse.State = TimelapseState.Running;

        var updateFields = new Dictionary<Expression<Func<Models.Timelapse, object>>, object>
        {
            { x => x.State, TimelapseState.Running }
        };

        _mongoDbService.Update(timelapse, updateFields);
    }

    public void StopTimelapse(Models.Timelapse timelapse)
    {
        timelapse.State = TimelapseState.BeforeVideo;
        timelapse.Stopped = DateTime.Now;

        var updateFields = new Dictionary<Expression<Func<Models.Timelapse, object>>, object>
        {
            { x => x.State, TimelapseState.BeforeVideo },
            { x => x.Stopped, DateTime.Now }
        };

        _mongoDbService.Update(timelapse, updateFields);
    }

    public void FinalizeTimelapse(Models.Timelapse timelapse)
    {
        timelapse.State = TimelapseState.AfterVideo;
        timelapse.Completed = DateTime.Now;

        var updateFields = new Dictionary<Expression<Func<Models.Timelapse, object>>, object>
        {
            { x => x.State, TimelapseState.AfterVideo },
            { x => x.Completed, DateTime.Now }
        };

        _mongoDbService.Update(timelapse, updateFields);
    }

    public async Task<Models.Timelapse?> GetActiveTimelapseAsync()
    {
        return (await _mongoDbService.GetListAsync<Models.Timelapse>(x => x.State == TimelapseState.Running)).SingleOrDefault();
    }

    public async Task<Models.Timelapse?> GetBeforeTimelapseAsync()
    {
        return (await _mongoDbService.GetListAsync<Models.Timelapse>(x => x.State == TimelapseState.BeforeVideo)).SingleOrDefault();
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
        return _mongoDbService.GetList<Models.Timelapse>(x => x.State == TimelapseState.Running).SingleOrDefault();
    }

    public void UpdateIndex(Models.Timelapse activeTimelapse)
    {
        var updateFields = new Dictionary<Expression<Func<Models.Timelapse, object>>, object>
        {
            { x => x.Index, activeTimelapse.Index }
        };

        _mongoDbService.Update(activeTimelapse, updateFields);
    }
}