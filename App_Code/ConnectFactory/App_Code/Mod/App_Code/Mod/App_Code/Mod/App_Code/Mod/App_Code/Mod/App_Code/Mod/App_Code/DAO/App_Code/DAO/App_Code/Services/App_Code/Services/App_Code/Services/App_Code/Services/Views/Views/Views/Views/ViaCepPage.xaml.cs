using MauiIBGE.App_Code.Mod;
using MauiIBGE.App_Code.Services;

namespace MauiIBGE.Views
{
    public partial class ViaCepPage : ContentPage
    {
        private readonly ViaCepService _viaCepService;

        public ViaCepPage()
        {
            InitializeComponent();
            _viaCepService = new ViaCepService();
        }

        #region Form
        public ViaCepResponse GetForm()
        {
            return new ViaCepResponse
            {
                Cep = CepEntry.Text?.Trim() ?? string.Empty,
                Logradouro = LogradouroEntry.Text ?? string.Empty,
                Bairro = BairroEntry.Text ?? string.Empty,
                Localidade = CidadeEntry.Text ?? string.Empty,
                Uf = UfEntry.Text ?? string.Empty,
                Regiao = RegiaoEntry.Text ?? string.Empty,
                Ibge = IbgeEntry.Text ?? string.Empty
            };
        }

        public void SetForm(ViaCepResponse endereco)
        {
            CepEntry.Text = endereco.Cep;
            LogradouroEntry.Text = endereco.Logradouro;
            BairroEntry.Text = endereco.Bairro;
            CidadeEntry.Text = endereco.Localidade;
            UfEntry.Text = endereco.Uf;
            RegiaoEntry.Text = endereco.Regiao;
            IbgeEntry.Text = endereco.Ibge;
        }

        public void ClearForm()
        {
            CepEntry.Text = string.Empty;
            LogradouroEntry.Text = string.Empty;
            BairroEntry.Text = string.Empty;
            CidadeEntry.Text = string.Empty;
            UfEntry.Text = string.Empty;
            RegiaoEntry.Text = string.Empty;
            IbgeEntry.Text = string.Empty;
        }

        public async Task<bool> ValidaForm()
        {
            var digitos = new string((CepEntry.Text ?? "").Where(char.IsDigit).ToArray());
            if (digitos.Length != 8)
            {
                await DisplayAlertAsync("Atenção", "Informe um CEP válido com 8 dígitos.", "OK");
                CepEntry.Focus();
                return false;
            }
            return true;
        }
        #endregion

        private async void BuscarCepButton_Clicked(object? sender, EventArgs e)
        {
            if (!await ValidaForm()) return;

            try
            {
                BuscarCepButton.IsEnabled = false;
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var endereco = await _viaCepService.ObterEnderecoPorCepAsync(CepEntry.Text!);
                if (endereco == null)
                {
                    ClearForm();
                    await DisplayAlertAsync("Não encontrado", "CEP inexistente na base ViaCEP.", "OK");
                    return;
                }

                SetForm(endereco);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Erro", $"Falha ao consultar ViaCEP: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
                BuscarCepButton.IsEnabled = true;
            }
        }

        private async void EnviarParaClimaButton_Clicked(object? sender, EventArgs e)
        {
            var dados = GetForm();
            if (string.IsNullOrWhiteSpace(dados.Localidade) || string.IsNullOrWhiteSpace(dados.Uf))
            {
                await DisplayAlertAsync("Atenção", "Consulte um CEP válido antes de enviar.", "OK");
                return;
            }

            var parametros = new Dictionary<string, object>
            {
                ["Cep"] = dados.Cep,
                ["Logradouro"] = dados.Logradouro,
                ["Cidade"] = dados.Localidade,
                ["Uf"] = dados.Uf
            };

            await Shell.Current.GoToAsync("//MainPage", parametros);
        }

        private void LimparButton_Clicked(object? sender, EventArgs e)
        {
            ClearForm();
        }
    }
}
