using System.ComponentModel.DataAnnotations;

namespace TSALX.Models
{
    public class Campeonato
    {
        public int IDCampeonato { get; set; }
        [Display( Name = "Temporada" )]
        public int IDTemporada { get; set; }
        [Display(Name = "Liga")]
        public int IDLiga { get; set; }
        public bool Ativo { get; set; }
    }
}