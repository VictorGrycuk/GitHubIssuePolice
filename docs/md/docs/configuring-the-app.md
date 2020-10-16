# Configuring the App

## Prerequisites

While the app is simple to configure and run, it has the following requirements:

- A [Slack App webhook](https://api.slack.com/messaging/webhooks) [This app uses a webhook, but it obviously can be used with a proper bot](:Footnote)
- A [GitHub Access Token](https://docs.github.com/en/free-pro-team@latest/github/authenticating-to-github/creating-a-personal-access-token) [The token It is not mandatory to use the API, but highly recommended as using the GitHub API without a token has a very low limit of calls](:Footnote)

<br>

## The configuration file

The app uses configuration file with a JSON format:

```json | configuration.json
{
  "GithubConfiguration": {
    "ProductHeader": "",
    "Token": "",
    "User": "",
    "Repository": "",
    "Libraries": [ "" ],
    "ApiOptions": {
      "PageCount": 1,
      "PageSize":  30
    }
  },
  "SlackConfiguration": {
    "Sections": [
      {
        "Kind": 0,
        "LeadingMessage": "",
        "Filters": [ "" ]
      }
    ],
    "Webhook": "",
    "SentBy": ""
  }
}
```

> :Buttons
>
> > :CopyButton

<br>

### GithubConfiguration

As the name suggests, this configuration revolves around the configuration of the GitHub API, but it also contains a configuration seemingly unrelated to it.

| Option        | Description                                                  |
| ------------- | ------------------------------------------------------------ |
| ProductHeader | It identifies the app                                        |
| Token         | A [personal access token](https://docs.github.com/en/free-pro-team@latest/github/authenticating-to-github/creating-a-personal-access-token). It allows a much higher amount of calls |
| User          | The owner of the repository to monitor                       |
| Repository    | The name of the repository to monitor                        |
| ApiOptions    | The pagination handling for the results. More pages or page size results in slower response from the server |
| Libraries     | This is actually not related to the GitHub API, however, it is used to parse the filters. It will be explained later. |

<br>

### SlackConfiguration

The Slack configuration deals with how the message is formatted and sent to Slack.

The message is divided by "sections", each with its own message and information. These sections are formatted using **Slack Blocks**.

The custom Slack API class has support for various types of blocks, by default the leading message is formated as a [Header](https://api.slack.com/reference/block-kit/blocks#header) and the body of the message is an [Overflow](https://api.slack.com/reference/block-kit/block-elements#overflow) block. At this moment it is not possible to configure which blocks are used and how they are formated.

| Option                   | Description                                                  |
| ------------------------ | ------------------------------------------------------------ |
| Sections: Kind           | **0** means *Issue*, **1** means *PR*.                       |
| Sections: LeadingMessage | The title for each section                                   |
| Sections: Filters        | An array of lambda expressions. They are used by [Roslyn](https://github.com/dotnet/roslyn) to filter the issues and pull requests. These are **accumulative**, which means that the result of one filter will be used for the next filter. This might change in the future |
| Webhook                  | The [Slack App webhook](https://api.slack.com/messaging/webhooks) to be used to send the message |
| SentBy                   | Used to identify who sent the message. Has no impact at the moment |

## Example configuration

The following configuration monitors the `some-repo` repository owned by the user `someUser`, then it retrieves the last 30 issues and the last 30 pull requests.

It has 5 sections, 3 for issues , and 2 for pull requests:

1. Section 1
   1. Type: Issue
   2. Filters: From the last 30 issues, it gets those that are open, then from those get those whose name contains `TypeScript` on their name or on their body, then from those get those that were created within the last 14 hours.
2. Section 2
   1. Type: Issue
   2. Filters: Same as Section 1, except it validates the update date instead of creation.
3. Section 3
   1. Type: Issue
   2. Filters: Same as Section 2, except it validates that the status is closed.
4. Section 4
   1. Type: PR
   2. Filters: Same as Section 1
5. Section 5
   1. Type: PR
   2. Filters: Same as Section 4, except it validates that the status is closed and merged.

```json | configuration.json
{
  "GithubConfiguration": {
    "ProductHeader": "GIW",
    "Token": "xxxx",
    "User": "someUser",
    "Repository": "some-repo",
    "Libraries": [ "System.Linq", "System.Collections.Generic", "System", "Octokit" ],
    "ApiOptions": {
      "PageCount": 1,
      "PageSize":  30
    }
  },
  "SlackConfiguration": {
    "Sections": [
      {
        "Kind": 0,
        "LeadingMessage": "The following ISSUES were opened:\n",
        "Filters": [
          "i => i.State == ItemState.Open",
          "i => i.Title.Contains(\"TypeScript\") || i.Body.Contains(\"TypeScript\")",
          "i => i.CreatedAt > DateTime.Now.Subtract(new TimeSpan(14, 0, 0))"
        ]
      },
      {
        "Kind": 0,
        "LeadingMessage": "The following ISSUES were updated :\n",
        "Filters": [
          "i => i.State == ItemState.Open",
          "i => i.Labels.Any(x => new string[] { \"Team: SuperTeam\" }.Contains(x.Name))",
          "i => i.UpdatedAt > DateTime.Now.Subtract(new TimeSpan(14, 0, 0))"
        ]
      },
      {
        "Kind": 0,
        "LeadingMessage": "The following ISSUES were closed:\n",
        "Filters": [
          "i => i.State == ItemState.Closed",
          "i => i.Labels.Any(x => new string[] { \"Team: SuperTeam\" }.Contains(x.Name))",
          "i => i.UpdatedAt > DateTime.Now.Subtract(new TimeSpan(14, 0, 0))"
        ]
      },
      {
        "Kind": 1,
        "LeadingMessage": "The following PR's were opened:\n",
        "Filters": [
          "i => i.State == ItemState.Open",
          "i => i.Title.Contains(\"TypeScript\") || i.Body.Contains(\"TypeScript\")",
          "i => i.CreatedAt > DateTime.Now.Subtract(new TimeSpan(14, 0, 0))"
        ]
      },
      {
        "Kind": 1,
        "LeadingMessage": "The following PR's were merged:\n",
        "Filters": [
          "i => i.State == ItemState.Closed && i.Merged == true",
          "i => i.Title.Contains(\"TypeScript\") || i.Body.Contains(\"TypeScript\")",
          "i => i.UpdatedAt > DateTime.Now.Subtract(new TimeSpan(14, 0, 0))"
        ]
      }
    ],
    "Webhook": "https://hooks.slack.com/services/xxxx/xxxx/xxxx",
    "SentBy": "GIW"
  }
}
```





> :ToCPrevNext

> :Footnotes