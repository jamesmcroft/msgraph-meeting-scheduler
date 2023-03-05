namespace GraphMeetingScheduler.Features.Scheduling.Models;

using Microsoft.Graph.Models;

public class UserScheduleItem
{
    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? TimeZone { get; set; }

    public DateTime StartTimeUtc { get; set; }

    public DateTime EndTimeUtc { get; set; }

    public static UserScheduleItem FromGraphScheduleItem(ScheduleItem scheduleItem)
    {
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(scheduleItem.Start!.TimeZone!);

        var startTime = DateTime.Parse(scheduleItem.Start.DateTime!);
        var endTime = DateTime.Parse(scheduleItem.End!.DateTime!);

        DateTime startTimeUtc = TimeZoneInfo.ConvertTimeToUtc(startTime, timeZoneInfo);
        DateTime endTimeUtc = TimeZoneInfo.ConvertTimeToUtc(endTime, timeZoneInfo);

        return new UserScheduleItem
        {
            StartTime = startTime,
            EndTime = endTime,
            TimeZone = scheduleItem.Start.TimeZone,
            StartTimeUtc = startTimeUtc,
            EndTimeUtc = endTimeUtc
        };
    }
}