namespace MauiIBGE.App_Code.Mod
{
    public class ConsultaFormModel
    {
        public string Cep { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string EstadoSigla { get; set; } = string.Empty;
        public string EstadoNome { get; set; } = string.Empty;
        public string RegiaoNome { get; set; } = string.Empty;
        public string CidadeNome { get; set; } = string.Empty;
    }

    public class AgroPerfilLocal
    {
        public string TipoAgronegocio { get; set; } = string.Empty;
        public string PrincipaisCulturas { get; set; } = string.Empty;
        public string ProdutoMaisVendidoPt { get; set; } = string.Empty;
        public string TermoBuscaUsdaEn { get; set; } = string.Empty;
    }
}
