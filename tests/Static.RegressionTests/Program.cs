using System.Diagnostics;
using System.Net;
using System.Text.Json;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Static.Infrastructure.Persistence;
using Static.API.Middleware.ExceptionHandler;
using Static.MediatR.Behaviours.Performance;
using Static.MediatR.Behaviours.Tracing;
using Static.Results;
using Static.UserSession.CurrentUser.Service;

var context = new DefaultHttpContext();
context.Response.Body = new MemoryStream();
context.Request.Headers.Authorization = "Bearer secret";
context.Request.Headers.Cookie = "secret=credential";
context.Request.QueryString = new QueryString("?token=secret");
await ErrorResponseBuilder.CreateBuilder(HttpStatusCode.InternalServerError, "Failure")
    .AttachContext(context).IncludeHttpRequestInfo().EnableErrorDetails(true)
    .WriteToResponseAsync(default);
Require(context.Response.StatusCode == 500, "Errors must set HTTP status");
context.Response.Body.Position = 0;
var json = await new StreamReader(context.Response.Body).ReadToEndAsync();
Require(!json.Contains("secret"), "Request diagnostics must omit credentials");
Require(json.Contains("traceId"), "Errors must contain a correlation ID");

var validationContext = new DefaultHttpContext();
validationContext.Response.Body = new MemoryStream();
var handler = new ValidationExceptionHandler(new CaptureLogger<ValidationExceptionHandler>());
await handler.TryHandleAsync(validationContext,
    new ValidationException([new FluentValidation.Results.ValidationFailure("Password", "Invalid") { AttemptedValue = "secret" }]), default);
Require(validationContext.Response.StatusCode == 422, "Validation errors must return 422");
validationContext.Response.Body.Position = 0;
Require(!(await new StreamReader(validationContext.Response.Body).ReadToEndAsync()).Contains("secret"), "Validation diagnostics must omit attempted values");

var logger = new CaptureLogger<Request>();
var user = new CurrentUserService(new HttpContextAccessor { HttpContext = new DefaultHttpContext() });
var performance = new PerformanceBehaviour<Request, Result>(logger, user);
await performance.Handle(new Request(), async _ => { await Task.Delay(1100); return Result.Success(); }, default);
Require(logger.Messages.Any(x => x.Contains("Long Running")), "1100ms must trigger slow request logging");

using var listener = new ActivityListener { ShouldListenTo = source => source.Name == RequestTracing.SourceName,
    Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData };
ActivitySource.AddActivityListener(listener);
Activity? captured = null;
listener.ActivityStopped = activity => captured = activity;
var tracing = new TracingBehavior<Request, Result>();
await tracing.Handle(new Request(), _ => Task.FromResult(Result.Success()), default);
Require(captured?.Status == ActivityStatusCode.Ok, "Successful requests need a trace span");
using var cancelled = new CancellationTokenSource();
cancelled.Cancel();
try {
    await tracing.Handle(new Request(), token => { Require(token == cancelled.Token, "Cancellation must propagate"); throw new OperationCanceledException(token); }, cancelled.Token);
    throw new Exception("Expected cancellation");
} catch (OperationCanceledException) { }
// Model inspection does not connect to PostgreSQL or create a database.
using var database = new StaticDbContext(new DbContextOptionsBuilder<StaticDbContext>()
    .UseNpgsql("Host=localhost;Database=unused_model_check;Username=unused;Password=unused").Options);
Require(!database.Database.GetMigrations().Any(), "The starter must ship without inherited migrations");
var model = database.GetService<IDesignTimeModel>().Model;
var expectedEntities = new[] { "DefaultSetting", "User", "UserLoginAttempt", "UserNotification", "UserNotificationSocket" };
Require(model.GetEntityTypes().Select(entity => entity.ClrType.Name).Order().SequenceEqual(expectedEntities.Order()),
    "Only reusable identity/settings/notification models belong in the fresh starter");
Require(model.GetEntityTypes().All(entity => !entity.GetSeedData().Any()), "The starter model must contain no seed records");
Console.WriteLine("All regression checks passed.");

static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
sealed record Request : IRequest<Result>;
sealed class CaptureLogger<T> : ILogger<T>
{
    public List<string> Messages { get; } = [];
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel level) => true;
    public void Log<TState>(LogLevel level, EventId id, TState state, Exception? ex, Func<TState, Exception?, string> formatter) => Messages.Add(formatter(state, ex));
}
