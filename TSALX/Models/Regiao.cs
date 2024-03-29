﻿using System.ComponentModel.DataAnnotations;
using TSALX.Servico;

namespace TSALX.Models
{
    public class Regiao
    {
        public short IDRegiao { get; set; }
        [Required( ErrorMessage = "Informe o nome da região" )]
        public string Nome { get; set; }
        [StringLength( 6, ErrorMessage = "No máximo 6 caracteres" )]
        public string Sigla { get; set; }
        [Display( Name = "Tem seleção nacional" )]
        public bool TemSelecao { get; set; }
        [Display( Name = "Nome em Inglês" )]
        public string CodCountry { get; set; }
        public string Country { get; set; }

        // Somente leiutra
        public string Bandeira 
        {
            get { return Util.informarBandeira( Sigla ); }
        }
    }
}