using System.Threading.Tasks;
using ExternalApis.Models;

namespace Repository
{
    public interface IOpenWeatherMapService
    {
        /// <summary>
        /// Gets the current weather data for the specified city.
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public Task<OpenWeatherMapResponse> GetCurrentWeather(string cityId);
    }
}
