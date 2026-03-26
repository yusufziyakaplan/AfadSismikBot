namespace AfadSismikBot.Enums;

public static class BotDialog
{
    public const string WELCOME =
        "👋 *AfadSismik Bot'a Hoş Geldiniz!*\n\n" +
        "Bu bot, AFAD verilerini kullanarak Türkiye'deki depremleri anlık olarak bildirir.\n\n" +
        "📌 Komutlar:\n" +
        "/abone - Deprem bildirimlerine abone ol\n" +
        "/iptal - Aboneliği iptal et\n" +
        "/buyukluk - Minimum büyüklük filtresi ayarla\n" +
        "/konum - Konum bazlı filtre ayarla\n" +
        "/konumkaldır - Konum filtresini kaldır\n" +
        "/hakkında - Bot hakkında bilgi\n\n" +
        "📢 Kanal: @afadsismik";

    public const string SUBSCRIBED =
        "✅ *Abonelik aktif!*\n\n" +
        "Artık deprem bildirimlerini alacaksınız.\n" +
        "Büyüklük filtresi için /buyukluk komutunu kullanabilirsiniz.\n" +
        "Konum filtresi için /konum komutunu kullanabilirsiniz.";

    public const string UNSUBSCRIBED =
        "❌ *Abonelik iptal edildi.*\n\n" +
        "Artık deprem bildirimi almayacaksınız.\n" +
        "Tekrar abone olmak için /abone komutunu kullanabilirsiniz.";

    public const string ASK_MAGNITUDE =
        "📊 *Minimum büyüklük seçin:*\n\n" +
        "Seçtiğiniz büyüklüğün altındaki depremler bildirilmeyecektir.";

    public const string MAGNITUDE_SET =
        "✅ Büyüklük filtresi *{0}* olarak ayarlandı.";

    public const string ASK_LOCATION =
        "📍 *Konum gönderin:*\n\n" +
        "Konumunuzu gönderin, yalnızca belirlenen yarıçap (200 km) içindeki depremler bildirilsin.\n\n" +
        "Telegram'da 📎 → Konum seçeneğini kullanabilirsiniz.";

    public const string LOCATION_SET =
        "✅ *Konum filtresi ayarlandı!*\n\n" +
        "200 km yarıçap içindeki depremler bildirilecektir.";

    public const string LOCATION_REMOVED =
        "✅ *Konum filtresi kaldırıldı.*\n\n" +
        "Artık tüm Türkiye'deki depremler bildirilecektir.";

    public const string ABOUT =
        "ℹ️ *AfadSismik Bot*\n\n" +
        "AFAD verilerini kullanarak Türkiye'deki depremleri anlık olarak bildiren bir Telegram botudur.\n\n" +
        "👨‍💻 *Geliştirici:* [GitHub](https://github.com/yusufziyakaplan)\n" +
        "📢 *Kanal:* @afadsismik\n\n" +
        "_Kaynak: AFAD Deprem Dairesi_";

    public const string UNKNOWN_COMMAND =
        "❓ Bilinmeyen komut. Yardım için /start yazabilirsiniz.";
}

public enum Command
{
    Start,
    Abone,
    Iptal,
    Buyukluk,
    Konum,
    KonumKaldir,
    Hakkinda,
    Unknown
}
