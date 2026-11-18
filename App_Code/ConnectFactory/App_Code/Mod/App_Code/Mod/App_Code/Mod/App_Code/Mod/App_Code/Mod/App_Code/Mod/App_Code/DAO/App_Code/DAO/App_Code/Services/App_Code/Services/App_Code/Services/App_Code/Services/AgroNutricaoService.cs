using MauiIBGE.App_Code.Mod;

namespace MauiIBGE.App_Code.Services
{
    public class AgroNutricaoService
    {
        public AgroPerfilLocal ObterPerfilPorLocalidade(string cidade, string uf, string regiao)
        {
            string cidadeNorm = cidade.Trim().ToLowerInvariant();
            string ufNorm = uf.Trim().ToUpperInvariant();

            // Destaques específicos de polos agroindustriais
            if (cidadeNorm.Contains("fernandópolis") || cidadeNorm.Contains("ribeirão preto") || cidadeNorm.Contains("araraquara"))
            {
                return new AgroPerfilLocal
                {
                    TipoAgronegocio = "Agroindústria Sucroenergética, Citricultura e Pecuária",
                    PrincipaisCulturas = "Laranja, Cana-de-açúcar, Seringueira e Gado de Corte",
                    ProdutoMaisVendidoPt = "Suco de Laranja Integral (Orange Juice)",
                    TermoBuscaUsdaEn = "orange juice"
                };
            }

            if (cidadeNorm.Contains("petrolina") || cidadeNorm.Contains("juazeiro"))
            {
                return new AgroPerfilLocal
                {
                    TipoAgronegocio = "Fruticultura Irrigada de Exportação",
                    PrincipaisCulturas = "Manga, Uva de Mesa, Melão e Goiaba",
                    ProdutoMaisVendidoPt = "Manga Fresca (Mango)",
                    TermoBuscaUsdaEn = "mango"
                };
            }

            if (cidadeNorm.Contains("sorriso") || cidadeNorm.Contains("lucas do rio verde") || cidadeNorm.Contains("rio verde"))
            {
                return new AgroPerfilLocal
                {
                    TipoAgronegocio = "Agricultura Extensiva de Grãos e Proteína Animal",
                    PrincipaisCulturas = "Soja, Milho Safrinha, Algodão e Suinocultura",
                    ProdutoMaisVendidoPt = "Grãos de Soja / Proteína de Soja (Soybeans)",
                    TermoBuscaUsdaEn = "soybeans"
                };
            }

            // Mapeamento por Unidade Federativa (UF)
            return ufNorm switch
            {
                "SP" => new AgroPerfilLocal
                {
                    TipoAgronegocio = "Agroindústria Citrícola, Sucroalcooleira e Hortifrúti",
                    PrincipaisCulturas = "Cana-de-açúcar, Laranja, Café, Amendoim e Ovos",
                    ProdutoMaisVendidoPt = "Suco de Laranja Natural (Orange Juice)",
                    TermoBuscaUsdaEn = "orange juice"
                },
                "MG" => new AgroPerfilLocal
                {
                    TipoAgronegocio = "Cafeicultura de Montanha e Pecúaria Leiteira",
                    PrincipaisCulturas = "Café Arábica, Leite, Queijos Artesanais e Milho",
                    ProdutoMaisVendidoPt = "Queijo Minas / Leite Integral (Milk, whole)",
                    TermoBuscaUsdaEn = "milk whole"
                },
                "PR" or "SC" or "RS" => new AgroPerfilLocal
                {
                    TipoAgronegocio = "Cooperativismo Agroindustrial, Grãos e Proteína Animal",
                    PrincipaisCulturas = "Soja, Trigo, Arroz Irrigado, Aves, Suínos e Maçã",
                    ProdutoMaisVendidoPt = "Peito de Frango / Grãos Integrais (Chicken breast)",
                    TermoBuscaUsdaEn = "chicken breast"
                },
                "MT" or "MS" or "GO" or "DF" => new AgroPerfilLocal
                {
                    TipoAgronegocio = "Agronegócio de Grãos de Alta Tecnologia e Pecuária Bovina",
                    PrincipaisCulturas = "Soja, Milho, Algodão, Feijão e Bovinocultura",
                    ProdutoMaisVendidoPt = "Carne Bovina Magra (Beef, top sirloin)",
                    TermoBuscaUsdaEn = "beef sirloin"
                },
                "BA" or "PE" or "CE" or "RN" or "PB" or "AL" or "SE" or "PI" or "MA" => new AgroPerfilLocal
                {
                    TipoAgronegocio = "Fruticultura Tropical, Cacauicultura e Grãos (MATOPIBA)",
                    PrincipaisCulturas = "Cacau, Manga, Coco, Castanha de Caju e Soja",
                    ProdutoMaisVendidoPt = "Castanha de Caju / Cacau (Cashew nuts)",
                    TermoBuscaUsdaEn = "cashew nuts"
                },
                "PA" or "AM" or "RO" or "AC" or "AP" or "RR" or "TO" => new AgroPerfilLocal
                {
                    TipoAgronegocio = "Bioeconomia Amazônica, Extrativismo Sustentável e Pecuária",
                    PrincipaisCulturas = "Açaí, Castanha-do-Brasil, Cacau Nativo, Mandioca e Pesca",
                    ProdutoMaisVendidoPt = "Castanha-do-Brasil / Mandioca (Brazil nuts)",
                    TermoBuscaUsdaEn = "brazil nuts"
                },
                _ => new AgroPerfilLocal
                {
                    TipoAgronegocio = $"Agronegócio Regional ({regiao})",
                    PrincipaisCulturas = "Hortaliças, Grãos, Fruticultura e Pecuária",
                    ProdutoMaisVendidoPt = "Banana Prata / Nanica (Banana, raw)",
                    TermoBuscaUsdaEn = "banana"
                }
            };
        }
    }
}
