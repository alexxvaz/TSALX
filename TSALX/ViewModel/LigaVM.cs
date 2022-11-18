using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TSALX.Models;
using TSALX.Pesquisa;

namespace TSALX.ViewModel
{
    public class LigaVM : basePagina
    {
        public Liga liga { get; set; }
        public LigaPesquisa Pesquisa {get;set; }
        public List<Regiao> ListaRegiao { get; set; }
    }
}