public class FlawedVirtualCamera(CameraMonitoringService server, string ID) : VirtualCamera(server, ID)
{
    protected override async Task SendCameraUpdatesAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(random.Next(500, 1500));
        while (!cancellationToken.IsCancellationRequested)
        {
            var update = GenerateRandomUpdate();
            peopleInSight += update.NetChange;
            if (random.NextDouble() > .1f)
            {
                await _server.AddUpdateAsync(update);
            }
            await Task.Delay(updatePeriod);
        }
    }
}

