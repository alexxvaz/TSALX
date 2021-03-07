using System.ComponentModel.DataAnnotations;

namespace TSALX.Models
{
    public class Mercado
    {
        public int IDMercado { get; set; }

        [Required(ErrorMessage ="Informe o nome do mercado")]
        public string Nome { get; set; }
    }
}