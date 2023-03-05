namespace GraphMeetingScheduler.Features.Scheduling.Models;

using Microsoft.Graph.Models;

public record UserSchedule(User User, IEnumerable<ScheduleInformation> Schedule);