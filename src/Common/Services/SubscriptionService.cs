namespace Common.Services;

public interface ISubscriptionStore
{
    Task<bool> UpdateAsync(SubscriptionUpdateRequest request);
    Task<bool> RemoveAsync(SubscriptionUpdateRequest request);
    Task<bool> RemoveAsync(long chatId);
}

public interface ISubscriptionService
{
    Task<IEnumerable<long>> GetAsync(double magnitude, double latitude, double longitude, int radiusMeters);
}

public class SubscriptionUpdateRequest
{
    public long ChatId { get; set; }
    public double? Magnitude { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool RemoveLocation { get; set; }
    public bool RemoveSubscription { get; set; }
}
