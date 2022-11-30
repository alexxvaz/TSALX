using System;
using System.ComponentModel.DataAnnotations;

namespace TSALX.Models
{
    public class Temporada
    {
        public int IDTemporada { get; set; }
        [Display( Name = "Ano Inicial" )]
        [Required( ErrorMessage = "Informe o ano inicial" )]
        [Range( 2020, short.MaxValue, ErrorMessage = "O ano inicial deve ser maior que 2020" )]
        public short AnoInicial { get; set; }
        [Display( Name = "Ano Final" )]
        [Required( ErrorMessage = "Informe o ano final" )]
        [Range( 2020, short.MaxValue, ErrorMessage = "O ano final deve ser maior que 2020" )]
        [ValidaAno]
        public short AnoFinal { get; set; }
        public string Anos
        {
            get
            {
                string strRet = string.Empty;

                if ( AnoInicial == AnoFinal )
                    strRet = AnoInicial.ToString();
                else
                    strRet = $"{ AnoInicial - 2000 }/{AnoFinal - 2000}";

                return strRet;
            }
        }
    }

    public class ValidaAno : ValidationAttribute
    {
        protected override ValidationResult IsValid( object value, ValidationContext validationContext )
        {
            short anoFinal = Convert.ToInt16( value );
            Temporada oTemp = (Temporada) validationContext.ObjectInstance;

            if ( anoFinal < oTemp.AnoInicial )
                return new ValidationResult( "Ano inicial é maior que o ano final" );
            else
                return ValidationResult.Success;
        }
    }
}