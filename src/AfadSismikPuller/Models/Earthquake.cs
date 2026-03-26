using System.Text.Json.Serialization;

namespace AfadSismikPuller.Models;

public class Earthquake
{
    [JsonPropertyName("eventID")]
    public string EventId { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("latitude")]
    public string Latitude { get; set; } = string.Empty;

    [JsonPropertyName("longitude")]
    public string Longitude { get; set; } = string.Empty;

    [JsonPropertyName("depth")]
    public string Depth { get; set; } = string.Empty;

    [JsonPropertyName("magnitude")]
    public string Magnitude { get; set; } = string.Empty;

    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [JsonPropertyName("province")]
    public string Province { get; set; } = string.Empty;

    [JsonPropertyName("district")]
    public string District { get; set; } = string.Empty;

    public double LatitudeValue => double.TryParse(Latitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : 0;
    public double LongitudeValue => double.TryParse(Longitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : 0;
    public double MagnitudeValue => double.TryParse(Magnitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : 0;
    public double DepthValue => double.TryParse(Depth, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : 0;

    public string ToTelegramMessage()
    {
        var mag = MagnitudeValue;
        var emoji = mag >= 6 ? "🔴" : mag >= 5 ? "🟠" : mag >= 4 ? "🟡" : "🟢";
        return $"{emoji} *{mag:F1} Büyüklüğünde Deprem*\n\n" +
               $"📍 *Konum:* {Location}\n" +
               $"🏙 *İl/İlçe:* {Province} / {District}\n" +
               $"🕐 *Zaman:* {Date:dd.MM.yyyy HH:mm:ss}\n" +
               $"🌊 *Derinlik:* {DepthValue:F1} km\n" +
               $"📌 *Koordinat:* {LatitudeValue:F4}N, {LongitudeValue:F4}E\n\n" +
               $"[🗺 Haritada Gör](https://www.google.com/maps?q={LatitudeValue},{LongitudeValue})\n\n" +
               $"_Kaynak: AFAD_";
    }
}
