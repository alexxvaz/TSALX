using System.ComponentModel.DataAnnotations;

using TSALX.Servico;

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

        // Somente Leitura
        public string Escudo 
        {
            get { return Util.informarEscudoEquipe( IDAPI ); }
        }
    }

}