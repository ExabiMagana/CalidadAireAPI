
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace CalidadAireAPI.Services
{
    public class ClimaService
    {
        private readonly HttpClient _httpClient;

        public ClimaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(double TemperaturaC, double HumedadPorcentaje, DateTime FechaHoraClima)?> ObtenerClimaAsync(
            string ubicacion,
            CancellationToken cancellationToken)
        {
            var ciudad = Uri.EscapeDataString(ubicacion);

            var urlGeocodificacion =
                $"https://geocoding-api.open-meteo.com/v1/search?name={ciudad}&count=1&language=es&format=json";

            var resultado = await _httpClient.GetFromJsonAsync<RespuestaGeocodificacion>(
                urlGeocodificacion, cancellationToken);

            var coordenadas = resultado?.Results?.FirstOrDefault();

            if (coordenadas == null)
                return null;

            var latitud = coordenadas.Latitude.ToString(CultureInfo.InvariantCulture);
            var longitud = coordenadas.Longitude.ToString(CultureInfo.InvariantCulture);

            var urlClima =
                $"https://api.open-meteo.com/v1/forecast?latitude={latitud}&longitude={longitud}&current=temperature_2m,relative_humidity_2m&timezone=auto";

            var clima = await _httpClient.GetFromJsonAsync<RespuestaClima>(
                urlClima, cancellationToken);

            if (clima?.Current == null ||
                clima.Current.Temperature == null ||
                clima.Current.Humidity == null)
            {
                return null;
            }

            return (
                clima.Current.Temperature.Value,
                clima.Current.Humidity.Value,
                clima.Current.Time
            );
        }

        private class RespuestaGeocodificacion
        {
            [JsonPropertyName("results")]
            public List<Coordenadas>? Results { get; set; }
        }

        private class Coordenadas
        {
            [JsonPropertyName("latitude")]
            public double Latitude { get; set; }

            [JsonPropertyName("longitude")]
            public double Longitude { get; set; }
        }

        private class RespuestaClima
        {
            [JsonPropertyName("current")]
            public ClimaActual? Current { get; set; }
        }

        private class ClimaActual
        {
            [JsonPropertyName("temperature_2m")]
            public double? Temperature { get; set; }

            [JsonPropertyName("relative_humidity_2m")]
            public double? Humidity { get; set; }

            [JsonPropertyName("time")]
            public DateTime Time { get; set; }
        }
    }
}
