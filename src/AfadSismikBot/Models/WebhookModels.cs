using System.Text.Json.Serialization;

namespace AfadSismikBot.Models;

public class WebhookMessage
{
    [JsonPropertyName("message")]
    public Message? Message { get; set; }

    [JsonPropertyName("callback_query")]
    public CallbackQuery? CallbackQuery { get; set; }
}

public class Message
{
    [JsonPropertyName("message_id")]
    public int MessageId { get; set; }

    [JsonPropertyName("chat")]
    public Chat Chat { get; set; } = new();

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("location")]
    public Location? Location { get; set; }
}

public class Chat
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
}

public class Location
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}

public class CallbackQuery
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("from")]
    public From From { get; set; } = new();

    [JsonPropertyName("message")]
    public Message? Message { get; set; }

    [JsonPropertyName("data")]
    public string? Data { get; set; }
}

public class From
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
}
