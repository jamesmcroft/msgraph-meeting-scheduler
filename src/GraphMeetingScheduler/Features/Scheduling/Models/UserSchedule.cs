namespace GraphMeetingScheduler.Features.Scheduling.Models;

using Microsoft.Graph.Models;
using User = Users.Models.User;

public class UserSchedule
{
    public User User { get; set; }

    public IEnumerable<UserWorkingHours> WorkingHours { get; set; }

    public IEnumerable<UserScheduleItem> ScheduledItems { get; set; }

    public static UserSchedule FromGraphSchedule(
        User user,
        DateTime startDateTime,
        DateTime endDateTime,
        IEnumerable<ScheduleInformation> schedule)
    {
        IEnumerable<ScheduleItem> scheduleItems = schedule.SelectMany(
            scheduleInformation => scheduleInformation.ScheduleItems!);

        WorkingHours scheduleWorkingHours = schedule.FirstOrDefault().WorkingHours!;

        IEnumerable<DateTime> daysOfWeek = Enumerable.Range(0, 1 + endDateTime.Subtract(startDateTime).Days)
            .Select(offset => startDateTime.AddDays(offset));

        IEnumerable<UserWorkingHours> userWorkingHours = daysOfWeek
            .Where(rangeDate =>
                scheduleWorkingHours.DaysOfWeek!.Any(scheduleDay => (int)scheduleDay! == (int)rangeDate.DayOfWeek))
            .Select(workingDay => UserWorkingHours.FromGraphWorkingHours(workingDay, scheduleWorkingHours));

        return new UserSchedule { User = user, WorkingHours = userWorkingHours, ScheduledItems = scheduleItems.Select(UserScheduleItem.FromGraphScheduleItem) };
    }
}