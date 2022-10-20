using System;

namespace ImportarTSALX
{
    public class Aposta
    {
        public DateTime DataPartida { get; set; }
        public string Campeonato { get; set; }
        public string Equipe1 { get; set; }
        public string Equipe2 { get; set; }
        public string Mercado { get; set; }
        public string TipoAposta { get; set; }
        public decimal ODD { get; set; }
        public decimal ValorAposta { get; set; }
        public char Situacao { get; set; }
        public decimal ValorRetorno { get; set; }
        public long PartidaID { get; set; }

    }
}
