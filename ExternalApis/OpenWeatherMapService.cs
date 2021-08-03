using System;
using System.Net.Http;
using System.Threading.Tasks;
using Core.OptionBinders;
using ExternalApis.Models;
using Microsoft.Extensions.Configuration;

namespace ExternalApis
{
    public class OpenWeatherMapService : IOpenWeatherMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _openWeatherApiKey;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public OpenWeatherMapService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            var apiOptions = configuration.GetSection(ApiOptions.Api).Get<ApiOptions>();
            _openWeatherApiKey = apiOptions.OpenWeatherApiKey;
        }

        /// <inheritdoc />
        public async Task<OpenWeatherMapResponse> GetCurrentWeather(string cityId)
        {
            var httpResponse = await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?id={cityId}&appid={_openWeatherApiKey}&units=metric");

            if (httpResponse.IsSuccessStatusCode)
            {
                return await httpResponse.Content.ReadAsAsync<OpenWeatherMapResponse>();
            }

            throw new Exception($"Unable to fetch current weather data. Response code: {httpResponse.StatusCode}");
        }
    }
}
