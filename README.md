# Microsoft Graph Meeting Scheduler

[![Hackathon][badge_hackathon]][link_hackathon]
[![GitHub Actions][badge_actions]][link_actions] 
[![GitHub Issues][badge_issues]][link_issues]
[![GitHub Stars][badge_repo_stars]][link_repo]
[![Repo Language][badge_language]][link_repo]
[![Repo License][badge_license]][link_repo]
[![GitHub Sponsor][badge_sponsor]][link_sponsor]

This is an experimentation project to understand the capabilities of the Microsoft Graph APIs including user mailboxes, calendars, and how Teams meetings could be scheduled.

The project contains an Azure Functions API that can check calendars of multiple users within a Microsoft 365 tenant based on a period of time, and find availability for the best time for a meeting. The request takes into consideration each participant's schedules, as well as including each individual's time zone to ensure that the times available are most convenient for everyone.

## Using the Scheduler API

You will need to create a new Azure AD application and grant it the following application permissions for the Microsoft Graph:

- Calendars.Read
- Mail.Read
- MailboxSettings.Read
- OnlineMeetings.ReadWrite.All
- User.Read.All

Once created, you will need a client secret for the application. This can be generated from the Azure Portal.

Update the `local.settings.json` file with the following values:

- `TenantId`: The tenant ID of the Azure AD tenant
- `ClientId`: The client ID of the Azure AD application
- `ClientSecret`: The client secret of the Azure AD application

You can then run the project locally using the Azure Functions Core Tools.

### Get Availability

To get availability for a meeting, you can make a `GET` request to the `/api/scheduling/availability` endpoint with the following query string parameters:

- `start`: The UTC start date and time to get availability from in ISO 8601 format.
- `end`: The UTC end date and time to get availability to in ISO 8601 format.
- `email`: An email address per participant to include in the availability check.
- `duration`: The expected duration for a meeting in minutes.

For example, to get availability between 1st March 2023 9am until 15th March 2023 5pm, you could make the following request:

```http
/api/scheduling/availability?start=2023-03-01T09:00:00Z&end=2023-03-15T17:00:00Z&email=james@croft.co.uk&email=tom@croft.co.uk&duration=60
```

The response will be a JSON object containing the availability for each participant.

```json
[
  {
    "startTimeUtc": "2023-03-01T16:00:00Z",
    "endTimeUtc": "2023-03-01T17:00:00Z"
  }
]
```

## Supporting this project

As many developers know, projects like this are built in spare time! If you find this project useful, please **Star** the repo.

## Author

👤 James Croft

[![Website][badge_blog]][link_blog]
[![Twitter][badge_twitter]][link_twitter]
[![LinkedIn][badge_linkedin]][link_linkedin]

[badge_hackathon]: https://img.shields.io/badge/Microsoft%20-Hack--Together-orange?style=for-the-badge&logo=microsoft
[link_hackathon]: https://github.com/microsoft/hack-together
[badge_blog]: https://img.shields.io/badge/blog-jamesmcroft.co.uk-blue?style=for-the-badge
[badge_linkedin]: https://img.shields.io/badge/LinkedIn-jmcroft-blue?style=for-the-badge&logo=linkedin
[badge_twitter]: https://img.shields.io/badge/follow-%40jamesmcroft-1DA1F2?logo=twitter&style=for-the-badge&logoColor=white
[link_blog]: https://www.jamescroft.co.uk/
[link_linkedin]: https://www.linkedin.com/in/jmcroft
[link_twitter]: https://twitter.com/jamesmcroft
[badge_language]: https://img.shields.io/badge/language-C%23-blue?style=for-the-badge
[badge_license]: https://img.shields.io/github/license/jamesmcroft/msgraph-meeting-scheduler?style=for-the-badge
[badge_issues]: https://img.shields.io/github/issues/jamesmcroft/msgraph-meeting-scheduler?style=for-the-badge
[badge_repo_stars]: https://img.shields.io/github/stars/jamesmcroft/msgraph-meeting-scheduler?logo=github&style=for-the-badge
[badge_sponsor]: https://img.shields.io/github/sponsors/jamesmcroft?logo=github&style=for-the-badge
[link_issues]: https://github.com/jamesmcroft/msgraph-meeting-scheduler/issues
[link_repo]: https://github.com/jamesmcroft/msgraph-meeting-scheduler
[link_sponsor]: https://github.com/sponsors/jamesmcroft
[badge_actions]: https://img.shields.io/github/actions/workflow/status/jamesmcroft/msgraph-meeting-scheduler/ci.yml?style=for-the-badge
[link_actions]: https://github.com/jamesmcroft/msgraph-meeting-scheduler
