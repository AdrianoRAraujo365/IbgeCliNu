using System.Text.Json.Serialization;

namespace MauiIBGE.App_Code.Mod
{
    public class ClimaInfo
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double TemperaturaAtual { get; set; }
        public double Media { get; set; }
        public double Maxima { get; set; }
        public double Minima { get; set; }
        public int Umidade { get; set; }
        public double PressaoAtmosferica { get; set; }
        public List<string> Dias { get; set; } = new();
        public List<double> TemperaturasMaximas { get; set; } = new();
        public List<double> TemperaturasMinimas { get; set; } = new();
    }

    public class GeocodingResponse
    {
        [JsonPropertyName("results")]
        public List<GeoLocation>? Results { get; set; }
    }

    public class GeoLocation
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("country_code")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("admin1")]
        public string? Admin1 { get; set; }
    }

    public class WeatherResponse
    {
        [JsonPropertyName("current")]
        public CurrentWeather? Current { get; set; }

        [JsonPropertyName("daily")]
        public DailyWeather? Daily { get; set; }
    }

    public class CurrentWeather
    {
        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public int Humidity { get; set; }

        [JsonPropertyName("surface_pressure")]
        public double SurfacePressure { get; set; }
    }

    public class DailyWeather
    {
        [JsonPropertyName("time")]
        public List<string>? Time { get; set; }

        [JsonPropertyName("temperature_2m_max")]
        public List<double>? TemperatureMax { get; set; }

        [JsonPropertyName("temperature_2m_min")]
        public List<double>? TemperatureMin { get; set; }

        [JsonPropertyName("temperature_2m_mean")]
        public List<double>? TemperatureMean { get; set; }
    }
}
