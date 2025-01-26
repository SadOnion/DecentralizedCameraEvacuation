using Microsoft.Extensions.Options;
using System.Threading.Channels;

public class CameraMonitoringService(IOptions<AppConfig> config) : BackgroundService
{
    public int TotalPeople { get; private set; } = 0;

    private readonly Channel<CameraUpdate> updateChannel = Channel.CreateUnbounded<CameraUpdate>();
    public CameraMonitor? CameraMonitor { get; private set; }
    private static readonly TimeSpan MissingDataInterval = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan MissingDataThreshold = TimeSpan.FromSeconds(10);

    DateTime lastPrintTime = DateTime.Now;
    private void InitializeVirtualCameras()
    {
        List<VirtualCamera> cameras = new();
        cameras.AddRange(Enumerable.Range(1, config.Value.HealthyCameraCount)
            .Select(i => new VirtualCamera(this, $"Healthy{i}")));
        cameras.AddRange(Enumerable.Range(1, config.Value.FlawedCameraCount)
            .Select(i => new FlawedVirtualCamera(this, $"Flawed{i}")));
        cameras.AddRange(Enumerable.Range(1, config.Value.OutOfOrderCameraCount)
            .Select(i => new OutOfOrderVirtualCamera(this, $"OutOfOrder{i}")));
        CameraMonitor = new CameraMonitor(cameras);
    }

    public void PrintCamerasInfo()
    {
        if (DateTime.Now.Subtract(lastPrintTime).Seconds < 1) return;
        lastPrintTime = DateTime.Now;
        CameraMonitor.PrintAllCameraDetails();
    }

    public async Task AddUpdateAsync(CameraUpdate update)
    {
        await updateChannel.Writer.WriteAsync(update);
    }

    private async Task ProcessCameraUpdatesAsync()
    {
        await foreach (var update in updateChannel.Reader.ReadAllAsync())
        {
            var cameraInfo = CameraMonitor.TryGetCameraInfo(update.CameraId);

            if (!cameraInfo.RecentTimeStamps.Contains(update.Timestamp))
            {
                cameraInfo.PeopleCount += update.NetChange;
                TotalPeople += update.NetChange;
                cameraInfo.RecentTimeStamps.Add(update.Timestamp);
                await CheckForMissingCameraDataAsync(update);
                RemoveOldestTimestamp(cameraInfo);
            }
            PrintCamerasInfo();
        }
    }

    private async Task CheckForMissingCameraDataAsync(CameraUpdate update)
    {
        CameraInfo info = CameraMonitor.TryGetCameraInfo(update.CameraId);
        DateTimeOffset? timeGap = FindTimeGapGreaterThan(MissingDataInterval, info.RecentTimeStamps);

        if (timeGap.HasValue && DateTimeOffset.UtcNow.Subtract(timeGap.Value) > MissingDataThreshold)
        {
            await CameraMonitor.ResetCameraInfoAsync(update.CameraId);
        }
    }

    private DateTimeOffset? FindTimeGapGreaterThan(TimeSpan interval, SortedSet<DateTimeOffset> set)
    {
        DateTimeOffset? previousTimestamp = null;
        foreach (var timeStamp in set)
        {
            if (previousTimestamp.HasValue)
            {
                var gap = timeStamp - previousTimestamp.Value;
                if (gap.Seconds > interval.Seconds)
                {
                    return previousTimestamp.Value + interval;
                }

            }
            previousTimestamp = timeStamp;
        }
        return null;
    }

    private void RemoveOldestTimestamp(CameraInfo info)
    {
        if (info.RecentTimeStamps.Count > 10)
        {
            info.RecentTimeStamps.Remove(info.RecentTimeStamps.Min);
        }
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        InitializeVirtualCameras();
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ProcessCameraUpdatesAsync();
    }
}

