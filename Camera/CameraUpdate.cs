public class CameraUpdate
{
    public string CameraId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public int In { get; set; }
    public int Out { get; set; }

    public int NetChange => In - Out;
}

