using GithubIssueWatcher.Models;
using Newtonsoft.Json;
using System.IO;

namespace Azure_Resource_Police.Helpers
{
    internal static class Serializer
    {
        internal static Configuration DeserializeConfiguration(string filePath)
        {
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(filePath));
        }
    }
}
