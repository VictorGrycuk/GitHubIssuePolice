using GithubIssueWatcher.Helpers;
using GithubIssueWatcher.Models;
using Octokit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GithubIssueWatcher
{
    internal class GithubSDK
    {
        private readonly GitHubClient client;
        private readonly GithubConfiguration configuration;

        public GithubSDK(string productHeader = "GitHubIssueWatcher")
        {
            client = new GitHubClient(new ProductHeaderValue(productHeader));
        }
        
        public GithubSDK(GithubConfiguration configuration)
        {
            this.configuration = configuration;

            client = new GitHubClient(new ProductHeaderValue(configuration.ProductHeader))
            {
                Credentials = new Credentials(configuration.Token)
            };
        }

        internal async Task<Repository> GetRepository()
        {
            return await client.Repository.Get(configuration.User, configuration.Repository);
        }

        internal async Task<User> GetUser()
        {
            return await client.User.Get(configuration.User);
        }

        internal async Task<IEnumerable<Issue>> GetIssues()
        {
            return await client.Issue.GetAllForRepository(
                configuration.User,
                configuration.Repository,
                new RepositoryIssueRequest { State = ItemStateFilter.All },
                configuration.ApiOptions);
        }
        
        internal async Task<IEnumerable<PullRequest>> GetPullRequests()
        {
            return await client.PullRequest.GetAllForRepository(
                configuration.User,
                configuration.Repository,
                new PullRequestRequest { State = ItemStateFilter.All },
                configuration.ApiOptions);
        }

        internal List<T> Filter<T>(List<T> issues, string lambda)
        {
            var filterExpression = RoslynScripting.Evaluate<T>(
                new System.Reflection.Assembly[] { typeof(T).Assembly },
                configuration.Libraries,
                lambda);
            return issues.Where(discountFilterExpression).ToList();
        }
    }
}
