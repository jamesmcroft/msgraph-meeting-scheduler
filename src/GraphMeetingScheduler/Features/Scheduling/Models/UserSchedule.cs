namespace GraphMeetingScheduler.Features.Scheduling.Models;

using Microsoft.Graph.Models;
using User = Users.Models.User;

public class UserSchedule
{
    public UserSchedule(
        User user,
        IEnumerable<UserWorkingHours> workingDays,
        IEnumerable<UserScheduleItem> meetings,
        IEnumerable<UserScheduleAvailability> availability)
    {
        this.User = user;
        this.WorkingDays = workingDays;
        this.Meetings = meetings;
        this.Availability = availability;
    }

    public User User { get; set; }

    public IEnumerable<UserWorkingHours> WorkingDays { get; set; }

    public IEnumerable<UserScheduleItem> Meetings { get; set; }

    public IEnumerable<UserScheduleAvailability> Availability { get; set; }
}