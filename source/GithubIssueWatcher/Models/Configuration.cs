using Octokit;
using System.Collections.Generic;

namespace GithubIssueWatcher.Models
{
    public class Configuration
    {
        public GithubConfiguration GithubConfiguration = new GithubConfiguration();
        public SlackConfiguration SlackConfiguration = new SlackConfiguration();
    }

    public class GithubConfiguration
    {
        public string ProductHeader { get; set; }
        public string Token { get; set; }
        public string User { get; set; }
        public string Repository { get; set; }
        public string[] Libraries { get; set; }
        public ApiOptions ApiOptions { get; set; }
    }

    public class Section
    {
        public Kind Kind { get; set; }
        public string LeadingMessage { get; set; }
        public string[] Filters { get; set; }
    }

    public class SlackConfiguration
    {
        public List<Section> Sections = new List<Section>();
        public string Webhook { get; set; }
        public string SentBy { get; set; }
    }

    public enum Kind
    {
        Issue,
        PullRequest
    }
}
