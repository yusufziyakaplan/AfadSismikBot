using AfadSismikBot.Models;
using AfadSismikBot.Services;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
namespace AfadSismikBot;

public class Function
{
    private static readonly IServiceProvider _serviceProvider;

    static Function()
    {
        var services = new ServiceCollection();
        services.AddTransient<IEnvironmentService, EnvironmentService>();
        services.AddTransient<ITelegramService, TelegramService>();
        services.AddTransient<ISubscriptionStore, DynamoDBSubscriptionStore>();
        services.AddTransient<IBotService, BotService>();
        services.AddAWSService<IAmazonDynamoDB>(new Amazon.Extensions.NETCore.Setup.AWSOptions { Region = Amazon.RegionEndpoint.EUWest1 });
        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        LambdaLogger.Log($"AfadSismikBot isteği alındı\n");

        try
        {
            var webhook = JsonSerializer.Deserialize<WebhookMessage>(request.Body ?? "{}");
            if (webhook is null) return Ok();

            var botService = _serviceProvider.GetRequiredService<IBotService>();

            if (webhook.CallbackQuery is not null)
                await botService.HandleCallbackQueryAsync(webhook.CallbackQuery);
            else if (webhook.Message is not null)
                await botService.HandleMessageAsync(webhook.Message);
        }
        catch (Exception ex)
        {
            LambdaLogger.Log($"Hata: {ex.Message}\n");
        }

        return Ok();
    }

    private static APIGatewayProxyResponse Ok() => new() { StatusCode = 200, Body = "ok" };
}
