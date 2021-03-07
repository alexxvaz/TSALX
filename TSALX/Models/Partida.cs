using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TSALX.Models
{
    public class Partida
    {
        public long IDPartida { get; set; }

        public List<CampeonatoLista> ListaCampeonato { get; set; }
        public List<EquipeLista> ListaEquipe { get; set; }
        public List<Entradas> ListaEntradas { get; set; }

        public decimal Saldo { get; set; }
        public bool FoiLancado { get; set; }

        [Display( Name = "Campeonato" )]
        [Required( ErrorMessage = "Selecione um campeonato" )]
        public int IDCampeonato { get; set; }

        [Display( Name = "Time 1" )]
        [Required( ErrorMessage = "Selecione o Time 1" )]
        public int IDEquipe1 { get; set; }

        [Display( Name = "Time 2" )]
        [Required( ErrorMessage = "Selecione o Time 2" )]
        [MesmoTime]
        public int IDEquipe2 { get; set; }
        
        [Display( Name = "Data da Partida" )]
        [Required( ErrorMessage = "Informe a data da partida" )]
        [DataMaiorHoje]
        public DateTime DataPartida { get; set; }

    }

    public class Partidas
    {
        public long IDPartida { get; set; }
        public DateTime DataPartida { get; set; }

        public string Equipe1 { get; set; }
        public string BanEquipe1 { get; set; }

        public string Equipe2 { get; set; }
        public string BanEquipe2 { get; set; }

        public string Campeonato { get; set; }
        public string BanCamp { get; set; }

        public List<Entradas> ListaEntradas { get; set; }
    }

    public class Inicio
    {
        [Display( Name = "Data Inicial" )]
        [Required( ErrorMessage = "Informe a data inicial")]
        public DateTime DtInicial { get; set; }
        [Display( Name = "Data Final" )]
       
        [Required( ErrorMessage = "Informe a data final" )]
        public DateTime DtFinal { get; set; }
        public List<Partidas> ListaPartidas { get; set; }
    }

    #region validação 

    public class MesmoTime: ValidationAttribute
    {

        protected override ValidationResult IsValid( object value, ValidationContext validationContext )
        {
            int intTime1 = ( (Partida) validationContext.ObjectInstance ).IDEquipe1;

            if( Convert.ToInt32( value ) == intTime1 )
                return new ValidationResult( "Esta equipe já foi selecionanda para a mesma partida" );
            else
                return ValidationResult.Success;
        }

    }
    public class DataMaiorHoje : ValidationAttribute
    {
        protected override ValidationResult IsValid( object value, ValidationContext validationContext )
        {
            if( Convert.ToDateTime( value ) > DateTime.Today )
                return new ValidationResult( "A data não poder ser maior que a data de hoje" );
            else
                return ValidationResult.Success;
        }
    }
    
    #endregion
}