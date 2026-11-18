using MauiIBGE.App_Code.Mod;
using MauiIBGE.App_Code.Services;
using System.Globalization;
using System.Text.Json;

namespace MauiIBGE.Views
{
    public partial class ClimaMapaPage : ContentPage, IQueryAttributable
    {
        private readonly AgroNutricaoService _agroService;
        private readonly FoodNutritionService _foodService;
        private AgroPerfilLocal? _perfilAtual;

        public ClimaMapaPage()
        {
            InitializeComponent();
            _agroService = new AgroNutricaoService();
            _foodService = new FoodNutritionService();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("FormDados", out var formObj) && formObj is ConsultaFormModel form &&
                query.TryGetValue("ClimaInfo", out var climaObj) && climaObj is ClimaInfo clima)
            {
                PreencherDadosTela(form, clima);
            }
        }

        private async void PreencherDadosTela(ConsultaFormModel form, ClimaInfo clima)
        {
            var cultura = CultureInfo.GetCultureInfo("pt-BR");

            LocalidadeHeaderLabel.Text = $"{form.CidadeNome} - {form.EstadoSigla}";
            RegiaoHeaderLabel.Text = $"Região: {form.RegiaoNome}";
            LogradouroHeaderLabel.Text = string.IsNullOrWhiteSpace(form.Logradouro)
                ? "Endereço geral do município"
                : $"Logradouro: {form.Logradouro}";

            // Aba 1: Clima completo
            MediaLabel.Text = $"{clima.Media.ToString("F1", cultura)} °C";
            MaximaLabel.Text = $"{clima.Maxima.ToString("F1", cultura)} °C";
            MinimaLabel.Text = $"{clima.Minima.ToString("F1", cultura)} °C";
            UmidadeLabel.Text = $"{clima.Umidade}%";
            PressaoLabel.Text = $"{clima.PressaoAtmosferica.ToString("F1", cultura)} hPa";

            RenderizarMapaCalor(clima, form.CidadeNome, form.RegiaoNome);

            // Aba 2: Agronegócio da cidade/estado/região
            _perfilAtual = _agroService.ObterPerfilPorLocalidade(form.CidadeNome, form.EstadoSigla, form.RegiaoNome);
            RegiaoAgroLabel.Text = $"Região {form.RegiaoNome} • Estado de {form.EstadoNome}";
            TipoAgroLabel.Text = _perfilAtual.TipoAgronegocio;
            CulturasAgroLabel.Text = _perfilAtual.PrincipaisCulturas;
            ClimaAgroResumoLabel.Text = $"Média: {clima.Media:F1} °C | Máx: {clima.Maxima:F1} °C | Mín: {clima.Minima:F1} °C";

            // Aba 3: Produto Nutricional mais vendido via USDA FoodData Central
            ProdutoDestaqueLabel.Text = _perfilAtual.ProdutoMaisVendidoPt;
            await CarregarNutricaoUsdaAsync(_perfilAtual.TermoBuscaUsdaEn);
        }

        private async Task CarregarNutricaoUsdaAsync(string termoUsda)
        {
            try
            {
                NutriLoadingIndicator.IsVisible = true;
                NutriLoadingIndicator.IsRunning = true;
                ListaNutrientesUsdaLayout.Children.Clear();

                var alimentos = await _foodService.PesquisarAlimentoAsync(termoUsda, limite: 3);

                foreach (var alimento in alimentos)
                {
                    var stackInterno = new VerticalStackLayout { Spacing = 4 };
                    stackInterno.Children.Add(new Label
                    {
                        Text = alimento.Description,
                        FontSize = 15,
                        FontAttributes = FontAttributes.Bold
                    });
                    stackInterno.Children.Add(new Label
                    {
                        Text = $"USDA FDC ID: {alimento.FdcId}",
                        FontSize = 11,
                        TextColor = Colors.Gray
                    });

                    foreach (var nut in alimento.FoodNutrients)
                    {
                        stackInterno.Children.Add(new Label
                        {
                            Text = $"• {nut.NutrientName}: {nut.Value:F1} {nut.UnitName}",
                            FontSize = 13
                        });
                    }

                    ListaNutrientesUsdaLayout.Children.Add(new Border
                    {
                        Padding = 12,
                        Stroke = Color.FromArgb("#E2E8F0"),
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                        Content = stackInterno
                    });
                }
            }
            catch (Exception ex)
            {
                ListaNutrientesUsdaLayout.Children.Add(new Label
                {
                    Text = $"Não foi possível consultar a API USDA no momento: {ex.Message}",
                    TextColor = Colors.Crimson
                });
            }
            finally
            {
                NutriLoadingIndicator.IsRunning = false;
                NutriLoadingIndicator.IsVisible = false;
            }
        }

