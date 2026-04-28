using Microsoft.Extensions.Logging.Abstractions;
using WebAPI_WebApp.Controllers;

namespace WebAPI_WebApp.Tests;

public class WeatherForecastControllerTests
{
    [Fact]
    public void Get_ReturnsFiveForecasts()
    {
        var controller = new WeatherForecastController(NullLogger<WeatherForecastController>.Instance);

        var result = controller.Get().ToArray();

        Assert.Equal(5, result.Length);
        Assert.All(result, x => Assert.InRange(x.TemperatureC, -20, 55));
    }
}
