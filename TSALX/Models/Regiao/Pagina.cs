using TSALX.Models.API;
using System.Collections.Generic;

namespace TSALX.Models.Regiao
{
    public class Pagina
    {
        public ItemRegiao regiao { get; set; }
        public List<Bandeira> ListaCountry { get; set; }
    }
}