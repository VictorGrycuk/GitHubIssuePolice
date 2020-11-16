using AutoMapper;
using AutoMapper.Configuration;
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
        private readonly IMapper mapper;

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

            var cfg = new MapperConfigurationExpression();
            cfg.CreateMap<Issue, Resource>();
            cfg.CreateMap<PullRequest, Resource>();

            var mapperConfig = new MapperConfiguration(cfg);
            mapper = new Mapper(mapperConfig);
        }

        internal async Task<Repository> GetRepository()
        {
            return await client.Repository.Get(configuration.User, configuration.Repository);
        }

        internal async Task<User> GetUser()
        {
            return await client.User.Get(configuration.User);
        }

        internal List<Resource> GetIssues()
        {
            var issuesList = client.Issue.GetAllForRepository(
                configuration.User,
                configuration.Repository,
                new RepositoryIssueRequest { State = ItemStateFilter.All },
                configuration.ApiOptions).Result;

            return MapToResource(issuesList.Where(i => i.PullRequest == null), ResourceType.ISSUE);
        }
        
        internal List<Resource> GetPullRequests()
        {
            var prList = client.PullRequest.GetAllForRepository(
                configuration.User,
                configuration.Repository,
                new PullRequestRequest { State = ItemStateFilter.All },
                configuration.ApiOptions).Result;

            return MapToResource(prList, ResourceType.PR);
        }

        internal async Task<IEnumerable<Branch>> GetBranches()
        {
            return await client.Repository.Branch.GetAll(
                configuration.User,
                configuration.Repository,
                configuration.ApiOptions);
        }

        internal async Task<GitHubCommit> GetCommits(string sha)
        {
            return await client.Repository.Commit.Get(
            configuration.User,
            configuration.Repository,
            sha);
        }

        private List<Resource> MapToResource<T>(T unmappedResource, ResourceType resourceType)
        {
            var resourceList = mapper.Map<List<Resource>>(unmappedResource);
            resourceList.ForEach(r => 
            {
                r.ResourceType = resourceType;
                r.Events = client.Issue.Timeline.GetAllForIssue(configuration.User, configuration.Repository, r.Number).Result.ToList();
            });

            return resourceList;
        }

        internal List<T> Filter<T>(List<T> issues, string lambda)
        {
            var filterExpression = RoslynScripting.Evaluate<T>(
                new System.Reflection.Assembly[] { typeof(T).Assembly },
                configuration.Libraries,
                lambda);
            return issues.Where(filterExpression).ToList();
        }
    }
}
