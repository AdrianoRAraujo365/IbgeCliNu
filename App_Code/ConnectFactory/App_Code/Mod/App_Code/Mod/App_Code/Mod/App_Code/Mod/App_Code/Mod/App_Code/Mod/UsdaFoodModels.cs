// App_Code/Mod/UsdaFoodModels.cs
using System.Text.Json.Serialization;

namespace MauiIBGE.App_Code.Mod
{
    public class FoodSearchResponse
    {
        [JsonPropertyName("totalHits")]
        public int TotalHits { get; set; }

        [JsonPropertyName("foods")]
        public List<FoodItem> Foods { get; set; } = new();
    }

    public class FoodItem
    {
        [JsonPropertyName("fdcId")]
        public int FdcId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("foodCategory")]
        public string? FoodCategory { get; set; }

        [JsonPropertyName("foodNutrients")]
        public List<NutrientDetail> FoodNutrients { get; set; } = new();
    }

    public class NutrientDetail
    {
        [JsonPropertyName("nutrientName")]
        public string NutrientName { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("unitName")]
        public string UnitName { get; set; } = string.Empty;
    }
}
