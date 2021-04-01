using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TSALX.Models
{
    public class Entradas
    {
        public long IDEntrada { get; set; }
        public string NomeMercado { get; set; }
        public string TipoAposta { get; set; }
        public decimal ODD { get; set; }
        public decimal ValorEntrada { get; set; }
        public char CodSitucao { get; set; }
        public string NomeSituacao { get; set; }
        public decimal ValorEncerrado { get; set; }
        public decimal ValorRetorno { get; set; }
        public string Icone { get; set; }
        public long IDLancamento { get; set; }
    }

    public class Entrada
    {
        public long IDEntrada { get; set; }
        public long IDLancamento { get; set; }
        public Partidas Partida { get; set; }
        
        public List<SituacaoEntrada> ListaEntrada { get; set; }
        public List<Mercado> ListaMercado { get; set; }

        [Required(ErrorMessage ="Selecione o mercado")]
        [Display(Name ="Mercado")]
        public int IDMercado { get; set; }

        [Required(ErrorMessage ="Informe o tipo de aposta")]
        [Display(Name ="Tipo de Aposta")]
        public string TipoAposta { get; set; }

        [Required (ErrorMessage ="Informe a ODD")]
        [Range( 0.01, 999999.99, ErrorMessage = "O valor da ODD não pode ser ZERO" )]
        [RegularExpression( @"^[0-9]{0,2}\.[0-9]{0,2}$", ErrorMessage = "Informe o ODD no padrão 99.99" )]
        public decimal ODD { get; set; }

        [Required( ErrorMessage = "Informe o valor da entrada" )]
        [Range( 0.01, 999999.99, ErrorMessage = "O valor não pode ser ZERO" )]
        [RegularExpression( @"^[0-9]{0,2}\.?[0-9]{0,2}$", ErrorMessage = "Informe o valor no padrão 99.99" )]
        [Display(Name = "Valor")]
        public decimal ValorEntrada { get; set; }

        [Required( ErrorMessage ="Informe a situação")]
        [Display(Name ="Situação")]
        public char CodSituacao { get; set; }

        [Display( Name = "Valor Encerrado")]
        [RegularExpression( @"^[0-9]{0,2}\.?[0-9]{0,2}$", ErrorMessage = "Informe o valor no padrão 99.99" )]
        [ValorEncerrado]
        [ValorAnulado]
        public decimal ValorEncerrado { get; set; }
    }

    public class SituacaoEntrada
    {
        public char Codigo { get; set; }
        public string Nome { get; set; }
    }

    public class ValorEncerrado : ValidationAttribute
    {
        protected override ValidationResult IsValid( object value, ValidationContext validationContext )
        {
            char chrSituacao = ( (Entrada) validationContext.ObjectInstance ).CodSituacao;

            if( ( chrSituacao == 'E' ) && ( Convert.ToDecimal( value ) == 0 ) )
                return new ValidationResult( "Informe o valor quando a situação for ENCERROU" );
            else
                return ValidationResult.Success;
        }
    }
    public class ValorAnulado: ValidationAttribute
    {
        protected override ValidationResult IsValid( object value, ValidationContext validationContext )
        {
            char chrSituacao = ( (Entrada) validationContext.ObjectInstance ).CodSituacao;
            decimal decValor = Convert.ToDecimal( value );
            decimal decVlEntrada = ( (Entrada) validationContext.ObjectInstance ).ValorEntrada;

            if( chrSituacao == 'A' )
            {
                if( decValor == 0 ) 
                    return new ValidationResult( "Informe o valor quando a situação for ANULADO" );

                if( decValor != decVlEntrada )
                    return new ValidationResult( "O valor Encerrado deve ser igual o valor da Entrada" );

                return ValidationResult.Success;
            }
            else
                return ValidationResult.Success;
        }
    }

}