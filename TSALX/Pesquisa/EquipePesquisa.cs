using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using TSALX.Models;

namespace TSALX.Pesquisa
{
    public class EquipePesquisa
    {
        // Pesquisa
        [Display( Name = "Equipe" )]
        public string NomeEquipe { get; set; }
        [Display( Name = "Região" )]
        public string NomePais { get; set; }
        [Display( Name = "Liga" )]
        public int IDAPILiga { get; set; }
        [Display( Name = "Temporada" )]
        public short AnoTemporada { get; set; }

        // Lista
        public List<short> ListaTemporadas { get; set; }
        public List<Regiao> ListaRegiao { get; set; }
        public List<LigaLista> ListaLiga { get; set; }
    }
}