using GithubIssueWatcher.Models;
using Newtonsoft.Json;
using System.IO;

namespace Azure_Resource_Police.Helpers
{
    internal static class Serializer
    {
        internal static Configuration Deserialize<T>(string filePath)
        {
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(filePath));
        }

        internal static void Serialize<T>(string filePath, T objectToSerialize)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented));
        }
    }
}
