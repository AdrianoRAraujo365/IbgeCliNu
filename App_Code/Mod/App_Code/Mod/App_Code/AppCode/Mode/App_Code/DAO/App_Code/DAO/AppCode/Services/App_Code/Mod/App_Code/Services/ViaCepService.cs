using MauiIBGE.App_Code.Mod;
using System.Net.Http.Json;

namespace MauiIBGE.App_Code.Services
{
    public class ViaCepService
    {
        private readonly HttpClient _httpClient;

        public ViaCepService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://viacep.com.br/ws/")
            };
        }

        public async Task<ViaCepResponse?> ObterEnderecoPorCepAsync(string cep)
        {
            var cepLimpo = new string(cep.Where(char.IsDigit).ToArray());
            if (cepLimpo.Length != 8) return null;

            var resultado = await _httpClient.GetFromJsonAsync<ViaCepResponse>($"{cepLimpo}/json/");
            return resultado?.Erro == true ? null : resultado;
        }
    }
}
