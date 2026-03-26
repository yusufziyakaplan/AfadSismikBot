using AfadSismikPuller.Services;
using Amazon.DynamoDBv2;
using Amazon.Lambda.CloudWatchEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.S3;
using Amazon.SQS;
using Common.Models;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(JsonSerializer))]
namespace AfadSismikPuller;

public class Function
{
    private static readonly IServiceProvider _serviceProvider;
    private const string TELEGRAM_CHANNEL = "TELEGRAM_CHANNEL_NAME";
    private const int SEARCH_RADIUS_METERS = 200000;

    static Function()
    {
        var services = new ServiceCollection();
        services.AddTransient<IEnvironmentService, EnvironmentService>();
        services.AddTransient<IAfadService, AfadService>();
        services.AddTransient<IBookmarkService, S3BookmarkService>();
        services.AddTransient<IQueueService, SqsQueueService>();
        services.AddTransient<ISubscriptionService, DynamoDBSubscriptionService>();
        services.AddAWSService<IAmazonS3>(new Amazon.Extensions.NETCore.Setup.AWSOptions { Region = Amazon.RegionEndpoint.EUWest1 });
        services.AddAWSService<IAmazonSQS>(new Amazon.Extensions.NETCore.Setup.AWSOptions { Region = Amazon.RegionEndpoint.EUWest1 });
        services.AddAWSService<IAmazonDynamoDB>(new Amazon.Extensions.NETCore.Setup.AWSOptions { Region = Amazon.RegionEndpoint.EUWest1 });
        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task FunctionHandler(CloudWatchEvent<object> @event, ILambdaContext context)
    {
        LambdaLogger.Log("AfadSismikPuller başladı\n");

        var bookmarkService = _serviceProvider.GetRequiredService<IBookmarkService>();
        var afadService = _serviceProvider.GetRequiredService<IAfadService>();
        var environmentService = _serviceProvider.GetRequiredService<IEnvironmentService>();
        var queueService = _serviceProvider.GetRequiredService<IQueueService>();
        var subscriptionService = _serviceProvider.GetRequiredService<ISubscriptionService>();

        var lastFetchStr = await bookmarkService.GetBookmarkAsync("last_fetch_date");
        var lastFetch = DateTime.TryParse(lastFetchStr, out var d) ? d : DateTime.UtcNow.AddMinutes(-2);
        var channelName = environmentService.GetEnvironmentValue(TELEGRAM_CHANNEL);

        var earthquakes = await afadService.GetEarthquakesAsync(lastFetch);
        var newEarthquakes = earthquakes
            .Where(q => q.Date > lastFetch)
            .OrderBy(q => q.Date)
            .Take(10)
            .ToList();

        if (newEarthquakes.Count == 0)
        {
            LambdaLogger.Log("Yeni deprem yok\n");
            return;
        }

        LambdaLogger.Log($"{newEarthquakes.Count} yeni deprem bulundu\n");

        var lastNotified = lastFetch;

        foreach (var earthquake in newEarthquakes)
        {
            var subscribers = (await subscriptionService.GetAsync(
                earthquake.MagnitudeValue, earthquake.LatitudeValue,
                earthquake.LongitudeValue, SEARCH_RADIUS_METERS)).ToList();

            LambdaLogger.Log($"Deprem: {earthquake.Location} - {earthquake.MagnitudeValue} - {subscribers.Count} abone\n");

            // Abonelere 10'arlı gruplar halinde gönder
            for (int i = 0; i < subscribers.Count; i += 10)
            {
                var chunk = subscribers.Skip(i).Take(10);
                await queueService.EnqueueAsync(chunk.Select(chatId => new TelegramMessage
                {
                    ChatId = chatId.ToString(),
                    Text = earthquake.ToTelegramMessage()
                }));
            }

            // Kanala gönder
            await queueService.EnqueueAsync(new[]
            {
                new TelegramMessage
                {
                    ChatId = $"@{channelName}",
                    DisableNotification = earthquake.MagnitudeValue < 4,
                    Text = earthquake.ToTelegramMessage()
                }
            });

            lastNotified = earthquake.Date;
        }

        await bookmarkService.SetBookmarkAsync("last_fetch_date", lastNotified.ToString("o"));
        LambdaLogger.Log("AfadSismikPuller tamamlandı\n");
    }
}
