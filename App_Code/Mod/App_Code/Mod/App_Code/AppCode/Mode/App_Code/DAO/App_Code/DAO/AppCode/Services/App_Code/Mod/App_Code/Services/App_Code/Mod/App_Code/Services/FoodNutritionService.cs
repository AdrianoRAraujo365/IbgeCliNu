using MauiIBGE.App_Code.Mod;
using System.Net.Http.Json;

namespace MauiIBGE.App_Code.Services
{
    public class FoodNutritionService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "DEMO_KEY";

        public FoodNutritionService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.nal.usda.gov/fdc/v1/")
            };
        }

        public async Task<List<FoodItem>> PesquisarAlimentoAsync(string termo, int limite = 10)
        {
            string url = $"foods/search?query={Uri.EscapeDataString(termo)}&pageSize={limite}&api_key={ApiKey}";
            var resultado = await _httpClient.GetFromJsonAsync<FoodSearchResponse>(url);

            if (resultado?.Foods == null) return new List<FoodItem>();

            string[] filtros = { "Energy", "Protein", "Total lipid (fat)", "Carbohydrate, by difference", "Fiber, total dietary" };
            foreach (var item in resultado.Foods)
            {
                item.FoodNutrients = item.FoodNutrients
                    .Where(n => filtros.Contains(n.NutrientName))
                    .ToList();
            }

            return resultado.Foods;
        }
    }
}
