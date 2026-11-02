# Static.Redis

Static.Redis is a lightweight library providing caching services via Redis. It abstracts common Redis operations into a simple interface, making it easy to integrate Redis caching into your .NET applications. As part of the **BuildingBlocks** project, Static.Redis is designed to be referenced and utilized by other microservices.

## Getting Started

### 1. Setting Up Redis with Docker
To run Redis locally, use the following Docker command:

```bash
docker run -d -p 6379:6379 --name static-redis redis:latest
```

This command pulls the latest Redis image, runs it as a container, and maps the container's port `6379` to your local machine's port `6379`.

### 2. Integration

Add a reference to the `Static.Redis` class library in your microservice project. Ensure the library is included as a project reference.


### 3. Configuring the Library
Register the caching service in your application during startup. In your `Startup.cs` or `Program.cs`, add the following:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Static.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add Static.Redis service
builder.Services.AddStaticRedis(builder.Configuration);

var app = builder.Build();

// Other app configurations
app.Run();
```

Ensure your `appsettings.json` has the Redis configuration, typically including the connection string:

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

### 4. Using the Caching Service
The library exposes the `ICacheService` interface for interacting with the cache. Inject it into your services or controllers as needed:

### 6. Contributions
Contributions are welcome! Feel free to open issues or submit pull requests.


