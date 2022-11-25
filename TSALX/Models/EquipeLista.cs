using TSALX.Servico;

namespace TSALX.Models
{
    public class EquipeLista
    {
        public int IDEquipe { get; set; }
        public string Nome { get; set; }
        public string NomeRegiao { get; set; }
        public string Sigla { get; set; }
        public bool Selecao { get; set; }
        public int? IDEquipeAPI { get; set; }

        // Somente Leitura
        public string Bandeira
        {
            get { return Util.informarBandeira( Sigla ); }
        }
        public string Escudo
        {
            get { return Util.informarEscudoEquipe( IDEquipeAPI ); }
        }

    }

}