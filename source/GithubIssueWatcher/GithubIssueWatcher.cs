using Azure_Resource_Police.Helpers;
using GithubIssueWatcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GithubIssueWatcher
{
    public class GithubIssueWatcher
    {
        private readonly GithubSDK githubSDK;
        private readonly Configuration configuration;
        private readonly Arguments options;

        public GithubIssueWatcher(Arguments options)
        {
            this.options = options;
            configuration = LoadConfiguration();
            githubSDK = new GithubSDK(configuration.GithubConfiguration);
        }

        public void Run()
        {
            var resources = githubSDK.GetIssues();
            resources.AddRange(githubSDK.GetPullRequests());
            var blocks = new List<Block>();

            foreach (var section in configuration.SlackConfiguration.Sections)
            {
                blocks.AddRange(CreateSectionBlocks(resources, section));
            }

            if (blocks.Count > 0)
            {
                Send("New Github daily report", blocks);
            }
        }

        private List<Block> CreateSectionBlocks(List<Resource> sectionResources, Section section)
        {
            var blocks = new List<Block>();
            var resources = GetFilteredResources(sectionResources, section.Filters).Where(r => r.ResourceType == section.Kind).ToList();
            if (resources.Count == 0) return blocks;

            foreach (var resource in resources)
            {
                var overflow = new Overflow($"*{ section.LeadingMessage } <{ resource.HtmlUrl }|#{ resource.Number }>:*  { resource.Title }");

                overflow.Accessory.Options.Add(new Option() { Text = new PlainText("Created by: " + resource.User.Login) });
                overflow.Accessory.Options.Add(new Option() { Text = new PlainText("Created at: " + resource.CreatedAt) });

                if (resource.Assignees.Count() > 0)
                {
                    overflow.Accessory.Options.Add(new Option() { Text = new PlainText("Assignee: " + resource.Assignees[0].Login ) });
                }

                if (resource.ClosedBy != null)
                {
                    overflow.Accessory.Options.Add(new Option() { Text = new PlainText("Closed by: " + resource.ClosedBy) });
                }

                if (resource.Events.Count > 0)
                {
                    overflow.Accessory.Options.Add(new Option() { Text = new PlainText("Latest Event: " + resource.Events[resource.Events.Count - 1].Event.StringValue) });
                }

                blocks.Add(overflow);
            }

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

        private Configuration LoadConfiguration()
        {
            if (string.IsNullOrEmpty(options.ConfigurationPath)) throw new ArgumentNullException("Configuration file not found");

            var configuration = Serializer.Deserialize<Configuration>(options.ConfigurationPath);

            return string.IsNullOrWhiteSpace(options.Password) ? configuration : DecryptConfiguration(configuration, options.Password);
        }

        private static Configuration DecryptConfiguration(Configuration configuration, string password)
        {
            configuration.GithubConfiguration.Token = Helpers.AESEncryption.Decrypt(configuration.GithubConfiguration.Token, password);
            configuration.SlackConfiguration.Webhook = Helpers.AESEncryption.Decrypt(configuration.SlackConfiguration.Webhook, password);

            return configuration;
        }

        private static Configuration EncryptConfiguration(Configuration configuration, string password)
        {
            configuration.GithubConfiguration.Token = Helpers.AESEncryption.Encrypt(configuration.GithubConfiguration.Token, password);
            configuration.SlackConfiguration.Webhook = Helpers.AESEncryption.Encrypt(configuration.SlackConfiguration.Webhook, password);

            return configuration;
        }

        internal static void FileEncryption(string filepath, string password, EncryptionAction encryptionAction)
        {
            var configuration = Serializer.Deserialize<Configuration>(filepath);
            configuration = encryptionAction == EncryptionAction.encrypt
                ? EncryptConfiguration(configuration, password)
                : DecryptConfiguration(configuration, password);

            WriteConfiguration(configuration, filepath);
        }

        internal static void WriteConfiguration(Configuration configuration, string filepath)
        {
            Serializer.Serialize(filepath, configuration);
        }
    }

    public enum EncryptionAction
    {
        encrypt,
        decrypt
    }
}
