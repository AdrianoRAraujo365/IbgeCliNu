using System.Text.Json.Serialization;

namespace MauiIBGE.App_Code.Mod
{
    public class Estado
    {
        [JsonPropertyName("id")]
        public int ESTINTID { get; set; }

        [JsonPropertyName("sigla")]
        public string ESTSTRSIGLA { get; set; } = string.Empty;

        [JsonPropertyName("nome")]
        public string ESTSTRDESCRICAO { get; set; } = string.Empty;
    }
}