        private void RenderizarMapaCalor(ClimaInfo clima, string cidade, string regiao)
        {
            string lat = clima.Latitude.ToString(CultureInfo.InvariantCulture);
            string lon = clima.Longitude.ToString(CultureInfo.InvariantCulture);

            double intensidade = Math.Clamp((clima.Maxima - 10.0) / 30.0, 0.30, 1.0);
            string intAlta = intensidade.ToString("F2", CultureInfo.InvariantCulture);
            string intMedia = (intensidade * 0.75).ToString("F2", CultureInfo.InvariantCulture);

            string diasJson = JsonSerializer.Serialize(clima.Dias);
            string maxJson = JsonSerializer.Serialize(clima.TemperaturasMaximas);
            string minJson = JsonSerializer.Serialize(clima.TemperaturasMinimas);

            string html = $@"<!DOCTYPE html>
            <html lang='pt-br'>
            <head>
                <meta charset='utf-8' />
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />
                <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
                <script src='https://unpkg.com/leaflet.heat@0.2.0/dist/leaflet-heat.js'></script>
                <script src='https://cdn.jsdelivr.net/npm/chart.js@3.7.0/dist/chart.min.js'></script>
                <style>
                    body {{ font-family: sans-serif; margin: 0; padding: 6px; display: flex; flex-direction: column; gap: 8px; height: 100vh; box-sizing: border-box; }}
                    #heatmap {{ height: 55%; width: 100%; border-radius: 8px; }}
                    .chart-box {{ height: 41%; width: 100%; }}
                </style>
            </head>
            <body>
                <div id='heatmap'></div>
                <div class='chart-box'><canvas id='tempChart'></canvas></div>
                <script>
                    var lat = {lat};
                    var lon = {lon};
                    var map = L.map('heatmap').setView([lat, lon], 11);

                    L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                        maxZoom: 18,
                        attribution: '&copy; OpenStreetMap'
                    }}).addTo(map);

                    var pontosCalor = [
                        [lat, lon, {intAlta}],
                        [lat + 0.015, lon + 0.015, {intAlta}],
                        [lat - 0.015, lon - 0.012, {intMedia}],
                        [lat + 0.012, lon - 0.018, {intMedia}],
                        [lat - 0.018, lon + 0.014, {intAlta}]
                    ];

                    L.heatLayer(pontosCalor, {{
                        radius: 40,
                        blur: 24,
                        maxZoom: 14,
                        gradient: {{ 0.2: '#3b82f6', 0.5: '#22c55e', 0.75: '#f59e0b', 1.0: '#ef4444' }}
                    }}).addTo(map);

                    L.marker([lat, lon]).addTo(map)
                        .bindPopup('<b>{cidade} (Região {regiao})</b><br/>Média: {clima.Media:F1} °C | Umidade: {clima.Umidade}%<br/>Pressão: {clima.PressaoAtmosferica:F1} hPa')
                        .openPopup();

                    var ctx = document.getElementById('tempChart').getContext('2d');
                    new Chart(ctx, {{
                        type: 'line',
                        data: {{
                            labels: {diasJson},
                            datasets: [
                                {{ label: 'Máxima (°C)', data: {maxJson}, borderColor: '#ef4444', backgroundColor: 'rgba(239,68,68,0.15)', fill: true, tension: 0.3 }},
                                {{ label: 'Mínima (°C)', data: {minJson}, borderColor: '#3b82f6', backgroundColor: 'rgba(59,130,246,0.15)', fill: true, tension: 0.3 }}
                            ]
                        }},
                        options: {{ responsive: true, maintainAspectRatio: false }}
                    }});
                </script>
            </body>
            </html>";

            MapaCalorWebView.Source = new HtmlWebViewSource { Html = html };
        }

        private void AlternarAba(int indiceAba)
        {
            AbaClimaLayout.IsVisible = indiceAba == 0;
            AbaAgroLayout.IsVisible = indiceAba == 1;
            AbaNutriLayout.IsVisible = indiceAba == 2;

            TabClimaBtn.BackgroundColor = indiceAba == 0 ? Color.FromArgb("#1565C0") : Color.FromArgb("#E2E8F0");
            TabClimaBtn.TextColor = indiceAba == 0 ? Colors.White : Color.FromArgb("#1E293B");

            TabAgroBtn.BackgroundColor = indiceAba == 1 ? Color.FromArgb("#1565C0") : Color.FromArgb("#E2E8F0");
            TabAgroBtn.TextColor = indiceAba == 1 ? Colors.White : Color.FromArgb("#1E293B");

            TabNutriBtn.BackgroundColor = indiceAba == 2 ? Color.FromArgb("#1565C0") : Color.FromArgb("#E2E8F0");
            TabNutriBtn.TextColor = indiceAba == 2 ? Colors.White : Color.FromArgb("#1E293B");
        }

        private void TabClimaBtn_Clicked(object? sender, EventArgs e) => AlternarAba(0);
        private void TabAgroBtn_Clicked(object? sender, EventArgs e) => AlternarAba(1);
        private void TabNutriBtn_Clicked(object? sender, EventArgs e) => AlternarAba(2);

        private async void AbrirTelaNutricao_Clicked(object? sender, EventArgs e)
        {
            string termo = _perfilAtual?.TermoBuscaUsdaEn ?? "orange juice";
            await Shell.Current.GoToAsync($"//NutricaoPage?termo={Uri.EscapeDataString(termo)}");
        }

        private async void VoltarButton_Clicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
