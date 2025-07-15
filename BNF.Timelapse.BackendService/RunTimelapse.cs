using BNF.Timelapse.Repositories.Repositories;
using Microsoft.Extensions.Hosting;

namespace BNF.Timelapse.BackendService;

public class RunTimelapse : BackgroundService
{
    private readonly ITimelapseDbRepository _timelapseDbRepository;
    private readonly ISnapshotRepository _snapshotRepository;
    private readonly Timer _timer;
    private bool _timerActive;
    private Models.Timelapse? _activeTimelapse;


    public RunTimelapse(ITimelapseDbRepository timelapseDbRepository, ISnapshotRepository snapshotRepository)
    {
        _timelapseDbRepository = timelapseDbRepository;
        _snapshotRepository = snapshotRepository;
        _timer = new Timer(DoWork, null, Timeout.Infinite, Timeout.Infinite);
    }

    private void DoWork(object? state)
    {
        var activeTimelapse = _activeTimelapse;

        if (activeTimelapse == null)
            return;


        _timelapseDbRepository.IncreaseIndex(activeTimelapse);
        _snapshotRepository.TakeSnapshot(_activeTimelapse);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (stoppingToken.IsCancellationRequested == false)
            {
                var activeTimelapse = await _timelapseDbRepository.GetActiveTimelapseAsync();

                if (activeTimelapse == null && _timerActive)
                {
                    StopTimer();
                    _activeTimelapse = null;
                }

                if (activeTimelapse != null && _timerActive == false)
                {
                    if (_activeTimelapse?.Id != activeTimelapse.Id)
                        _activeTimelapse = activeTimelapse;

                    if (_timerActive == false)
                        StartTimer();
                }

                await Task.Delay(1000, stoppingToken);
            }

            StopTimer();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
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