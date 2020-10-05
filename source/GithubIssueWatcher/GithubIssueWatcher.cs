using GithubIssueWatcher.Models;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubIssueWatcher
{
    public class GithubIssueWatcher
    {
        private readonly GithubSDK githubSDK;
        private readonly Configuration configuration;

        public GithubIssueWatcher(Configuration configuration)
        {
            this.configuration = configuration;
            githubSDK = new GithubSDK(
                this.configuration.GithubConfiguration.Token,
                this.configuration.GithubConfiguration.User,
                this.configuration.GithubConfiguration.Repository,
                this.configuration.GithubConfiguration.ProductHeader
            );
        }

        public void Run()
        {
            // Issues created
            var issuesCreated = githubSDK.GetIssues().Result;

            var message = GetMessage("The following issues were open:\n", issuesCreated, configuration.GithubConfiguration.FiltersCreated);
            message += GetMessage("\nThe following issues were updated:\n", issuesCreated, configuration.GithubConfiguration.FiltersCreated);

            Send(message);
        }

        private string GetMessage(string leadingMessage, IEnumerable<Issue> issues, IEnumerable<string> filters)
        {
            var message = leadingMessage;

            // Determine how many time span in days we have to add depending when it is run
            var days = DateTime.Now.DayOfWeek == DayOfWeek.Saturday
                ? 1
                : DateTime.Now.DayOfWeek == DayOfWeek.Sunday
                    ? 2
                    : DateTime.Now.DayOfWeek == DayOfWeek.Monday
                        ? 3
                        : 0;

            // Then we filter the number of issues by the configuration lambda filters
            foreach (var filter in filters)
            {
                issues = githubSDK.Filter(issues, filter).Result;
            }

            // Finally we filter by TimeSpan (I need to find to determine if use CreatedAt or UpdatedAt, or better yet, make it work with scripting)
            issues = issues.Where(i => i.CreatedAt > DateTime.Now.Subtract(new TimeSpan(days, configuration.GithubConfiguration.TimeSpan, 0, 0)));

            foreach (var issue in issues)
            {
                message += $"• <{ issue.HtmlUrl }|{ issue.Number }>: { issue.Title }\n";
            }

            return message;
        }

        private void Send(string message)
        {
            var slackClient = new SlackClient(configuration.SlackConfiguration.Webhook);
            slackClient.PostMessage(username: configuration.SlackConfiguration.SentBy,
               text: message,
               channel: configuration.SlackConfiguration.SentBy);
        }
    }
}
