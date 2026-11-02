# OpenTelemetry Building Block for Microservices

This class library provides a comprehensive OpenTelemetry integration for microservices, including tracing, metrics, and logging functionalities. It simplifies setting up telemetry for your API projects with minimal configuration.

## Features

- **Tracing**: Configure distributed tracing with OpenTelemetry.
- **Metrics**: Collect application metrics with custom meters.
- **Logging**: Enhanced logging with Serilog, including header redaction for sensitive data.

## Getting Started

### Integration

Add a reference to the `Static.OpenTelemetry` class library in your microservice project. Ensure the library is included as a project reference.

### Configuration

#### 1. Service Extensions

Call the following methods in your `Startup.cs` or `Program.cs` file:

1. **Add Static OpenTelemetry**

   ```csharp
   services.AddStaticOpenTelemetry(hostEnvironment, configuration, instrumentations);
   ```

   This method sets up OpenTelemetry tracing, metrics, and logging configurations.

2. **Use Static Serilog Request Logging**

   ```csharp
   app.UseStaticSerilogRequestLogging(configuration);
   ```

   This middleware enables request logging using Serilog.

#### 2. Docker Compose

To run the telemetry backend (Grafana, OpenTelemetry Collector, etc.), navigate to the `\Static.OpenTelemetry\Compose` folder and run the following command:

```bash
docker-compose up
```

Or, Run the `Compose:OTel` configuration profile, if you are using Rider.

#### 3. Required Configuration

Refer to `appsettings.example.json` and include the corresponding section in your `appsettings.json` file.
