using BNF.Timelapse.Models;
using BNF.Timelapse.Repositories.Services;

namespace BNF.Timelapse.Repositories.Repositories;

public interface ISettingsDbRepository
{
    Settings GetSettings();
    void UpdateSettings(Settings settings);
}

public class SettingsDbRepository : ISettingsDbRepository
{
    private readonly IMongoDbService _mongoDbService;

    public SettingsDbRepository(IMongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    public Settings GetSettings()
    {
        var settings = _mongoDbService.Get<Settings>(x => true);

        if (settings != null) return settings;

        settings = new Settings();
        _mongoDbService.Insert(settings);

        return settings;
    }

    public void UpdateSettings(Settings settings)
    {
        _mongoDbService.Replace(settings);
    }
}