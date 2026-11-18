using MauiIBGE.App_Code.ConnectFactory;
using MauiIBGE.App_Code.DAO;
using MauiIBGE.App_Code.Mod;
using MauiIBGE.App_Code.Services;
using System.Globalization;
using System.Text.Json;

namespace MauiIBGE
{
    public partial class MainPage : ContentPage
    {
        #region AtributoForm/Object
        private readonly EstadoDAO _estadoDAO;
        private readonly CidadeDAO _cidadeDAO;
        private readonly ClimaService _climaService;
        #endregion

        public MainPage()
        {
            InitializeComponent();
            var services = new Services();
            _estadoDAO = new EstadoDAO(services);
            _cidadeDAO = new CidadeDAO(services);
            _climaService = new ClimaService();
            CarregarEstados();
        }

        private async void CarregarEstados()
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var estados = await _estadoDAO.ListarEstadoAsync();
                EstadoPicker.ItemsSource = estados;
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Erro", $"Não foi possível carregar os estados.\n\n{ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        private void EstadoPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EstadoPicker.SelectedItem is not Estado estado)
            {
                return;
            }

            _ = CarregaCidade(estado);
        }

        private async Task CarregaCidade(Estado estado)
        {
            try
            {
                CidadePicker.ItemsSource = null;
                CidadePicker.SelectedItem = null;
                ResultadoLayout.IsVisible = false;
                CidadePicker.IsEnabled = false;

                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var cidades = await _cidadeDAO.ListarCidadeAsync(estado.ESTSTRSIGLA);

                CidadePicker.ItemsSource = cidades;
                CidadePicker.IsEnabled = true;
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Erro", $"Não foi possível carregar as cidades.\n\n{ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        private async void ConsultarButton_Clicked(object sender, EventArgs e)
        {
            if (EstadoPicker.SelectedItem is not Estado estado ||
                CidadePicker.SelectedItem is not Cidade cidade)
            {
                await DisplayAlertAsync("Atenção", "Selecione um estado e uma cidade.", "OK");
                return;
            }

            try
            {
                ConsultarButton.IsEnabled = false;
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var clima = await _climaService.ConsultarAsync(
                    cidade.CIDSTRDESCRICAO,
                    estado.ESTSTRDESCRICAO);

                var logradouro = LogradouroEntry.Text?.Trim();
                EnderecoLabel.Text = string.IsNullOrWhiteSpace(logradouro)
                    ? $"{cidade.CIDSTRDESCRICAO} - {estado.ESTSTRSIGLA}"
                    : $"{logradouro}, {cidade.CIDSTRDESCRICAO} - {estado.ESTSTRSIGLA}";

                var cultura = CultureInfo.GetCultureInfo("pt-BR");
                MediaLabel.Text = FormatarTemperatura(clima.Media);
                MaximaLabel.Text = FormatarTemperatura(clima.Maxima);
                MinimaLabel.Text = FormatarTemperatura(clima.Minima);
                UmidadeLabel.Text = $"{clima.Umidade}%";
                PressaoLabel.Text = $"{clima.PressaoAtmosferica.ToString("F1", cultura)} hPa";

                RenderizarMapaCalorEGrafico(clima, cidade.CIDSTRDESCRICAO);
                ResultadoLayout.IsVisible = true;
            }
            catch (Exception ex)
            {
                ResultadoLayout.IsVisible = false;
                await DisplayAlertAsync("Erro", $"Não foi possível consultar o clima.\n\n{ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
                ConsultarButton.IsEnabled = true;
            }
        }

        private void RenderizarMapaCalorEGrafico(ClimaInfo clima, string nomeCidade)
        {
            string lat = clima.Latitude.ToString(CultureInfo.InvariantCulture);
            string lon = clima.Longitude.ToString(CultureInfo.InvariantCulture);

            // Intensidade térmica proporcional à temperatura máxima (escala 10°C a 40°C)
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
                        [lat - 0.018, lon + 0.014, {intAlta}],
                        [lat + 0.006, lon + 0.008, {intAlta}],
                        [lat - 0.007, lon - 0.006, {intMedia}]
                    ];

                    L.heatLayer(pontosCalor, {{
                        radius: 38,
                        blur: 24,
                        maxZoom: 14,
                        gradient: {{ 0.2: '#3b82f6', 0.5: '#22c55e', 0.75: '#f59e0b', 1.0: '#ef4444' }}
                    }}).addTo(map);

                    L.marker([lat, lon]).addTo(map)
                        .bindPopup('<b>{nomeCidade}</b><br/>Média: {clima.Media:F1} °C | Umidade: {clima.Umidade}%<br/>Pressão: {clima.PressaoAtmosferica:F1} hPa')
                        .openPopup();

                    var ctx = document.getElementById('tempChart').getContext('2d');
                    new Chart(ctx, {{
                        type: 'line',
                        data: {{
                            labels: {diasJson},
                            datasets: [
                                {{
                                    label: 'Máxima (°C)',
                                    data: {maxJson},
                                    borderColor: '#ef4444',
                                    backgroundColor: 'rgba(239, 68, 68, 0.15)',
                                    fill: true,
                                    tension: 0.3
                                }},
                                {{
                                    label: 'Mínima (°C)',
                                    data: {minJson},
                                    borderColor: '#3b82f6',
                                    backgroundColor: 'rgba(59, 130, 246, 0.15)',
                                    fill: true,
                                    tension: 0.3
                                }}
                            ]
                        }},
                        options: {{ responsive: true, maintainAspectRatio: false }}
                    }});
                </script>
            </body>
            </html>";

            MapaCalorWebView.Source = new HtmlWebViewSource { Html = html };
        }

        private static string FormatarTemperatura(double temperatura)
        {
            return $"{temperatura.ToString("F1", CultureInfo.GetCultureInfo("pt-BR"))} °C";
        }
    }
}
