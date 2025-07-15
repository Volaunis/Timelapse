using System.Text.Json;
using BNF.Timelapse.Repositories.Configuration;
using BNF.Timelapse.Repositories.Repositories;
using BNF.Timelapse.Repositories.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var services = builder.Services;

services.AddSingleton<IMongoDbService, MongoDbService>();
services.AddSingleton<ITimelapseDbRepository, TimelapseDbRepository>();
services.AddSingleton<ISettingsDbRepository, SettingsDbRepository>();

services.Configure<MongoDbConfiguration>(builder.Configuration.GetSection("mongoDbConfiguration"));


using var host = builder.Build();


var settingsDbRepository = host.Services.GetRequiredService<ISettingsDbRepository>();


var setting = settingsDbRepository.GetSettings();

Console.WriteLine(JsonSerializer.Serialize(setting, new JsonSerializerOptions { WriteIndented = true }));