using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TSALX.Models;

namespace TSALX.ViewModel
{
    public class CampeonatoPG : basePagina
    {
        public Campeonato campeonato { get; set; }

        public List<Temporada> ListaTemporada { get; set; }
        public List<LigaLista> ListaLiga { get; set; }
        public List<CampeonatoLista> ListaCampeonato { get; set; }
        
    }
}