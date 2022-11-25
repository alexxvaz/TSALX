using TSALX.Servico;

namespace TSALX.Models
{
    public class LigaLista
    {
        public int IDLiga { get; set; }
        public string Nome { get; set; }
        public string NomeRegiao { get; set; }
        public bool Selecao { get; set; }
        public int? IDLigaAPI { get; set; }
        public string Sigla { get; set; }

        public string Bandeira 
        {
            get { return Util.informarBandeira( Sigla ); }
        }
        public string Escudo 
        {
            get { return Util.informarEscudoLiga( IDLigaAPI ); } 
        }

    }
}