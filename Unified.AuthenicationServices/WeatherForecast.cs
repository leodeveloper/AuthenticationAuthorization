using System;

namespace Unified.AuthenicationServices
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
        public bool isAuthenticated { get; set; }

        public string token { get; set; }
    }
}
