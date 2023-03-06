namespace GraphMeetingScheduler.Features.Scheduling.Models;

using Microsoft.Graph.Models;

public class UserWorkingHours : BaseUserScheduleTime
{
    public static UserWorkingHours FromGraphWorkingHours(DateTime actualDay, WorkingHours workingHours)
    {
        TimeSpan workingHoursStartTime = workingHours.StartTime!.Value.DateTime.TimeOfDay;
        TimeSpan workingHoursEndTime = workingHours.EndTime!.Value.DateTime.TimeOfDay;

        DateTime startTime = actualDay.Date.Add(workingHoursStartTime);
        DateTime endTime = actualDay.Date.Add(workingHoursEndTime);

        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(workingHours.TimeZone!.Name!);

        DateTime startTimeUtc = TimeZoneInfo.ConvertTimeToUtc(startTime, timeZoneInfo);
        DateTime endTimeUtc = TimeZoneInfo.ConvertTimeToUtc(endTime, timeZoneInfo);

        return new UserWorkingHours
        {
            StartTime = startTime,
            EndTime = endTime,
            TimeZone = workingHours.TimeZone!.Name!,
            StartTimeUtc = startTimeUtc,
            EndTimeUtc = endTimeUtc
        };
    }
}