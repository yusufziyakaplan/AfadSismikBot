using Common.Exceptions;
using Common.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Common.Services;

public interface ITelegramService
{
    Task<bool> SendMessage(TelegramMessage message);
    Task<bool> DeleteMessage(TelegramDeleteMessage message);
    Task<bool> AnswerCallbackQuery(AnswerCallbackQuery query);
}

public class TelegramService : ITelegramService
{
    private static readonly HttpClient _httpClient = new();
    private const string TELEGRAM_API_TOKEN = "TELEGRAM_API_TOKEN";
    private readonly string _baseUrl;

    public TelegramService(IEnvironmentService environmentService)
    {
        var token = environmentService.GetEnvironmentValue(TELEGRAM_API_TOKEN);
        _baseUrl = $"https://api.telegram.org/bot{token}";
    }

    public async Task<bool> SendMessage(TelegramMessage message)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/sendMessage", message);
        return await HandleResponse(response);
    }

    public async Task<bool> DeleteMessage(TelegramDeleteMessage message)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/deleteMessage", message);
        return await HandleResponse(response);
    }

    public async Task<bool> AnswerCallbackQuery(AnswerCallbackQuery query)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/answerCallbackQuery", query);
        return await HandleResponse(response);
    }

    private static async Task<bool> HandleResponse(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TelegramResponse>(content);

        if (result is null || !result.Ok)
            throw new TelegramApiException(new TelegramErrorResponse
            {
                ErrorCode = result?.ErrorCode ?? 0,
                Description = result?.Description ?? "Bilinmeyen hata"
            });

        return true;
    }
}
