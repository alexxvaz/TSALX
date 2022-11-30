using TSALX.Models;

namespace TSALX.ViewModel
{
    public class CampeonatoVM : basePagina
    {
        public int IDCampeonato { get; set; }
        public bool Ativo { get; set; }
        public Liga liga { get; set; }
        public Temporada temporada { get; set; }
        public Regiao regiao { get; set; }
    }
}