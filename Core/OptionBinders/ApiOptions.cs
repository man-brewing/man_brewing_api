namespace Core.OptionBinders
{
    public class ApiOptions
    {
        public const string Api = nameof(Api);

        public string OpenWeatherApiKey { get; set; }
        public string OpenWeatherCityId { get; set; }
    }
}
