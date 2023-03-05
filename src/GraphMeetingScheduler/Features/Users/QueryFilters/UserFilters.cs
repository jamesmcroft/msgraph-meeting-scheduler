namespace GraphMeetingScheduler.Features.Users.QueryFilters;

public static class UserFilters
{
    public static string ByEmailAddress(string emailAddress)
    {
        return $"mail eq '{emailAddress}'";
    }
}