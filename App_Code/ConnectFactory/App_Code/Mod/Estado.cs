// App_Code/Mod/Estado.cs
using System.Text.Json.Serialization;

namespace MauiIBGE.App_Code.Mod
{
    public class RegiaoIbge
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("sigla")]
        public string Sigla { get; set; } = string.Empty;

        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;
    }

    public class Estado
    {
        [JsonPropertyName("id")]
        public int ESTINTID { get; set; }

        [JsonPropertyName("sigla")]
        public string ESTSTRSIGLA { get; set; } = string.Empty;

        [JsonPropertyName("nome")]
        public string ESTSTRDESCRICAO { get; set; } = string.Empty;

        [JsonPropertyName("regiao")]
        public RegiaoIbge? Regiao { get; set; }
    }
}
