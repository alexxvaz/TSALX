using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TSALX.Models
{
    public class Equipe
    {
        public int IDEquipe { get; set; }
        public List<Regiao> ListaRegiao { get; set; }
        
        [Display( Name = "Região" )]
        [Required( ErrorMessage = "Selecione uma região" )]
        public short IDRegiao { get; set; }
                
        [Required( ErrorMessage ="Informe o nome da equipe")]
        [StringLength(30, ErrorMessage ="O nome da equipe deve ser no máximo 30 caracteres")]
        public string Nome { get; set; }

        [Display( Name = "Seleção do país?" )]
        public bool Selecao { get; set; }
    }

    public class EquipeLista
    {
        public int IDEquipe { get; set; }
        public string Nome { get; set; }
        public string NomeRegiao { get; set; }
        public string Bandeira { get; set; }
        public bool Selecao { get; set; }
    }
}