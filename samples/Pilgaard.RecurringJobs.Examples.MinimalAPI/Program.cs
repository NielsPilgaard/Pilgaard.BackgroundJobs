using Pilgaard.RecurringJobs.Examples.MinimalAPI;
using Pilgaard.RecurringJobs.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add RecurringJobs to the container.
builder.Services.AddRecurringJobs(typeof(Program));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", WeatherForecastEndpoint.Get);

app.Run();
