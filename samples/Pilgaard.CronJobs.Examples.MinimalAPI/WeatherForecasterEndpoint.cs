namespace Pilgaard.CronJobs.Examples.MinimalAPI;

public class WeatherForecasterEndpoint
{
    public static IEnumerable<WeatherForecast> Get(ILogger<WeatherForecasterEndpoint> logger)
    {
        logger.LogInformation("Received request at {timeNow}", DateTime.UtcNow.ToString("T"));

        foreach (var index in Enumerable.Range(1, 5))
        {
            yield return new WeatherForecast
            (
                DateTime.Now.AddDays(index),
                Random.Shared.Next(-20, 55),
                Summaries[Random.Shared.Next(Summaries.Length)]
            );
        }
    }

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}