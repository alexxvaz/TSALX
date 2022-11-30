using TSALX.Servico;

namespace TSALX.Models
{
    public class CampeonatoLista
    {
        public int IDCampeonato { get; set; }
        public int? IDLigaAPI { get; set; }
        public short AnoInicial { get; set; }
        public short AnoFinal { get; set; }
        public string NomeLiga { get; set; }
        public string Sigla { get; set; }
        public bool EhSelecao { get; set; }
        public bool Ativo { get; set; }

        // Somente leitura
        public string Logo
        {
            get { return Util.informarEscudoLiga( this.IDLigaAPI ); }
        }
        public string Bandeira
        {
            get { return Util.informarBandeira( this.Sigla ); }
        }
        public string Temporada
        {
            get 
            { 
                string strRet = string.Empty;

                if ( AnoInicial == AnoFinal )
                    strRet = AnoInicial.ToString();
                else
                    strRet = $"{ AnoInicial - 2000 }/{AnoFinal - 2000}";

                return strRet;
            }
        }
    }
}