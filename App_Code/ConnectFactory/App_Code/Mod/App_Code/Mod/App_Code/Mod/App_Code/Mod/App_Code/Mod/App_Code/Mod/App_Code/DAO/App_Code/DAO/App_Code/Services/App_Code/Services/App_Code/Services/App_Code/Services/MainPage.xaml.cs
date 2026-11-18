using MauiIBGE.App_Code.ConnectFactory;
using MauiIBGE.App_Code.DAO;
using MauiIBGE.App_Code.Mod;
using MauiIBGE.App_Code.Services;

namespace MauiIBGE
{
    public partial class MainPage : ContentPage, IQueryAttributable
    {
        #region AtributoForm/Object
        private readonly EstadoDAO _estadoDAO;
        private readonly CidadeDAO _cidadeDAO;
        private readonly ClimaService _climaService;
        private readonly ViaCepService _viaCepService;
        private List<Estado> _listaEstados = new();
        private string _cidadePendenteSetForm = string.Empty;
        #endregion

        public MainPage()
        {
            InitializeComponent();
            var services = new Services();
            _estadoDAO = new EstadoDAO(services);
            _cidadeDAO = new CidadeDAO(services);
            _climaService = new ClimaService();
            _viaCepService = new ViaCepService();

            Routing.RegisterRoute("detalhesclima", typeof(Views.ClimaMapaPage));
            CarregarEstados();
        }

        // Recebe parâmetros caso o usuário venha da tela ViaCEP
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var dados = new ConsultaFormModel
            {
                Cep = query.TryGetValue("Cep", out var c) ? c?.ToString() ?? "" : "",
                Logradouro = query.TryGetValue("Logradouro", out var l) ? l?.ToString() ?? "" : "",
                EstadoSigla = query.TryGetValue("Uf", out var uf) ? uf?.ToString() ?? "" : "",
                CidadeNome = query.TryGetValue("Cidade", out var cid) ? cid?.ToString() ?? "" : ""
            };

            await SetForm(dados);
        }

        #region Form
        public ConsultaFormModel GetForm()
        {
            var estado = EstadoPicker.SelectedItem as Estado;
            var cidade = CidadePicker.SelectedItem as Cidade;

            return new ConsultaFormModel
            {
                Cep = CepEntry.Text?.Trim() ?? string.Empty,
                Logradouro = LogradouroEntry.Text?.Trim() ?? string.Empty,
                EstadoSigla = estado?.ESTSTRSIGLA ?? string.Empty,
                EstadoNome = estado?.ESTSTRDESCRICAO ?? string.Empty,
                RegiaoNome = estado?.Regiao?.Nome ?? "Brasil",
                CidadeNome = cidade?.CIDSTRDESCRICAO ?? string.Empty
            };
        }

        public async Task SetForm(ConsultaFormModel dados)
        {
            CepEntry.Text = dados.Cep;
            LogradouroEntry.Text = dados.Logradouro;
            LblMsg.Text = string.Empty;

            if (!string.IsNullOrWhiteSpace(dados.EstadoSigla))
            {
                if (_listaEstados.Count == 0)
                {
                    _listaEstados = await _estadoDAO.ListarEstadoAsync();
                    EstadoPicker.ItemsSource = _listaEstados;
                }

                var estadoEncontrado = _listaEstados.FirstOrDefault(e =>
                    string.Equals(e.ESTSTRSIGLA, dados.EstadoSigla, StringComparison.OrdinalIgnoreCase));

                if (estadoEncontrado != null)
                {
                    _cidadePendenteSetForm = dados.CidadeNome;
                    EstadoPicker.SelectedItem = estadoEncontrado;
                    await CarregaCidade(estadoEncontrado);
                }
            }
        }

        public void ClearForm()
        {
            CepEntry.Text = string.Empty;
            LogradouroEntry.Text = string.Empty;
            EstadoPicker.SelectedItem = null;
            CidadePicker.ItemsSource = null;
            CidadePicker.SelectedItem = null;
            CidadePicker.IsEnabled = false;
            LblRegiaoSelecionada.Text = "Região: --";
            LblMsg.Text = string.Empty;
        }

