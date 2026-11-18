using MauiIBGE.App_Code.Mod;
using System.Globalization;
using System.Net.Http.Json;

namespace MauiIBGE.App_Code.Services
{
    public class ClimaService
    {
        private readonly HttpClient _httpClient;

        public ClimaService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ClimaInfo> ConsultarAsync(string nomeCidade, string nomeEstado)
        {
            // 1. Geocodificação da cidade filtrando pelo Brasil e Estado
            string cidadeEncoded = Uri.EscapeDataString(nomeCidade);
            string geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={cidadeEncoded}&count=10&language=pt&format=json";

            var geoResponse = await _httpClient.GetFromJsonAsync<GeocodingResponse>(geoUrl);
            var local = geoResponse?.Results?
                .FirstOrDefault(r => string.Equals(r.CountryCode, "BR", StringComparison.OrdinalIgnoreCase) &&
                                     string.Equals(r.Admin1, nomeEstado, StringComparison.OrdinalIgnoreCase))
                ?? geoResponse?.Results?
                .FirstOrDefault(r => string.Equals(r.CountryCode, "BR", StringComparison.OrdinalIgnoreCase))
                ?? geoResponse?.Results?.FirstOrDefault();

            if (local == null)
            {
                throw new Exception("Coordenadas geográficas da cidade não encontradas.");
            }

            // 2. Consulta climática: temperatura (média, máxima, mínima), umidade e pressão atmosférica
            string lat = local.Latitude.ToString(CultureInfo.InvariantCulture);
            string lon = local.Longitude.ToString(CultureInfo.InvariantCulture);

            string weatherUrl =
                $"https://api.open-meteo.com/v1/forecast" +
                $"?latitude={lat}" +
                $"&longitude={lon}" +
                $"&current=temperature_2m,relative_humidity_2m,surface_pressure" +
                $"&daily=temperature_2m_max,temperature_2m_min,temperature_2m_mean" +
                $"&forecast_days=5" +
                $"&timezone=auto";

            var weather = await _httpClient.GetFromJsonAsync<WeatherResponse>(weatherUrl);

            if (weather?.Current == null || weather.Daily == null)
            {
                throw new Exception("Dados meteorológicos inválidos retornados pela API.");
            }

            double maximaHoje = weather.Daily.TemperatureMax?.FirstOrDefault() ?? weather.Current.Temperature;
            double minimaHoje = weather.Daily.TemperatureMin?.FirstOrDefault() ?? weather.Current.Temperature;
            double mediaHoje = weather.Daily.TemperatureMean?.FirstOrDefault() ?? ((maximaHoje + minimaHoje) / 2.0);

            return new ClimaInfo
            {
                Latitude = local.Latitude,
                Longitude = local.Longitude,
                TemperaturaAtual = weather.Current.Temperature,
                Media = mediaHoje,
                Maxima = maximaHoje,
                Minima = minimaHoje,
                Umidade = weather.Current.Humidity,
                PressaoAtmosferica = weather.Current.SurfacePressure,
                Dias = weather.Daily.Time ?? new List<string>(),
                TemperaturasMaximas = weather.Daily.TemperatureMax ?? new List<double>(),
                TemperaturasMinimas = weather.Daily.TemperatureMin ?? new List<double>()
            };
        }
    }
}
