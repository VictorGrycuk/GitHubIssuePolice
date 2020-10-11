using GithubIssueWatcher.Models;
using Newtonsoft.Json.Linq;
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
            var blocks = new List<Block>();

            foreach (var section in configuration.SlackConfiguration.Sections)
            {
                if (section.Kind == Kind.Issue && issues != null)
                {
                    blocks.AddRange(CreateSectionBlocks(issues, section, "Issue"));
                };

                if (section.Kind == Kind.PullRequest && pullRequests != null)
                {
                    blocks.AddRange(CreateSectionBlocks(pullRequests, section, "PR"));
                };
            }

            if (blocks.Count > 0)
            {
                Send("New Github daily report", blocks);
            }
        }

        private List<Block> CreateSectionBlocks<T>(IEnumerable<T> sectionResources, Section section, string resourceName)
        {
            var blocks = new List<Block>();
            var resources = GetFilteredResources(sectionResources, section.Filters);
            if (resources.Count == 0) return blocks;

            blocks.Add(new Title(section.LeadingMessage));
            foreach (var resource in resources)
            {
                var genericResource = JObject.FromObject(resource);
                blocks.Add(new LinkButton($"*{ genericResource["Title"] }*", "See " + resourceName, genericResource["HtmlUrl"].ToString()));
                blocks.Add(new Fields(new List<BaseText>
                {
                    new MarkdownText($"*{ resourceName } Number:*\n#" + genericResource["Number"]),
                    new MarkdownText("*Open by:*\n" + genericResource["User"]["Login"]),
                    new MarkdownText("*Created at:*\n" + genericResource["CreatedAt"]),
                    new MarkdownText("*Closed at:*\n" + genericResource["ClosedAt"]),
                }));
                blocks.Add(new Divider());
            }

            blocks.Add(new Divider());

            return blocks;
        }

        private List<T> GetFilteredResources<T>(IEnumerable<T> sectionResources, string[] filters)
        {
            var resources = sectionResources.ToList();
            foreach (var filter in filters)
            {
                resources = githubSDK.Filter(resources, filter);
            }

            return resources;
        }

        private void Send(string message, List<Block> blocks = null)
        {
            var slackClient = new SlackClient(configuration.SlackConfiguration.Webhook);
            slackClient.PostMessage(
                text: message,
                username: configuration.SlackConfiguration.SentBy,
                channel: configuration.SlackConfiguration.SentBy,
                blocks);
        }
    }
}
