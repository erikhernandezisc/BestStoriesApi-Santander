# BestStoriesApi

A RESTful API built with ASP.NET Core to fetch the top stories from Hacker News, optimized with LazyCache to avoid overloading the external API.

---

## 🚀 What does this API do?

This service exposes an endpoint to retrieve the best stories from Hacker News, sorted in descending order by score. It uses the official public Hacker News endpoints:

- [Best stories list](https://hacker-news.firebaseio.com/v0/beststories.json)
- [Story details](https://hacker-news.firebaseio.com/v0/item/{id}.json)

---

## 🔧 Technologies Used

- ASP.NET Core 8 (Web API)
- LazyCache 2.4.0
- IHttpClientFactory
- Swagger/OpenAPI
- System.Text.Json

---

## 📦 Installation

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/BestStoriesApi.git
cd BestStoriesApi
```

### 2. Restore packages and build

```bash
dotnet restore
dotnet build
```

### 3. Run the project

```bash
dotnet run
```

Visit [https://localhost:5001/swagger](https://localhost:5001/swagger) to test via Swagger UI.

---

## 📡 Main Endpoint

```
GET /api/stories?count=10
```

### Parameters

| Name  | Type | Required | Description                        |
|-------|------|----------|------------------------------------|
| count | int  | No       | Number of stories (max: 100)       |

### Sample Response

```json
[
  {
    "title": "A uBlock Origin update was rejected...",
    "uri": "https://github.com/uBlockOrigin/uBlock-issues/issues/745",
    "postedBy": "ismaildonmez",
    "time": "2019-10-12T13:43:01+00:00",
    "score": 1716,
    "commentCount": 572
  }
]
```

---

## 🧠 Assumptions & Technical Decisions

- **LazyCache** is used to cache:
  - Best story IDs (`10 minutes`)
  - Individual story details (`10 minutes`)
- Since stories are mostly immutable, caching is appropriate to reduce API load.
- Future enhancement: a `refresh=true` query parameter to force cache refresh.
- A simplified DTO is returned with only required fields.

---

## 🌱 Future Improvements

- Add **distributed caching (e.g. Redis)** for multi-instance deployments.
- Support `refresh` query param to invalidate cache manually.
- Add logging and metrics to monitor usage and performance.
- Include unit tests and load tests using Artillery or k6.

---

## 👨‍💻 Author

Erik Raymundo Hernández Camacho  
[LinkedIn](https://www.linkedin.com/in/erickelmay) • [GitHub](https://github.com/erikhernandezisc)