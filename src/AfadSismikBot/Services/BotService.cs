using AfadSismikBot.Enums;
using AfadSismikBot.Models;
using Common.Models;
using Common.Services;

namespace AfadSismikBot.Services;

public interface IBotService
{
    Task HandleMessageAsync(Message message);
    Task HandleCallbackQueryAsync(CallbackQuery callbackQuery);
}

public class BotService : IBotService
{
    private readonly ISubscriptionStore _subscriptionStore;
    private readonly ITelegramService _telegramService;

    public BotService(ISubscriptionStore subscriptionStore, ITelegramService telegramService)
    {
        _subscriptionStore = subscriptionStore;
        _telegramService = telegramService;
    }

    public async Task HandleMessageAsync(Message message)
    {
        var chatId = message.Chat.Id;

        if (message.Location is not null)
        {
            await SetLocationAsync(chatId, message.Location);
            return;
        }

        var command = ParseCommand(message.Text);

        await (command switch
        {
            Command.Start => SendMessageAsync(chatId, BotDialog.WELCOME),
            Command.Abone => SubscribeAsync(chatId),
            Command.Iptal => UnsubscribeAsync(chatId),
            Command.Buyukluk => AskMagnitudeAsync(chatId),
            Command.Konum => SendMessageAsync(chatId, BotDialog.ASK_LOCATION),
            Command.KonumKaldir => RemoveLocationAsync(chatId),
            Command.Hakkinda => SendMessageAsync(chatId, BotDialog.ABOUT),
            _ => SendMessageAsync(chatId, BotDialog.UNKNOWN_COMMAND)
        });
    }

    public async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery)
    {
        if (!double.TryParse(callbackQuery.Data, out var magnitude)) return;

        var chatId = callbackQuery.From.Id;

        await _subscriptionStore.UpdateAsync(new SubscriptionUpdateRequest
        {
            ChatId = chatId,
            Magnitude = magnitude
        });

        if (callbackQuery.Message is not null)
            await _telegramService.DeleteMessage(new TelegramDeleteMessage
            {
                ChatId = chatId,
                MessageId = callbackQuery.Message.MessageId
            });

        await _telegramService.AnswerCallbackQuery(new AnswerCallbackQuery
        {
            CallbackQueryId = callbackQuery.Id,
            Text = string.Format(BotDialog.MAGNITUDE_SET, magnitude == 0 ? "Hepsi" : $"{magnitude}+")
        });

        await SendMessageAsync(chatId, string.Format(BotDialog.MAGNITUDE_SET, magnitude == 0 ? "Hepsi" : $"{magnitude}+"));
    }

    private async Task SubscribeAsync(long chatId)
    {
        await _subscriptionStore.UpdateAsync(new SubscriptionUpdateRequest { ChatId = chatId, Magnitude = 0 });
        await SendMessageAsync(chatId, BotDialog.SUBSCRIBED);
    }

    private async Task UnsubscribeAsync(long chatId)
    {
        await _subscriptionStore.RemoveAsync(chatId);
        await SendMessageAsync(chatId, BotDialog.UNSUBSCRIBED);
    }

    private async Task AskMagnitudeAsync(long chatId)
    {
        var markup = new TelegramInlineKeyboardMarkup
        {
            InlineKeyboard = new List<IEnumerable<TelegramInlineKeyboardButton>>
            {
                new List<TelegramInlineKeyboardButton>
                {
                    new() { Text = "🌍 Hepsi", CallBackData = "0" },
                    new() { Text = "3.0+", CallBackData = "3" },
                    new() { Text = "4.0+", CallBackData = "4" },
                    new() { Text = "5.0+", CallBackData = "5" },
                    new() { Text = "6.0+", CallBackData = "6" }
                }
            }
        };

        await _telegramService.SendMessage(new TelegramMessage
        {
            ChatId = chatId.ToString(),
            Text = BotDialog.ASK_MAGNITUDE,
            ReplyMarkup = markup
        });
    }

    private async Task SetLocationAsync(long chatId, Location location)
    {
        await _subscriptionStore.UpdateAsync(new SubscriptionUpdateRequest
        {
            ChatId = chatId,
            Latitude = location.Latitude,
            Longitude = location.Longitude
        });
        await SendMessageAsync(chatId, BotDialog.LOCATION_SET);
    }

    private async Task RemoveLocationAsync(long chatId)
    {
        await _subscriptionStore.UpdateAsync(new SubscriptionUpdateRequest { ChatId = chatId, RemoveLocation = true });
        await SendMessageAsync(chatId, BotDialog.LOCATION_REMOVED);
    }

    private Task SendMessageAsync(long chatId, string text) =>
        _telegramService.SendMessage(new TelegramMessage { ChatId = chatId.ToString(), Text = text });

    private static Command ParseCommand(string? text) => text?.Split('@')[0].ToLower() switch
    {
        "/start" => Command.Start,
        "/abone" => Command.Abone,
        "/iptal" => Command.Iptal,
        "/buyukluk" => Command.Buyukluk,
        "/konum" => Command.Konum,
        "/konumkaldır" or "/konumkaldir" => Command.KonumKaldir,
        "/hakkında" or "/hakkinda" => Command.Hakkinda,
        _ => Command.Unknown
    };
}
