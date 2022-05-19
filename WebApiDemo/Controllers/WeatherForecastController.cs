using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static List<WeatherForecast> forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
            .ToList();

        private readonly IOptions<ApiBehaviorOptions> apiBehaviorOptions;

        public WeatherForecastController(IOptions<ApiBehaviorOptions> apiBehaviorOptions)
        {
            this.apiBehaviorOptions = apiBehaviorOptions ?? throw new ArgumentNullException(nameof(apiBehaviorOptions));
        }

        [HttpGet] 
        public IEnumerable<WeatherForecast> GetWeatherForecast()
        {
            return forecasts;
        } 

        [HttpGet]
        [Route("[action]")]
        public string GetSpecial()
        {
            return "special";

        }

        [HttpPost]
        public IActionResult CreateForecast(WeatherForecast weatherForecast)
        {
            if (forecasts.Any(x => x.Date == weatherForecast.Date))
            {
                ModelState.AddModelError(nameof(WeatherForecast.Date), "Already exists");
                var response = apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
                return response;
            }

            forecasts.Add(weatherForecast);
      
            return CreatedAtAction(nameof(CreateForecast), weatherForecast);
        }

        [HttpPut]
        public IActionResult Put(WeatherForecast weatherForecast)
        {
            var existingForecast = forecasts.Where(x => x.Date == weatherForecast.Date).FirstOrDefault();
            if (existingForecast == null) return NotFound(weatherForecast);

            existingForecast.Summary = weatherForecast.Summary;
            existingForecast.TemperatureC = weatherForecast.TemperatureC;
            return Ok(weatherForecast);
        }

        [HttpDelete]
        public IActionResult Delete(DateTime date)
        {
            var existingForecast = forecasts.Where(x => x.Date == date).FirstOrDefault();
            if (existingForecast == null) return NotFound(date);

            forecasts.Remove(existingForecast);
            return Ok(date);
        }

    }
}
