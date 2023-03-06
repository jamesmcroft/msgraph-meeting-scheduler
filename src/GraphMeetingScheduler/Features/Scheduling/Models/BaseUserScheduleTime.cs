namespace GraphMeetingScheduler.Features.Scheduling.Models;

public abstract class BaseUserScheduleTime
{
    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? TimeZone { get; set; }

    public DateTime StartTimeUtc { get; set; }

    public DateTime EndTimeUtc { get; set; }
}