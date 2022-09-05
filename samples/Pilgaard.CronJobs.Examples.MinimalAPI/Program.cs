using Pilgaard.CronJobs.Examples.MinimalAPI;
using Pilgaard.CronJobs.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add CronJobs to the container.
builder.Services.AddCronJobs(typeof(Program));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", WeatherForecasterEndpoint.Get);

app.Run();