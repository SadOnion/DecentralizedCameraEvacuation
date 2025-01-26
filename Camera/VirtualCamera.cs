public class VirtualCamera
{
    protected readonly CameraMonitoringService _server;
    protected int peopleInSight;
    public string ID { get; init; }
    protected Random random;
    protected TimeSpan updatePeriod = TimeSpan.FromSeconds(5);
    public VirtualCamera(CameraMonitoringService server, string ID)
    {
        random = new Random(ID.GetHashCode());
        _server = server;
        this.ID = ID;
        Task.Run(async () => await SendCameraUpdatesAsync(CancellationToken.None));
    }

    protected virtual async Task SendCameraUpdatesAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(random.Next(500, 1500));
        while (!cancellationToken.IsCancellationRequested)
        {
            var update = new CameraUpdate
            {
                CameraId = ID,
                Timestamp = DateTimeOffset.UtcNow,
                In = random.Next(0, 5)
            };
            peopleInSight += update.In;
            update.Out = random.Next(0, peopleInSight);
            peopleInSight -= update.Out;
            await _server.AddUpdateAsync(update);
            await Task.Delay(updatePeriod);
        }
    }
    public CameraUpdate GenerateRandomUpdate()
    {
        return new CameraUpdate
        {
            CameraId = ID,
            Timestamp = DateTimeOffset.UtcNow,
            In = random.Next(0, 5),
            Out = random.Next(0, peopleInSight)
        };
    }

    public async Task<int> GetPeopleInSightAsync()
    {
        await Task.Delay(random.Next(750, 1500)); // Simulate Network Delay
        return peopleInSight;
    }
}

