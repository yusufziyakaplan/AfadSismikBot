namespace Common.Services;

public interface IEnvironmentService
{
    string GetEnvironmentValue(string key);
}

public class EnvironmentService : IEnvironmentService
{
    public string GetEnvironmentValue(string key) =>
        Environment.GetEnvironmentVariable(key) ?? string.Empty;
}
