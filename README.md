# 🌍 AfadSismik Bot

<div align="center">

![Build and Deploy](https://github.com/yusufziyakaplan/AfadSismikBot/actions/workflows/deploy.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![AWS Lambda](https://img.shields.io/badge/AWS-Lambda-FF9900?logo=awslambda)
![Telegram](https://img.shields.io/badge/Telegram-Bot-26A5E4?logo=telegram)
![License](https://img.shields.io/badge/License-MIT-green)

**AFAD verilerini kullanarak Türkiye'deki depremleri anlık olarak Telegram üzerinden bildiren serverless bir bot.**

[📢 Kanala Katıl](https://t.me/afadsismik) • [🤖 Bota Yaz](https://t.me/AfadSismikBot) • [👨‍💻 Geliştirici](https://github.com/yusufziyakaplan)

</div>

---

## 📸 Önizleme

```
🟠 4.2 Büyüklüğünde Deprem

📍 Konum: Merkez (Kahramanmaraş)
🏙 İl/İlçe: Kahramanmaraş / Merkez
🕐 Zaman: 26.03.2026 22:15:43
🌊 Derinlik: 7.0 km
📌 Koordinat: 37.5842N, 36.9371E

🗺 Haritada Gör

Kaynak: AFAD
```

---

## ✨ Özellikler

- 🔔 **Anlık Bildirim** — Depremler AFAD'a düşer düşmez saniyeler içinde bildirilir
- 📍 **Konum Filtresi** — Sadece belirlediğin konuma yakın (200 km) depremleri al
- 📊 **Büyüklük Filtresi** — Minimum büyüklük eşiği belirle (3.0+, 4.0+, 5.0+, 6.0+)
- 🗺 **Google Maps Linki** — Her depremde harita linki
- 🏙 **Detaylı Konum** — İl, ilçe ve mahalle bilgisi
- 💰 **Tamamen Ücretsiz** — AWS Free Tier ile sıfır maliyet
- ⚡ **Serverless** — Sürekli çalışan sunucu yok, sadece iş varken çalışır

---

## 🏗 Mimari

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│   EventBridge (her dakika)                                  │
│         │                                                   │
│         ▼                                                   │
│   ┌─────────────┐    ┌─────┐    ┌──────────────────┐       │
│   │AfadSismik   │───▶│ SQS │───▶│ AfadSismikNotifier│───▶ Telegram │
│   │Puller       │    └─────┘    └──────────────────┘       │
│   └─────────────┘                                           │
│         │                                                   │
│         ▼                                                   │
│      ┌────┐  ┌──────────┐                                  │
│      │ S3 │  │ DynamoDB │                                  │
│      └────┘  └──────────┘                                  │
│   (bookmark)  (aboneler)                                    │
│                                                             │
│   Telegram ──▶ API Gateway ──▶ AfadSismikBot ──▶ DynamoDB  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Bileşenler

| Bileşen | Görev |
|---------|-------|
| **AfadSismikPuller** | Her dakika AFAD API'den deprem çeker, yeni depremleri SQS'e koyar |
| **AfadSismikNotifier** | SQS'ten mesajları alır, Telegram'a gönderir |
| **AfadSismikBot** | Kullanıcı komutlarını işler, abone yönetimi yapar |
| **DynamoDB** | Abone bilgilerini (chatId, büyüklük, konum) saklar |
| **S3** | Son çekilen deprem tarihini (bookmark) saklar |
| **SQS** | Puller ile Notifier arasında mesaj kuyruğu |

---

## 🛠 Teknoloji Stack

- **[.NET 8](https://dotnet.microsoft.com/)** — C# ile yazılmış Lambda fonksiyonları
- **[AWS Lambda](https://aws.amazon.com/lambda/)** — Serverless fonksiyonlar
- **[AWS SQS](https://aws.amazon.com/sqs/)** — Mesaj kuyruğu
- **[AWS DynamoDB](https://aws.amazon.com/dynamodb/)** — NoSQL veritabanı
- **[AWS S3](https://aws.amazon.com/s3/)** — Nesne depolama
- **[AWS API Gateway](https://aws.amazon.com/api-gateway/)** — REST API
- **[AWS SAM](https://aws.amazon.com/serverless/sam/)** — Serverless uygulama modeli
- **[AFAD Deprem API](https://deprem.afad.gov.tr)** — Deprem veri kaynağı
- **[Telegram Bot API](https://core.telegram.org/bots/api)** — Bildirim kanalı

---

## 🤖 Bot Komutları

| Komut | Açıklama |
|-------|----------|
| `/start` | Botu başlat ve komutları gör |
| `/abone` | Deprem bildirimlerine abone ol |
| `/iptal` | Aboneliği iptal et |
| `/buyukluk` | Minimum büyüklük filtresi ayarla |
| `/konum` | Konum bazlı filtre ayarla (200 km yarıçap) |
| `/konumkaldır` | Konum filtresini kaldır |
| `/hakkında` | Bot hakkında bilgi |

---

## 🚀 Kurulum

### Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [AWS CLI](https://aws.amazon.com/cli/)
- [AWS SAM CLI](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/install-sam-cli.html)
- AWS Hesabı (Free Tier yeterli)
- Telegram Bot Token ([BotFather](https://t.me/BotFather)'dan alınır)

### Adım 1 — Telegram Bot Oluştur

1. Telegram'da [@BotFather](https://t.me/BotFather)'a git
2. `/newbot` yaz
3. Bot adı ve kullanıcı adı belirle
4. Verilen **API Token**'ı kopyala

### Adım 2 — Telegram Kanalı Oluştur

1. Telegram'da yeni bir **public kanal** oluştur
2. Botu kanala **yönetici** olarak ekle
3. Kanal kullanıcı adını not al (örn. `@depremlerim`)

### Adım 3 — AWS Yapılandırması

```bash
# AWS CLI kur ve yapılandır
aws configure
# AWS Access Key ID: <access_key>
# AWS Secret Access Key: <secret_key>
# Default region name: eu-west-1
# Default output format: json
```

### Adım 4 — Deploy için S3 Bucket Oluştur

```bash
aws s3 mb s3://afadsismik-deploy-<AWS_ACCOUNT_ID> --region eu-west-1
```

> AWS Account ID'ni öğrenmek için: `aws sts get-caller-identity --query Account --output text`

### Adım 5 — Build ve Deploy

```bash
# Projeyi klonla
git clone https://github.com/yusufziyakaplan/AfadSismikBot.git
cd AfadSismikBot

# Build et
sam build

# Deploy et
sam deploy \
  --stack-name AfadSismikBot \
  --s3-bucket afadsismik-deploy-<AWS_ACCOUNT_ID> \
  --region eu-west-1 \
  --capabilities CAPABILITY_NAMED_IAM \
  --parameter-overrides TelegramApiToken=<TELEGRAM_BOT_TOKEN>
```

### Adım 6 — Webhook Ayarla

Deploy tamamlandığında çıktıda `BotWebhookUrl` görünür. Bu URL'i Telegram'a kaydet:

```bash
curl -X POST "https://api.telegram.org/bot<TELEGRAM_BOT_TOKEN>/setWebhook" \
  -H "Content-Type: application/json" \
  -d '{"url": "<BotWebhookUrl>"}'
```

### Adım 7 — Bot Komut Menüsünü Ayarla

1. Telegram'da [@BotFather](https://t.me/BotFather)'a git
2. `/mybots` yaz → botunu seç
3. **Edit Bot** → **Edit Commands**
4. Aşağıdaki komutları yapıştır:

```
start - Botu başlat
abone - Deprem bildirimlerine abone ol
iptal - Aboneliği iptal et
buyukluk - Minimum büyüklük filtresi ayarla
konum - Konum bazlı filtre ayarla
konumkaldir - Konum filtresini kaldır
hakkinda - Bot hakkında bilgi
```

✅ Kurulum tamamlandı! Bota `/start` yazarak test edebilirsin.

---

## 💰 AWS Maliyet Analizi

Tüm servisler **AWS Free Tier** kapsamında ücretsizdir:

| Servis | Free Tier Limiti | Tahmini Kullanım |
|--------|-----------------|-----------------|
| Lambda | 1M çağrı/ay | ~45K çağrı/ay |
| SQS | 1M mesaj/ay | Deprem sayısına göre |
| DynamoDB | 25GB + 200M istek/ay | Minimal |
| S3 | 5GB + 20K GET/ay | Minimal |
| API Gateway | 1M çağrı/ay | Bot kullanımına göre |

> **Sonuç: Aylık $0** 🎉

---

## 🔄 CI/CD

`main` branch'e push yapıldığında GitHub Actions otomatik olarak build ve deploy yapar.

GitHub repository'de şu secret'ları tanımla:

| Secret | Açıklama |
|--------|----------|
| `AWS_ACCESS_KEY_ID` | AWS erişim anahtarı |
| `AWS_SECRET_ACCESS_KEY` | AWS gizli anahtar |
| `AWS_REGION` | AWS bölgesi (örn. `eu-west-1`) |
| `AWS_ACCOUNT_ID` | AWS hesap ID'si |
| `TELEGRAM_API_TOKEN` | Telegram bot token'ı |

---

## 📁 Proje Yapısı

```
AfadSismikBot/
├── src/
│   ├── Common/                    # Paylaşılan modeller ve servisler
│   │   ├── Exceptions/
│   │   ├── Models/                # Telegram API modelleri
│   │   └── Services/              # Telegram, Environment servisleri
│   ├── AfadSismikPuller/          # AFAD'dan veri çeken Lambda
│   │   ├── Models/                # Deprem modeli
│   │   └── Services/              # AFAD, S3, SQS, DynamoDB servisleri
│   ├── AfadSismikNotifier/        # SQS'ten Telegram'a gönderen Lambda
│   │   └── Services/
│   └── AfadSismikBot/             # Telegram bot webhook Lambda
│       ├── Enums/                 # Bot dialog metinleri
│       ├── Models/                # Webhook modelleri
│       └── Services/              # Bot ve DynamoDB servisleri
├── .github/
│   └── workflows/
│       └── deploy.yml             # GitHub Actions CI/CD
├── template.yml                   # AWS SAM template
└── AfadSismikBot.sln
```

---

## 🤝 Katkıda Bulunma

1. Fork'la
2. Feature branch oluştur (`git checkout -b feature/yeni-ozellik`)
3. Commit'le (`git commit -m 'Yeni özellik eklendi'`)
4. Push'la (`git push origin feature/yeni-ozellik`)
5. Pull Request aç

---

## 👨‍💻 Geliştirici

**Yusuf Ziya Kaplan**

[![GitHub](https://img.shields.io/badge/GitHub-yusufziyakaplan-181717?logo=github)](https://github.com/yusufziyakaplan)

---

## 📄 Lisans

MIT License — Dilediğin gibi kullanabilirsin.

---

<div align="center">
⭐ Beğendiysen yıldız vermeyi unutma!
</div>
