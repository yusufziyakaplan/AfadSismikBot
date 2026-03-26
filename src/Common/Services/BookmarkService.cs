namespace Common.Services;

public interface IBookmarkService
{
    Task<string?> GetBookmarkAsync(string key);
    Task SetBookmarkAsync(string key, string value);
}
