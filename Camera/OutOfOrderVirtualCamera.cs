public class OutOfOrderVirtualCamera(CameraMonitoringService server, string ID) : VirtualCamera(server, ID)
{
    private int updateCount;
    protected override async Task SendCameraUpdatesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var update = GenerateRandomUpdate();
            peopleInSight += update.NetChange;
            var futureUpdate = GenerateRandomUpdate();
            peopleInSight += futureUpdate.NetChange;
            futureUpdate.Timestamp += updatePeriod;
            updateCount++;
            if (updateCount % 2 == 0)
            {
                await _server.AddUpdateAsync(update);
                await Task.Delay(random.Next(500, 1500)); // Network Delay
                await _server.AddUpdateAsync(futureUpdate);
            }
            else
            {
                await _server.AddUpdateAsync(futureUpdate);
                await Task.Delay(random.Next(500, 1500)); // Network Delay
                await _server.AddUpdateAsync(update);
            }
            await Task.Delay(updateCount);
        }
    }
}

