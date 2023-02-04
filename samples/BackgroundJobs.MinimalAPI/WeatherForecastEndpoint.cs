namespace Pilgaard.BackgroundJobs.Examples.MinimalAPI;

public class WeatherForecastEndpoint
{
    public static IEnumerable<WeatherForecast> Get(ILogger<WeatherForecastEndpoint> logger)
    {
        logger.LogInformation("Received request at {timeNow}", DateTime.UtcNow.ToString("T"));

        foreach (int index in Enumerable.Range(1, 5))
        {
            yield return new WeatherForecast
            (
                DateTime.Now.AddDays(index),
                Random.Shared.Next(-20, 55),
                _summaries[Random.Shared.Next(_summaries.Length)]
            );
        }
    }

    private static readonly string[] _summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
