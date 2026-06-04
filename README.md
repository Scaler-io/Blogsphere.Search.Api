<div align="center">

# 🔍 Blogsphere Search API

**Event-driven search service powered by Elasticsearch**

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Elasticsearch](https://img.shields.io/badge/Elasticsearch-7.17-005571?logo=elasticsearch)](https://www.elastic.co/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-MassTransit-FF6600?logo=rabbitmq)](https://masstransit.io/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)

</div>

---

## 🎯 What It Does

A searchable projection layer for Blogsphere platform data. Indexes operational data from upstream APIs and domain events, serving fast search/count queries via Elasticsearch.

## ✨ Key Features

| Feature | Description |
|---------|-------------|
| 🔎 **Search & Count** | Advanced filtering, sorting, pagination, phrase matching |
| 🔄 **Auto-Sync** | Real-time index updates via RabbitMQ events |
| 📥 **Bulk Seeding** | Full reindex from upstream APIs on-demand |
| 📊 **Observability** | Swagger, Health checks, OpenTelemetry, Serilog |
| 📝 **Event History** | Audit trail in Azure Table Storage |

## 📦 Indexed Domains

```
├── apicluster-search-index      # API Gateway clusters
├── apiroute-search-index         # API Gateway routes
├── managementuser-search-index   # Management users
└── appuser-search-index          # Application users
```

---

## 🚀 Quick Start

### Local Development

```bash
# Navigate to project
cd src/Blogsphere.Search.Api

# Run
dotnet restore && dotnet run
```

**Access**: `http://localhost:5002`

### Docker Compose

```bash
# From src/ directory
docker network create blogsphere_dev_net

# Set secrets
export API_GATEWAY_CLIENT_SECRET="your-secret"
export USER_API_CLIENT_SECRET="your-secret"
export USER_API_SUBSCRIPTION_KEY="your-key"

# Start
docker compose up --build blogsphere.search.api
```

**Access**: `http://localhost:8002`

---

## 🌐 API Endpoints

### Search & Count

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/v1/{indexName}` | Search documents |
| `POST` | `/api/v1/count/{indexName}` | Count documents |

**Supported indexes**: `apicluster-search-index`, `apiroute-search-index`, `managementuser-search-index`

### Seeding

| Method | Endpoint | Action |
|--------|----------|--------|
| `POST` | `/api/v1/seed/api-clusters` | Reindex clusters |
| `POST` | `/api/v1/seed/api-routes` | Reindex routes |
| `POST` | `/api/v1/seed/management-users` | Reindex mgmt users |
| `POST` | `/api/v1/seed/app-users` | Reindex app users |

### Utilities

| Endpoint | Purpose |
|----------|---------|
| `/healthcheck` | Health status |
| `/dashboard` | Health UI |
| `/swagger` | API docs (Swagger) |
| `/scalar/v1` | API docs (Scalar) |

---

## 📝 Request Body Example

```json
{
  "pageIndex": 1,
  "pageSize": 20,
  "sortField": "lastUpdatedOn",
  "sortOrder": "Desc",
  "filters": {
    "status": "active"
  },
  "matchPhrase": "search term",
  "matchPhraseField": "name"
}
```

---

## 🔗 Dependencies

| Service | Purpose |
|---------|---------|
| **Elasticsearch** | Search engine |
| **RabbitMQ** | Event bus |
| **Identity Server** | Token provider |
| **API Gateway API** | Cluster/route data |
| **User API** | User data |
| **Azure Table Storage** | Event history |

---

## ⚙️ Configuration

<details>
<summary><b>Key Config Sections</b> (click to expand)</summary>

### `appsettings.json`

```json
{
  "ElasticSearch": {
    "Uri": "http://localhost:9200",
    "ApiClusterIndex": "apicluster-search-index"
  },
  "EventBus": {
    "Host": "localhost",
    "Port": 5672
  },
  "ProviderSettings": {
    "ApiGatewaySettings": { "BaseUrl": "http://localhost:8000" },
    "UserApiSettings": { "BaseUrl": "http://localhost:8000/user/" }
  }
}
```

### Environment Variables

- `ElasticSearch__Uri`
- `EventBus__Host`
- `ProviderSettings__ApiGatewaySettings__ClientSecret`
- `ProviderSettings__UserApiSettings__ClientSecret`

</details>

---

## 📁 Project Structure

```
src/
  └── Blogsphere.Search.Api/
      ├── Controllers/          # API endpoints
      ├── Services/             # Search logic
      ├── Providers/            # External API clients
      ├── EventBus/             # Event consumers
      └── Entities/             # Data models
```

---

## ⚠️ Known Limitations

- `appuser-search-index` is seeded but not exposed in search/count endpoints
- Authentication middleware present but requires explicit configuration

---

## 📄 License

Licensed under terms in `LICENSE` file.

---

<div align="center">

**Built with ❤️ for Blogsphere Platform**

[Report Bug](https://github.com/your-org/blogsphere-search-api/issues) · [Request Feature](https://github.com/your-org/blogsphere-search-api/issues)

</div>
