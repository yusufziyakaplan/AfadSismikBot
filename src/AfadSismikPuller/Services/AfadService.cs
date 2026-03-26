using AfadSismikPuller.Models;
using Common.Services;
using System.Net.Http.Json;

namespace AfadSismikPuller.Services;

public interface IAfadService
{
    Task<IEnumerable<Earthquake>> GetEarthquakesAsync(DateTime since);
}

public class AfadService : IAfadService
{
    private static readonly HttpClient _httpClient = new();
    private const string AFAD_API_URL = "https://deprem.afad.gov.tr/apiv2/event/filter";

    public async Task<IEnumerable<Earthquake>> GetEarthquakesAsync(DateTime since)
    {
        var url = $"{AFAD_API_URL}?start={since:yyyy-MM-ddTHH:mm:ss}&end={DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss}&minmag=0&orderby=timedesc&limit=100";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"AFAD API hata döndü: {response.StatusCode}");
            return Enumerable.Empty<Earthquake>();
        }
        var result = await response.Content.ReadFromJsonAsync<List<Earthquake>>();
        return result ?? new List<Earthquake>();
    }
}
