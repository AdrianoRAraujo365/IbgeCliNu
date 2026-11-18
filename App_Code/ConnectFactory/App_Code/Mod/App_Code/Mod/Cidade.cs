// App_Code/Mod/Cidade.cs
using System.Text.Json.Serialization;

namespace MauiIBGE.App_Code.Mod
{
    public class Cidade
    {
        [JsonPropertyName("id")]
        public int CIDINTID { get; set; }

        [JsonPropertyName("nome")]
        public string CIDSTRDESCRICAO { get; set; } = string.Empty;
    }
}
