using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TSALX.Models;

namespace TSALX.ViewModel
{
    public class TemporadaVM : basePagina
    {
        public Temporada temporada { get; set; }
        public List<Temporada> ListaTemporada { get; set; }
    }
}