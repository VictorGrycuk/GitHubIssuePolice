# SlackClient

## Constructor

To instantiate the SlackClient class is required to pass the webhook previously created.

```csharp | SlackClient.cs
private readonly Uri _uri;

public SlackClient(string urlWithAccessToken)
{
    _uri = new Uri(urlWithAccessToken);
}
```

## Methods

### PostMessage

The class a single overloaded method, `PostMessage()`.

They are pretty straightforward, one receives several parameters (including the list of blocks), and then calls the overload which serialize the information and sends it as a payload to Slack.

```csharp | SlackClient.cs
public void PostMessage(string text, string username = null, string channel = null, List<Block> blocks = null)
{
    Payload payload = new Payload()
    {
        Channel = channel,
        Username = username,
        Text = text,
/*!*/        Blocks = blocks		// --> When blocks are are added to the payload, the Text parameter is used for the toast notification
    };

    PostMessage(payload);
}
```

```csharp | SlackClient.cs
public void PostMessage(Payload payload)
{
    string payloadJson = JsonConvert.SerializeObject(payload, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

    using (WebClient client = new WebClient())
    {
        var data = new NameValueCollection();
        data["payload"] = payloadJson;

        var response = client.UploadValues(_uri, "POST", data);

        //The response text is usually "ok"
/*!*/        string responseText = _encoding.GetString(response);		// --> Sadly the response of a webhook does not return the timestamp, therefore, it can't be replied to 
    }
}
```

<br>

## Block Classes

The block classes are C# representation of the [blocks](https://api.slack.com/block-kit) in Slack. They are used to format the messages sent to Slack more neatly.

At the moment they are only used to present information for the user, not for the user to interact with.

### Block

The abstract class `Block` is a superclass that all the other block classes derive from.

All the blocks use the `Type` property, most of them use the `Text` property, and some of them use the `Accessory` and `Fields` properties.

The constructor set everything except the `Type` to `null` to avoid being added to the payload. Otherwise, if they are added to a class that don't use them, the webhook will return an error 400.

```csharp | SlackClient.cs
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
```

<br>

### BaseText, PlainText, and MarkdownText

There are two types of texts, plain text and markdown. Some blocks can use either one and some blocks will use one of them and will return a request error if they use the wrong one.


```csharp | SlackClient.cs
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
```

<br>

### Accessory

Some blocks use accessories, which usually are buttons.

Again, all the properties except `Type` are set to null as no all the accesories use all the properties, and will return an error if they are present.

```csharp | SlackClient.cs
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

    [JsonProperty("options")]
    public List<Option> Options { get; set; }

    public Accessory()
    {
        Value = null;
        Url = null;
        ActionId = null;
        Options = null;
    }
}
```

<br>

### Option

The option is part of the `Accessory` class, and use in the `Overflow` block.

```csharp | SlackClient.cs
public class Option
{
    [JsonProperty("text")]
    public PlainText Text { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }
}
```

<br>

### Fields

A simple block that shows a list of texts in two columns.

```csharp | SlackClient.cs
public class Fields : Block
{
    public Fields(List<BaseText> fields)
    {
        Fields = fields;
        Type = "section";
    }
}
```

<br>

### Overflow

The overflow has a text body, and a button that shows collection of `Option`.

**Important:** The overflow only supports up to 5 options.

```csharp | SlackClient.cs
public class Overflow : Block
{   
    public Overflow(string text)
    {
        Type = "section";
        Text = new MarkdownText(text);
        Accessory = new Accessory
        {
            Type = "overflow",
            Options = new List<Option>()
        };
    }
}
```



> :ToCPrevNext