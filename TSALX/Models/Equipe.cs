using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TSALX.Models;

namespace TSALX.Models
{
    public class Equipe
    {
        public int IDEquipe { get; set; }
        
        [Display( Name = "Região" )]
        [Required( ErrorMessage = "Selecione uma região" )]
        public short IDRegiao { get; set; }
                
        [Required( ErrorMessage ="Informe o nome da equipe")]
        [StringLength(30, ErrorMessage ="O nome da equipe deve ser no máximo 30 caracteres")]
        public string Nome { get; set; }

        public bool Selecao { get; set; }
        [Display( Name = "Código da API")]
        public int IDAPI { get; set; }

    }

    public class EquipeLista
    {
        public int IDEquipe { get; set; }
        public string Nome { get; set; }
        public string NomeRegiao { get; set; }
        public string Bandeira { get; set; }
        public string Escudo { get; set; }
        public bool Selecao { get; set; }
    }

    public class EquipePagina
    {
        public Equipe equipe { get; set; }
        public EquipePesquisa Pesquisa { get; set; }
        public List<Regiao> ListaRegiao { get; set; }
    }

    public class EquipePesquisa
    {
        // Pesquisa
        [Display(Name = "Equipe")]
        public string NomeEquipe { get; set; }
        [Display(Name = "Região")]
        public string NomePais { get; set; }
        [Display(Name = "Liga")]
        public  int IDLiga { get; set; }
        [Display(Name = "Temporada")]
        public short AnoTemporada { get; set; }

        // Lista
        public List<Temporada> ListaTemporadas { get; set; }
        public List<Regiao> ListaRegiao { get; set; }
        public List<Campeonato> ListaLiga { get; set; }
    }
}