using System;
using System.Threading.Tasks;
using Core.OptionBinders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repository;
using Repository.Models;

namespace ManBrewingApi.Controllers
{
    [Route("beerroom/[controller]")]
    [ApiController]
    public class EnvironmentController : Controller
    {
        private readonly IDataLogService _dataLogService;
        private readonly IOpenWeatherMapService _openWeatherMapService;
        private readonly IConfiguration _configuration;

        public EnvironmentController(IConfiguration configuration, IDataLogService dataLogService, IOpenWeatherMapService openWeatherMapService)
        {
            _dataLogService = dataLogService;
            _openWeatherMapService = openWeatherMapService;
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            var result = _dataLogService.Get(id);
            return new JsonResult(result);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = _dataLogService.GetMostRecent();
            return new JsonResult(result);
        }

        [HttpGet("history")]
        public IActionResult GetHistory()
        {
            return Redirect("history/5");
        }

        [HttpGet("history/{count}")]
        public IActionResult GetHistory(int count)
        {
            var result = _dataLogService.GetLast(count);
            return new JsonResult(result);
        }

        [HttpGet("history/{start}/{end}")]
        public IActionResult GetHistory(DateTime start, DateTime end)
        {
            var result = _dataLogService.GetBetweenDates(start, end);
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromForm] EnvironmentData environmentData)
        {
            var cityId = _configuration.GetSection(ApiOptions.Api).Get<ApiOptions>().OpenWeatherCityId;

            var weatherData = await _openWeatherMapService.GetCurrentWeather(cityId);
            var dataLog = new DataLog
            {
                Ambient_Humid = weatherData.main.humidity, 
                Ambient_Temp = weatherData.main.temp,
                Timestamp = DateTime.Now,
                Humidity = environmentData.humidity,
                Temperature = environmentData.temp
            };

            var result = _dataLogService.Save(dataLog);
            return new JsonResult(result);
        }

        public class EnvironmentData
        {
            public double temp { get; set; }
            public double humidity { get; set; }
        }
    }
}
