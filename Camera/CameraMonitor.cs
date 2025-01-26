using System.Text;

public class CameraMonitor
{
    private readonly Dictionary<string, CameraInfo> cameraInfo = new();


    public CameraMonitor(List<VirtualCamera> cameras)
    {
        foreach (var camera in cameras)
        {
            cameraInfo[camera.ID] = new CameraInfo()
            {
                RecentTimeStamps = new SortedSet<DateTimeOffset>(),
                PeopleCount = 0,
                Camera = camera
            };
        }
    }
    public CameraInfo? TryGetCameraInfo(string cameraID)
    {
        cameraInfo.TryGetValue(cameraID, out CameraInfo? info);
        return info;
    }

    public Dictionary<string, int> GetPeopleCountByCamera()
    {
        return cameraInfo.ToDictionary(kv => kv.Key, kv => kv.Value.PeopleCount);
    }

    public async Task ResetCameraInfoAsync(string cameraID)
    {
        cameraInfo.TryGetValue(cameraID, out CameraInfo? info);
        if (info is null) return;

        info.PeopleCount = await info.Camera.GetPeopleInSightAsync();
        info.RecentTimeStamps.Clear();
    }

    public void PrintAllCameraDetails()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var info in cameraInfo.Values)
        {
            stringBuilder.Append(info);
        }
        var pos = Console.GetCursorPosition();
        Console.Write($"{stringBuilder.ToString()}");
        Console.SetCursorPosition(pos.Left, pos.Top);
    }
}

