public enum EventStatus
{
    NotDone,
    InProgress,
    Done
}

public enum EventPriority
{
    Low,
    Medium,
    High,
    Critical
}

public class Wydarzenie
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime? PlannedDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public EventStatus Status { get; set; }
    public EventPriority Priority { get; set; } = EventPriority.Medium;
    public string? Category { get; set; }
    public string? Description { get; set; }
}