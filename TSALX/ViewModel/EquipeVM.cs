using System.Collections.Generic;

using TSALX.Models;
using TSALX.Pesquisa;

namespace TSALX.ViewModel
{
    public class EquipeVM : basePagina
    {
        public Equipe equipe { get; set; }
        public EquipePesquisa Pesquisa { get; set; }
        public List<Regiao> ListaRegiao { get; set; }
    }
}