public class CameraInfo
{
    public SortedSet<DateTimeOffset> RecentTimeStamps { get; init; }
    public int PeopleCount { get; set; }
    public VirtualCamera Camera { get; init; }

    public override string ToString()
    {
        return $"{Camera.ID}:{PeopleCount} ";
    }
}

