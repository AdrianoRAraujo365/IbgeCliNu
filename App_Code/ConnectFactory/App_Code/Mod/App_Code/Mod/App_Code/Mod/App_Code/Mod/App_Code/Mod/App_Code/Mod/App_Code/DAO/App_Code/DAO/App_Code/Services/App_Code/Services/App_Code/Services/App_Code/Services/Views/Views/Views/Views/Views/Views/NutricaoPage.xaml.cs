using MauiIBGE.App_Code.Services;

namespace MauiIBGE.Views
{
    [QueryProperty(nameof(TermoInicial), "termo")]
    public partial class NutricaoPage : ContentPage
    {
        private readonly FoodNutritionService _foodService;

        public string TermoInicial
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    AlimentoSearchBar.Text = Uri.UnescapeDataString(value);
                    _ = ExecutarBuscaAsync(AlimentoSearchBar.Text);
                }
            }
        }

        public NutricaoPage()
        {
            InitializeComponent();
            _foodService = new FoodNutritionService();
        }

        private async void AlimentoSearchBar_SearchButtonPressed(object? sender, EventArgs e)
        {
            string termo = AlimentoSearchBar.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(termo))
            {
                await DisplayAlertAsync("Atenção", "Digite o nome de um alimento.", "OK");
                return;
            }

            await ExecutarBuscaAsync(termo);
        }

        private async Task ExecutarBuscaAsync(string termo)
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                AlimentosCollectionView.ItemsSource = null;

                var resultados = await _foodService.PesquisarAlimentoAsync(termo, limite: 10);
                AlimentosCollectionView.ItemsSource = resultados;

                if (resultados.Count == 0)
                {
                    await DisplayAlertAsync("Sem resultados", "Nenhum alimento encontrado na base USDA.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Erro USDA API", $"Falha ao buscar nutrientes: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }
    }
}
