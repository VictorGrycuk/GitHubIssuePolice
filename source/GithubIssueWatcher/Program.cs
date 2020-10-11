using Azure_Resource_Police.Helpers;
using System;
using System.IO;

namespace GithubIssueWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var configPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Configuration.json");
            if (string.IsNullOrEmpty(configPath)) throw new ArgumentNullException("Configuration file not found");
            var config = Serializer.DeserializeConfiguration(configPath);
            var watcher = new GithubIssueWatcher(config);
            watcher.Run();
        }
    }
}
