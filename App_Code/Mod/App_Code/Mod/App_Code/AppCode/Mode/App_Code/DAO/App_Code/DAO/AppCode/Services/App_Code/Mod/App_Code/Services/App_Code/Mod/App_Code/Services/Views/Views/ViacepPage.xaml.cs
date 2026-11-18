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

        private async void BuscarCepButton_Clicked(object sender, EventArgs e)
        {
            string cep = CepEntry.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(cep))
            {
                await DisplayAlertAsync("Atenção", "Digite um CEP com 8 dígitos.", "OK");
                return;
            }

            try
            {
                BuscarCepButton.IsEnabled = false;
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var endereco = await _viaCepService.ObterEnderecoPorCepAsync(cep);
                if (endereco == null)
                {
                    LogradouroEntry.Text = BairroEntry.Text = CidadeEntry.Text = EstadoEntry.Text = DddEntry.Text = IbgeEntry.Text = string.Empty;
                    await DisplayAlertAsync("Não encontrado", "CEP inválido ou inexistente.", "OK");
                    return;
                }

                LogradouroEntry.Text = endereco.Logradouro;
                BairroEntry.Text = endereco.Bairro;
                CidadeEntry.Text = endereco.Localidade;
                EstadoEntry.Text = string.IsNullOrWhiteSpace(endereco.Estado) ? endereco.Uf : $"{endereco.Estado} ({endereco.Uf})";
                DddEntry.Text = endereco.Ddd;
                IbgeEntry.Text = endereco.Ibge;
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
    }
}