        public async Task<bool> ValidaForm()
        {
            LblMsg.Text = string.Empty;

            if (EstadoPicker.SelectedItem is not Estado)
            {
                LblMsg.Text = "Selecione um Estado antes de consultar.";
                await DisplayAlertAsync("Atenção", "Selecione um estado na lista do IBGE.", "OK");
                EstadoPicker.Focus();
                return false;
            }

            if (CidadePicker.SelectedItem is not Cidade)
            {
                LblMsg.Text = "Selecione uma Cidade antes de consultar.";
                await DisplayAlertAsync("Atenção", "Selecione uma cidade na lista do IBGE.", "OK");
                CidadePicker.Focus();
                return false;
            }

            return true;
        }
        #endregion

        private async void CarregarEstados()
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                _listaEstados = await _estadoDAO.ListarEstadoAsync();
                EstadoPicker.ItemsSource = _listaEstados;
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

        private void EstadoPicker_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (EstadoPicker.SelectedItem is not Estado estado)
            {
                LblRegiaoSelecionada.Text = "Região: --";
                return;
            }

            LblRegiaoSelecionada.Text = $"Região: {estado.Regiao?.Nome ?? "Não informada"}";
            _ = CarregaCidade(estado);
        }

        private async Task CarregaCidade(Estado estado)
        {
            try
            {
                CidadePicker.ItemsSource = null;
                CidadePicker.SelectedItem = null;
                CidadePicker.IsEnabled = false;

                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var cidades = await _cidadeDAO.ListarCidadeAsync(estado.ESTSTRSIGLA);
                CidadePicker.ItemsSource = cidades;
                CidadePicker.IsEnabled = true;

                if (!string.IsNullOrWhiteSpace(_cidadePendenteSetForm))
                {
                    var cidadeEncontrada = cidades.FirstOrDefault(c =>
                        string.Equals(c.CIDSTRDESCRICAO, _cidadePendenteSetForm, StringComparison.OrdinalIgnoreCase));

                    if (cidadeEncontrada != null)
                    {
                        CidadePicker.SelectedItem = cidadeEncontrada;
                    }
                    _cidadePendenteSetForm = string.Empty;
                }
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

        private async void BuscarCepRapidoButton_Clicked(object? sender, EventArgs e)
        {
            string cep = CepEntry.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(cep))
            {
                await DisplayAlertAsync("Atenção", "Digite um CEP válido com 8 dígitos.", "OK");
                return;
            }

            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var viaCep = await _viaCepService.ObterEnderecoPorCepAsync(cep);
                if (viaCep == null)
                {
                    await DisplayAlertAsync("CEP não encontrado", "Não foi possível localizar o CEP informado.", "OK");
                    return;
                }

                await SetForm(new ConsultaFormModel
                {
                    Cep = viaCep.Cep,
                    Logradouro = viaCep.Logradouro,
                    EstadoSigla = viaCep.Uf,
                    CidadeNome = viaCep.Localidade
                });
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Erro ViaCEP", ex.Message, "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        private async void ConsultarButton_Clicked(object? sender, EventArgs e)
        {
            if (!await ValidaForm()) return;

            var formDados = GetForm();

            try
            {
                ConsultarButton.IsEnabled = false;
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var clima = await _climaService.ConsultarAsync(formDados.CidadeNome, formDados.EstadoNome);

                // Navegação em Pilha (Stack) passando objetos entre telas
                var parametros = new Dictionary<string, object>
                {
                    ["FormDados"] = formDados,
                    ["ClimaInfo"] = clima
                };

                await Shell.Current.GoToAsync("detalhesclima", parametros);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Erro", $"Não foi possível consultar o clima.\n\n{ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
                ConsultarButton.IsEnabled = true;
            }
        }

        private void LimparButton_Clicked(object? sender, EventArgs e)
        {
            ClearForm();
        }
    }
}
