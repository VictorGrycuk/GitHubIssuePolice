using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GithubIssueWatcher
{
    internal class GithubSDK
    {
        private readonly GitHubClient client;
        private readonly string User;
        private readonly string Repository;

        public GithubSDK(string token, string user, string repository, string productHeader = "GitHubIssueWatcher")
        {
            client = new GitHubClient(new ProductHeaderValue(productHeader));
            client.Credentials = new Credentials(token);
            User = user;
            Repository = repository;
        }

        public GithubSDK(string productHeader = "GitHubIssueWatcher")
        {
            client = new GitHubClient(new ProductHeaderValue(productHeader));
        }

        internal async Task<Repository> GetRepository()
        {
            return await client.Repository.Get(User, Repository);
        }

        internal async Task<User> GetUser()
        {
            return await client.User.Get(User);
        }

        internal async Task<IEnumerable<Issue>> GetIssues()
        {
            return await client.Issue.GetAllForRepository(User, Repository);
        }

        internal async Task<IEnumerable<T>> Filter<T>(IEnumerable<T> issues, string lambda)
        {
            var options = ScriptOptions.Default.AddReferences(typeof(T).Assembly).WithImports(new string[] { "System.Linq", "System.Collections.Generic" });
            var discountFilterExpression = await CSharpScript.EvaluateAsync<Func<T, bool>>(lambda, options);
            return issues.Where(discountFilterExpression);
        }
    }
}
