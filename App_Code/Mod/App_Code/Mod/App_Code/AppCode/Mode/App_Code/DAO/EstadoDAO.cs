using MauiIBGE.App_Code.Mod;
using System.Net.Http.Json;

namespace MauiIBGE.App_Code.DAO
{
    public class EstadoDAO
    {
        private readonly ConnectFactory.Services _services;

        public EstadoDAO(ConnectFactory.Services services)
        {
            _services = services;
        }

        public async Task<List<Estado>> ListarEstadoAsync()
        {
            const string url = "https://servicodados.ibge.gov.br/api/v1/localidades/estados?orderBy=nome";
            var estados = await _services.Client.GetFromJsonAsync<List<Estado>>(url);
            return estados ?? new List<Estado>();
        }
    }
}
