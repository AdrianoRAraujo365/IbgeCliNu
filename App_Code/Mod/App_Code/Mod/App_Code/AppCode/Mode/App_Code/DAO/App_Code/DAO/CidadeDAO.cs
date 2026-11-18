using MauiIBGE.App_Code.Mod;
using System.Net.Http.Json;

namespace MauiIBGE.App_Code.DAO
{
    public class CidadeDAO
    {
        private readonly ConnectFactory.Services _services;

        public CidadeDAO(ConnectFactory.Services services)
        {
            _services = services;
        }

        public async Task<List<Cidade>> ListarCidadeAsync(string uf)
        {
            string url = $"https://servicodados.ibge.gov.br/api/v1/localidades/estados/{uf}/municipios?orderBy=nome";
            var cidades = await _services.Client.GetFromJsonAsync<List<Cidade>>(url);
            return cidades ?? new List<Cidade>();
        }
    }
}
