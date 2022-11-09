using System.Collections.Generic;

using TSALX.Models.API;
using TSALX.Models;

namespace TSALX.ViewModel
{
    public class RegiaoVM : basePagina
    {
        public Regiao regiao { get; set; }
        public List<Bandeira> ListaCountry { get; set; }
    }
}