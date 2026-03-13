public enum EventStatus
{
    NotDone,
    InProgress,
    Done
}

public class Wydarzenie
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime? PlannedDate { get; set; } 
    public TimeSpan? StartTime { get; set; }  
    public TimeSpan? EndTime { get; set; }    
    public EventStatus Status { get; set; }
}