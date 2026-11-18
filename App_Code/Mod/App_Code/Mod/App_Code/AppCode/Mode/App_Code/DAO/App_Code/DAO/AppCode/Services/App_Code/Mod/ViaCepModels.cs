using System.Text.Json.Serialization;

namespace MauiIBGE.App_Code.Mod
{
    public class ViaCepResponse
    {
        [JsonPropertyName("cep")]
        public string Cep { get; set; } = string.Empty;

        [JsonPropertyName("logradouro")]
        public string Logradouro { get; set; } = string.Empty;

        [JsonPropertyName("bairro")]
        public string Bairro { get; set; } = string.Empty;

        [JsonPropertyName("localidade")]
        public string Localidade { get; set; } = string.Empty;

        [JsonPropertyName("uf")]
        public string Uf { get; set; } = string.Empty;

        [JsonPropertyName("estado")]
        public string Estado { get; set; } = string.Empty;

        [JsonPropertyName("ddd")]
        public string Ddd { get; set; } = string.Empty;

        [JsonPropertyName("ibge")]
        public string Ibge { get; set; } = string.Empty;

        [JsonPropertyName("erro")]
        public bool Erro { get; set; }
    }
}
