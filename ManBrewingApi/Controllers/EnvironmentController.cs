using System;
using System.Threading.Tasks;
using ExternalApis;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Gets the <see cref="EnvironmentLog"/> by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var result = _dataLogService.Get(id);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message) {StatusCode = StatusCodes.Status500InternalServerError};
            }
        }

        /// <summary>
        /// Gets the most recent <see cref="EnvironmentLog"/> record.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var result = _dataLogService.GetMostRecent();
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        /// <summary>
        /// Gets the most recent 5 <see cref="EnvironmentLog"/> records.
        /// </summary>
        /// <returns></returns>
        [HttpGet("history")]
        public IActionResult GetHistory()
        {
            return Redirect("history/5");
        }

        /// <summary>
        /// Gets the most recent count of <see cref="EnvironmentLog"/> records.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet("history/{count}")]
        public IActionResult GetHistory(int count)
        {
            try
            {
                var result = _dataLogService.GetLast(count);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        /// <summary>
        /// Gets the <see cref="EnvironmentLog"/> records between the specified dates.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet("history/{start}/{end}")]
        public IActionResult GetHistory(DateTimeOffset start, DateTimeOffset end)
        {
            try
            {
                var result = _dataLogService.GetBetweenDates(start, end);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        /// <summary>
        /// Saves the environment data to the database.
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="environmentLoggerData"></param>
        /// <returns></returns>
        [HttpPost("{authToken}")]
        public async Task<IActionResult> Save(string authToken, [FromForm] EnvironmentLoggerData environmentLoggerData)
        {
            if (authToken != _configuration["AuthToken"])
            {
                return Unauthorized();
            }

            try
            {
                var cityId = _configuration["OpenWeatherCityId"];

                var weatherData = await _openWeatherMapService.GetCurrentWeather(cityId);
                var dataLog = new EnvironmentLog
                {
                    WeatherHumidityPercent = weatherData.main.humidity,
                    WeatherTemperatureC = weatherData.main.temp,
                    AmbientHumidityPercent = environmentLoggerData.humidity,
                    AmbientTemperatureC = environmentLoggerData.temp
                };

                var result = _dataLogService.Save(dataLog);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        /// <summary>
        /// Model for data received from the environment logger box.
        /// </summary>
        public class EnvironmentLoggerData
        {
            public double temp { get; set; }
            public double humidity { get; set; }
        }
    }
}
