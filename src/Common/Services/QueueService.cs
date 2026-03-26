using Common.Models;

namespace Common.Services;

public interface IQueueService
{
    Task EnqueueAsync(IEnumerable<TelegramMessage> messages);
}
