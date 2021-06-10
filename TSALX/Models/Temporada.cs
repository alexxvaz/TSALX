using System.Collections.Generic;

namespace TSALX.Models
{
    public class Temporada
    {
        public int IDCampeonato { get; set; }
        public short Ano { get; set; }

        public string equipe_IDEquipe { get; set; }
        public List<CampeonatoLista> ListaCampeonato { get; set; }
        public List<TemporadaEquipe> Coluna1Equipe { get; set; }
        public List<TemporadaEquipe> Coluna2Equipe { get; set; }
        public List<TemporadaEquipe> Coluna3Equipe { get; set; }
        public List<TemporadaEquipe> Coluna4Equipe { get; set; }
    }

    public class TemporadaEquipe
    {
        public int IDEquipe { get; set; }
        public string NomeEquipe { get; set; }
        public string Bandeira { get; set; }
        public bool Participa { get; set; }
        public bool Selecao { get; set; }

    }

}