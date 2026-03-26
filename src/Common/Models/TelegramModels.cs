using System.Text.Json.Serialization;
using System.Text.Json;

namespace Common.Models;

public class TelegramMessage
{
    [JsonPropertyName("chat_id")]
    public string ChatId { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("parse_mode")]
    public string ParseMode { get; set; } = "markdown";

    [JsonPropertyName("disable_web_page_preview")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool DisableWebPagePreview { get; set; } = true;

    [JsonPropertyName("disable_notification")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool DisableNotification { get; set; }

    [JsonPropertyName("reply_markup")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? ReplyMarkup { get; set; }
}

public class TelegramDeleteMessage
{
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    [JsonPropertyName("message_id")]
    public int MessageId { get; set; }
}

public class TelegramInlineKeyboardMarkup
{
    [JsonPropertyName("inline_keyboard")]
    public List<IEnumerable<TelegramInlineKeyboardButton>> InlineKeyboard { get; set; } = new();
}

public class TelegramInlineKeyboardButton
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("callback_data")]
    public string CallBackData { get; set; } = string.Empty;
}

public class TelegramReplyKeyboardMarkup
{
    [JsonPropertyName("keyboard")]
    public List<IEnumerable<TelegramKeyboardButton>> Keyboard { get; set; } = new();

    [JsonPropertyName("one_time_keyboard")]
    public bool OneTimeKeyboard { get; set; } = true;

    [JsonPropertyName("resize_keyboard")]
    public bool ResizeKeyboard { get; set; } = true;
}

public class TelegramKeyboardButton
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("request_location")]
    public bool RequestLocation { get; set; }
}

public class AnswerCallbackQuery
{
    [JsonPropertyName("callback_query_id")]
    public string CallbackQueryId { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

public class TelegramResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("error_code")]
    public int ErrorCode { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
