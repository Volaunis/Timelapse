using BNF.Timelapse.Repositories.Repositories;
using Microsoft.Extensions.Hosting;

namespace BNF.Timelapse.BackendService;

public class RunTimelapse : BackgroundService
{
    private readonly ITimelapseDbRepository _timelapseDbRepository;
    private readonly Timer _timer;
    private bool _timerActive;
    private Models.Timelapse? _activeTimelapse;


    public RunTimelapse(ITimelapseDbRepository timelapseDbRepository)
    {
        _timelapseDbRepository = timelapseDbRepository;
        _timer = new Timer(DoWork, null, Timeout.Infinite, Timeout.Infinite);
    }

    private void DoWork(object? state)
    {
        var x = _activeTimelapse;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested == false)
        {
            var activeTimelapse = await _timelapseDbRepository.GetActiveTimelapseAsync();

            if (activeTimelapse == null && _timerActive)
                StopTimer();

            if (activeTimelapse != null && _timerActive == false)
            {
                if (_activeTimelapse?.Id == activeTimelapse.Id)
                    _activeTimelapse = activeTimelapse;

                if (_timerActive == false)
                    StartTimer();
            }

            await Task.Delay(1000, stoppingToken);
        }

        StopTimer();
    }

    private void StartTimer()
    {
        _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(10));
        _timerActive = true;
    }

    private void StopTimer()
    {
        _timer.Change(Timeout.InfiniteTimeSpan, TimeSpan.FromSeconds(0));
        _timerActive = false;
    }
}