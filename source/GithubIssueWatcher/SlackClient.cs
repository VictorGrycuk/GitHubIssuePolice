using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace GithubIssueWatcher
{
    //A simple C# class to post messages to a Slack channel
    //Note: This class uses the Newtonsoft Json.NET serializer available via NuGet
    public class SlackClient
    {
        private readonly Uri _uri;
        private readonly Encoding _encoding = new UTF8Encoding();

        public SlackClient(string urlWithAccessToken)
        {
            _uri = new Uri(urlWithAccessToken);
        }

        //Post a message using simple strings
        public void PostMessage(string text, string username = null, string channel = null, List<Block> blocks = null)
        {
            Payload payload = new Payload()
            {
                Channel = channel,
                Username = username,
                Text = text,
                Blocks = blocks
            };

            PostMessage(payload);
        }

        //Post a message using a Payload object
        public void PostMessage(Payload payload)
        {
            string payloadJson = JsonConvert.SerializeObject(payload, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            using (WebClient client = new WebClient())
            {
                var data = new NameValueCollection();
                data["payload"] = payloadJson;

                var response = client.UploadValues(_uri, "POST", data);

                //The response text is usually "ok"
                string responseText = _encoding.GetString(response);
            }
        }
    }

    //This class serializes into the Json payload required by Slack Incoming WebHooks
    public class Payload
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("blocks")]
        public List<Block> Blocks { get; set; }
    }

    public class BaseText
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("emoji")]
        public bool? Emoji { get; set; }
    }

    public class MarkdownText : BaseText
    {
        public MarkdownText(string text)
        {
            Type = "mrkdwn";
            Text = text;
            Emoji = null;
        }
    }

    public class PlainText : BaseText
    {
        public PlainText(string text)
        {
            Type = "plain_text";
            Text = text;
            Emoji = true;
        }
    }

    public abstract class Block
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public BaseText Text { get; set; }

        [JsonProperty("accessory")]
        public Accessory Accessory { get; set; }

        [JsonProperty("fields")]
        public List<BaseText> Fields { get; set; }

        public Block()
        {
            Text = null;
            Accessory = null;
            Fields = null;
        }
    }

    public class Title : Block
    {
        public Title(string text)
        {
            Type = "header";
            Text = new PlainText(text);
        }
    }

    public class Divider : Block
    {
        public Divider()
        {
            Type = "divider";
        }
    }

    public class MarkdownSection : Block
    {
        public MarkdownSection(string text)
        {
            Type = "section";
            Text = new MarkdownText(text);
        }
    }

    public class LinkButton : Block
    {
        public LinkButton(string sectionText, string buttonText, string urlValue)
        {
            Type = "section";
            Text = new MarkdownText(sectionText);
            Accessory = new Accessory
            {
                Type = "button",
                Text = new PlainText(buttonText),
                Url = urlValue,
                Value = null,
                ActionId = null
            };
        }
    }

    public class Accessory
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public BaseText Text { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("action_id")]
        public string ActionId { get; set; }
    }

    public class Fields : Block
    {
        public Fields(List<BaseText> fields)
        {
            Fields = fields;
            Type = "section";
        }
    }
}
