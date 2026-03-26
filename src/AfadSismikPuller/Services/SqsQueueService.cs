using Amazon.SQS;
using Amazon.SQS.Model;
using Common.Models;
using Common.Services;
using System.Text.Json;

namespace AfadSismikPuller.Services;

public class SqsQueueService : IQueueService
{
    private readonly IAmazonSQS _sqsClient;
    private const string SQS_QUEUE_URL = "SQS_QUEUE_URL";
    private readonly string _queueUrl;

    public SqsQueueService(IAmazonSQS sqsClient, IEnvironmentService environmentService)
    {
        _sqsClient = sqsClient;
        _queueUrl = environmentService.GetEnvironmentValue(SQS_QUEUE_URL);
    }

    public async Task EnqueueAsync(IEnumerable<TelegramMessage> messages)
    {
        var entries = messages.Select((msg, i) => new SendMessageBatchRequestEntry
        {
            Id = i.ToString(),
            MessageBody = JsonSerializer.Serialize(msg)
        }).ToList();

        if (entries.Count == 0) return;

        await _sqsClient.SendMessageBatchAsync(new SendMessageBatchRequest
        {
            QueueUrl = _queueUrl,
            Entries = entries
        });
    }
}
