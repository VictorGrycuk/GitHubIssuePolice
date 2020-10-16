# GitHubSDK

## Constructor

It has two constructors, the first one is actually deprecated as it creates a GitHub client without a token.

```csharp | GithubSDK.cs
private readonly GitHubClient client;

public GithubSDK(string productHeader = "GitHubIssueWatcher")
{
    client = new GitHubClient(new ProductHeaderValue(productHeader));
}
```

The second constructor takes a `GithubConfiguration` class and uses it to initialize a `GitHubClient` class.

```csharp | GithubSDK.cs
private readonly GitHubClient client;
private readonly GithubConfiguration configuration;

public GithubSDK(GithubConfiguration configuration) // @see [Helpers](/docs/helpers)
{
    this.configuration = configuration;

    client = new GitHubClient(new ProductHeaderValue(configuration.ProductHeader))
    {
        Credentials = new Credentials(configuration.Token)
    };
}
```

## Methods

### GetIssues

The `GetIssues()` simply get all the issues from a repository.

By default, the API only seems to return the last ~100 opened issues, therefore the `State = ItemStateFilter.All`. However, this returns **all** the issues from the repository, and it might take quite a long time if the repository has over 1000 issues.

This is solved by specifying the amount of results to return in the form of `configuration.ApiOptions`.

```csharp | GithubSDK.cs
private readonly GithubConfiguration configuration;

internal async Task<IEnumerable<Issue>> GetIssues()
{
    return await client.Issue.GetAllForRepository(
        configuration.User,
        configuration.Repository,
        new RepositoryIssueRequest { State = ItemStateFilter.All },
        configuration.ApiOptions);
}
```

<br>

### GetPullRequests

The `GetPullRequests()` simply get all the PR's from a repository.

Very similar to `GetIssues()` in terms of configuration.

Sadly, `Issue` and `PullRequest` don't share a superclass even though they share many properties. This would have made a lot of the work easier.

```csharp | GithubSDK.cs
private readonly GithubConfiguration configuration;

internal async Task<IEnumerable<PullRequest>> GetPullRequests()
{
    return await client.PullRequest.GetAllForRepository(
        configuration.User,
        configuration.Repository,
        new PullRequestRequest { State = ItemStateFilter.All },
        configuration.ApiOptions);
}
```

<br>

### Filter

The `Filter()` method is used to filter a list of issues of PR's using roslyn scripting. See [Helpers](/docs/helpers).

It takes a list of generics and a lambda expresion, then `RoslynScripting` class is used to evaluate the lambda expression. After roslyn it is done evaluating it, it is used to filter the list of generics.

```csharp | GithubSDK.cs
private readonly GithubConfiguration configuration;

internal List<T> Filter<T>(List<T> issues, string lambda)
{
    var filterExpression = RoslynScripting.Evaluate<T>(
        new System.Reflection.Assembly[] { typeof(T).Assembly },
        configuration.Libraries,
        lambda);
    return issues.Where(discountFilterExpression).ToList();
}
```

Since roslyn evaluates the code in an isolated spaced, it requires its own assemblies and usings directives.

The assemblies can be passed dynamically by simply passing the same generic into the evaluation, however, since it is impossible to determine which using directives will be needed by each filter, they have to be specified in the configuration.

> :ToCPrevNext