namespace GraphMeetingScheduler.Features.Scheduling.Models;

public class UserScheduleAvailability : BaseUserScheduleTime
{
    public static UserScheduleAvailability FromUserWorkingHours(UserWorkingHours workingHours, int offset, int duration)
    {
        return new UserScheduleAvailability
        {
            StartTimeUtc = workingHours.StartTimeUtc.AddMinutes(offset * duration),
            EndTimeUtc = workingHours.StartTimeUtc.AddMinutes((offset + 1) * duration)
        };
    }
}