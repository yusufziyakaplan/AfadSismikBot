using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Services;

namespace AfadSismikPuller.Services;

public class DynamoDBSubscriptionService : ISubscriptionService
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private const string TABLE_NAME = "DYNAMODB_TABLE_NAME";
    private readonly string _tableName;

    public DynamoDBSubscriptionService(IAmazonDynamoDB dynamoDb, IEnvironmentService environmentService)
    {
        _dynamoDb = dynamoDb;
        _tableName = environmentService.GetEnvironmentValue(TABLE_NAME);
    }

    public async Task<IEnumerable<long>> GetAsync(double magnitude, double latitude, double longitude, int radiusMeters)
    {
        var request = new ScanRequest { TableName = _tableName };
        var response = await _dynamoDb.ScanAsync(request);

        return response.Items
            .Where(item =>
            {
                if (!item.TryGetValue("magnitude", out var mag)) return false;
                if (!double.TryParse(mag.N, out var minMag)) return false;
                if (magnitude < minMag) return false;

                if (!item.TryGetValue("latitude", out var lat) ||
                    !item.TryGetValue("longitude", out var lon)) return true;

                if (!double.TryParse(lat.N, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var subLat) ||
                    !double.TryParse(lon.N, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var subLon)) return true;

                return CalculateDistance(latitude, longitude, subLat, subLon) <= radiusMeters;
            })
            .Select(item => long.Parse(item["chatId"].N))
            .ToList();
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
