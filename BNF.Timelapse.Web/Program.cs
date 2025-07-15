using BNF.Timelapse.BackendService;
using BNF.Timelapse.Repositories.Configuration;
using BNF.Timelapse.Repositories.Repositories;
using BNF.Timelapse.Repositories.Services;
using BNF.Timelapse.Web.Components;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var services = builder.Services;

services.AddHostedService<RunTimelapse>();

services.AddSingleton<IMongoDbService, MongoDbService>();
services.AddSingleton<ITimelapseDbRepository, TimelapseDbRepository>();
services.AddSingleton<ISettingsDbRepository, SettingsDbRepository>();
services.AddSingleton<ISnapshotRepository, SnapshotRepository>();

services.Configure<MongoDbConfiguration>(builder.Configuration.GetSection("mongoDbConfiguration"));


var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();