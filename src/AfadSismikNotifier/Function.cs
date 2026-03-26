using AfadSismikNotifier.Services;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Common.Exceptions;
using Common.Models;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace AfadSismikNotifier;

public class Function
{
    private static readonly IServiceProvider _serviceProvider;

    static Function()
    {
        var services = new ServiceCollection();
        services.AddTransient<IEnvironmentService, EnvironmentService>();
        services.AddTransient<ITelegramService, TelegramService>();
        services.AddTransient<ISubscriptionStore, DynamoDBSubscriptionStore>();
        services.AddAWSService<IAmazonDynamoDB>(new Amazon.Extensions.NETCore.Setup.AWSOptions { Region = Amazon.RegionEndpoint.EUWest1 });
        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        LambdaLogger.Log($"AfadSismikNotifier başladı - {sqsEvent.Records.Count} mesaj\n");

        var telegramService = _serviceProvider.GetRequiredService<ITelegramService>();
        var subscriptionStore = _serviceProvider.GetRequiredService<ISubscriptionStore>();

        foreach (var record in sqsEvent.Records)
        {
            var message = JsonSerializer.Deserialize<TelegramMessage>(record.Body);
            if (message is null) continue;

            try
            {
                await telegramService.SendMessage(message);
                LambdaLogger.Log($"Mesaj gönderildi: {message.ChatId}\n");
            }
            catch (TelegramApiException ex) when (ex.Response.ErrorCode == 403)
            {
                LambdaLogger.Log($"Bot engellendi, abone siliniyor: {message.ChatId}\n");
                if (long.TryParse(message.ChatId, out var chatId))
                    await subscriptionStore.RemoveAsync(chatId);
            }
        }

        LambdaLogger.Log($"AfadSismikNotifier tamamlandı\n");
    }
}
