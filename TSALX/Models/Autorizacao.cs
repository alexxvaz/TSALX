using System.ComponentModel.DataAnnotations;

namespace TSALX.Models
{
    public class Autorizacao
    {
        [Required( ErrorMessage ="Informe o código de acesso" )] 
        [Display( Name ="Código de acesso")]
        public string Codigo { get; set; }
    }
}