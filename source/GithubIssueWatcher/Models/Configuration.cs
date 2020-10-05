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
        public string[] FiltersCreated { get; set; }
        public string[] FiltersModified { get; set; }
        public int TimeSpan { get; set; }
    }

    public class SlackConfiguration
    {
        public string Webhook { get; set; }
        public string MessageBody { get; set; }
        public string SentBy { get; set; }
    }
}
