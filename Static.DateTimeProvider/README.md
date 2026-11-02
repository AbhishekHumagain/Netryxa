# Static.DateTimeProvider

`Static.DateTimeProvider` is a simple and customizable date-time provider for ASP.NET Core applications. It provides a
consistent way to handle date and time throughout your application and allows for easy testing by using mockable
implementations.

## Project Reference

Reference `Static.DateTimeProvider` to your project.

## Exposed Public API

### `IDateTimeProvider`

The `IDateTimeProvider` interface exposes the following properties to retrieve the current date and time:

- UtcNow: Gets the current UTC date and time.
- Now: Gets the current local date and time.
- Today: Gets the current date, with the time set to midnight.

### `DateTimeProvider`

`DateTimeProvider` is the default implementation of the IDateTimeProvider interface.

## Service Registration

To register the default DateTimeProvider:

```csharp
builder.Services.AddDateTimeProvider();
```

## Usage

Once registered, you can inject IDateTimeProvider into your services or controllers:

```csharp
public class MyService
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public MyService(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public void LogCurrentDateTime()
    {
        var currentDateTime = _dateTimeProvider.Now;
        Console.WriteLine($"Current Date and Time: {currentDateTime}");
    }
}
```