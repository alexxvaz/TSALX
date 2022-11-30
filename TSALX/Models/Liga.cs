using System.ComponentModel.DataAnnotations;

using TSALX.Servico;

namespace TSALX.Models
{
    public class Liga
    {
        public int IDLiga { get; set; }
        [Display( Name = "Região" )]
        [Required( ErrorMessage = "Selecione uma região" )]
        public short IDRegiao { get; set; }
        [Required( ErrorMessage = "Informe o nome do campeonato" )]
        [StringLength( 50, ErrorMessage = "O nome do campeonato deve ser no máximo de 50 caracteres" )]
        public string Nome { get; set; }
        [Display( Name = "Somente seleções nacionais?" )]
        public bool Selecao { get; set; }
        [Display( Name = "Código da API" )]
        public int IDAPI { get; set; }
        // Somente Leitura
        public string Logo 
        {
            get { return Util.informarEscudoLiga( IDAPI ); }
        }
    }
}