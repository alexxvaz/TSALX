using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TSALX.Models
{
    public class Campeonato
    {
        public int IDCampeonato { get; set; }
        public List<Regiao> ListaRegiao { get; set; }
        public List<EquipeLista> Coluna1Equipe { get; set; }
        public List<EquipeLista> Coluna2Equipe { get; set; }
        public List<EquipeLista> Coluna3Equipe { get; set; }

        public bool Ativo { get; set; }

        [Display( Name = "Região" )]
        [Required( ErrorMessage = "Selecione uma região" )]
        public short IDRegiao { get; set; }

        [Required( ErrorMessage = "Informe o nome do campeonato" )]
        [StringLength(50, ErrorMessage ="O nome do campeonato deve ser no máximo de 50 caracteres")]
        public string Nome { get; set; }
    }

    public class CampeonatoLista
    {
        public int IDCampeonato { get; set; }
        public string Nome { get; set; }
        public string NomeRegiao { get; set; }
        public bool Ativo { get; set; }
        public string Bandeira { get; set; }
    }
}