using System.ComponentModel.DataAnnotations;

namespace TSALX.Models
{
    public class Regiao
    {
        public short IDRegiao { get; set; }
        [Required( ErrorMessage ="Informe o nome da região")]
        public string Nome { get; set; }
        [StringLength(2, ErrorMessage = "No máximo 2 caracteres" )]
        public string Sigla { get; set; }
        public string Bandeira { get; set; }
    }
}