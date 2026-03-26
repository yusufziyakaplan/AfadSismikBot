using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Services;

namespace AfadSismikNotifier.Services;

public class DynamoDBSubscriptionStore : ISubscriptionStore
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;

    public DynamoDBSubscriptionStore(IAmazonDynamoDB dynamoDb, IEnvironmentService environmentService)
    {
        _dynamoDb = dynamoDb;
        _tableName = environmentService.GetEnvironmentValue("DYNAMODB_TABLE_NAME");
    }

    public async Task<bool> UpdateAsync(SubscriptionUpdateRequest request)
    {
        var updates = new Dictionary<string, AttributeValueUpdate>();

        if (request.Magnitude.HasValue)
            updates["magnitude"] = new AttributeValueUpdate { Action = AttributeAction.PUT, Value = new AttributeValue { N = request.Magnitude.Value.ToString() } };

        if (request.Latitude.HasValue)
            updates["latitude"] = new AttributeValueUpdate { Action = AttributeAction.PUT, Value = new AttributeValue { N = request.Latitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) } };

        if (request.Longitude.HasValue)
            updates["longitude"] = new AttributeValueUpdate { Action = AttributeAction.PUT, Value = new AttributeValue { N = request.Longitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) } };

        if (request.RemoveLocation)
        {
            updates["latitude"] = new AttributeValueUpdate { Action = AttributeAction.DELETE };
            updates["longitude"] = new AttributeValueUpdate { Action = AttributeAction.DELETE };
        }

        if (updates.Count == 0) return true;

        await _dynamoDb.UpdateItemAsync(new UpdateItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue> { { "chatId", new AttributeValue { N = request.ChatId.ToString() } } },
            AttributeUpdates = updates
        });

        return true;
    }

    public async Task<bool> RemoveAsync(SubscriptionUpdateRequest request)
    {
        await RemoveAsync(request.ChatId);
        return true;
    }

    public async Task<bool> RemoveAsync(long chatId)
    {
        await _dynamoDb.DeleteItemAsync(new DeleteItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue> { { "chatId", new AttributeValue { N = chatId.ToString() } } }
        });
        return true;
    }
}
