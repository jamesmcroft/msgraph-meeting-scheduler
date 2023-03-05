namespace GraphMeetingScheduler.Features.Users.Models;

public class User
{
    public string? Id { get; set; }

    public string? Mail { get; set; }

    public static User FromGraphUser(Microsoft.Graph.Models.User user)
    {
        return new User
        {
            Id = user.Id,
            Mail = user.Mail,
        };
    }
}