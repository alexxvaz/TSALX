namespace TSALX.Models
{
    public class LigaLista
    {
        public int IDLiga { get; set; }
        public string Nome { get; set; }
        public string NomeRegiao { get; set; }
        public bool Selecao { get; set; }
        public string Bandeira { get; set; }
        public string Escudo { get; set; }
        public int? IDAPI { get; set; }
    }
}