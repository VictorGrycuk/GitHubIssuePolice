using GithubIssueWatcher.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GithubIssueWatcher
{
    public class GithubIssueWatcher
    {
        private readonly GithubSDK githubSDK;
        private readonly Configuration configuration;

        public GithubIssueWatcher(Configuration configuration)
        {
            this.configuration = configuration;
            githubSDK = new GithubSDK(configuration.GithubConfiguration);
        }

        public void Run()
        {
            var issues = configuration.SlackConfiguration.Sections.Any(i => i.Kind == Kind.Issue) ? githubSDK.GetIssues().Result : null;
            var pullRequests = configuration.SlackConfiguration.Sections.Any(i => i.Kind == Kind.PullRequest) ? githubSDK.GetPullRequests().Result : null;
            var message = string.Empty;
            
            foreach (var section in configuration.SlackConfiguration.Sections)
            {
                if (section.Kind == Kind.Issue && issues != null)
                { 
                    message += GenerateSectionMessage(section, issues);
                };
                
                if (section.Kind == Kind.PullRequest && pullRequests != null)
                {
                    message += GenerateSectionMessage(section, pullRequests);
                };

                message += Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                Send(message);
            }
        }

        private string GenerateSectionMessage<T>(Section section, IEnumerable<T> resources)
        {
            var message = section.LeadingMessage;

            foreach (var filter in section.Filters)
            {
                resources = githubSDK.Filter(resources, filter);
            }

            foreach (var resource in resources)
            {
                var genericResource = JObject.FromObject(resource);
                message += $"• <{ genericResource["HtmlUrl"] }|{ genericResource["Number"] }>: { genericResource["Title"] }\n";
            }

            return resources.Count() > 0 ? message : string.Empty;
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
