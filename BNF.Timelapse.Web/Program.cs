using BNF.Timelapse.BackendService;
using BNF.Timelapse.Repositories.Configuration;
using BNF.Timelapse.Repositories.Repositories;
using BNF.Timelapse.Repositories.Services;
using BNF.Timelapse.Web.Components;

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

services.Configure<MongoDbConfiguration>(builder.Configuration.GetSection("mongoDbConfiguration"));


var app = builder.Build();

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