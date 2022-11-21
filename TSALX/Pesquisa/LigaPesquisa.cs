using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using TSALX.Models;

namespace TSALX.Pesquisa
{
    public class LigaPesquisa
    {
        // Pesquisa
        [Display( Name = "Liga" )]
        public string NomeLiga { get; set; }
        [Display( Name = "Região" )]
        public string NomePais { get; set; }
        [Display( Name = "Temporada" )]
        public short AnoTemporada { get; set; }

        // Lista
        public List<short> ListaTemporadas { get; set; }
        public List<Regiao> ListaRegiao { get; set; }
        
    }
}