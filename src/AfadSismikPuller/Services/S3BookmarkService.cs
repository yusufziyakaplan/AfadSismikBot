using Amazon.S3;
using Amazon.S3.Model;
using Common.Services;

namespace AfadSismikPuller.Services;

public class S3BookmarkService : IBookmarkService
{
    private readonly IAmazonS3 _s3Client;
    private const string BUCKET_NAME = "BOOKMARK_BUCKET";
    private readonly string _bucketName;

    public S3BookmarkService(IAmazonS3 s3Client, IEnvironmentService environmentService)
    {
        _s3Client = s3Client;
        _bucketName = environmentService.GetEnvironmentValue(BUCKET_NAME);
    }

    public async Task<string?> GetBookmarkAsync(string key)
    {
        try
        {
            var response = await _s3Client.GetObjectAsync(_bucketName, key);
            using var reader = new StreamReader(response.ResponseStream);
            return await reader.ReadToEndAsync();
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task SetBookmarkAsync(string key, string value)
    {
        await _s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            ContentBody = value
        });
    }
}
