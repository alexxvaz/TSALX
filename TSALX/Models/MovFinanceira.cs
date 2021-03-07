using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TSALX.Models
{
    public class Lancamento
    {
        [Display( Name = "Data" )]
        [DisplayFormat( DataFormatString = "dd/MM/yyyy" )]
        [Required( ErrorMessage = "Informe a data do lançamento" )]
        [DataMaiorHoje]
        public DateTime DataLancamento { get; set; }

        [Display( Name = "Valor" )]
        [Required( ErrorMessage = "Informe o valor" )]
        [Range( 0.01, 999999.99, ErrorMessage = "O valor do depósito não pode ser ZERO" )]
        [RegularExpression( @"^([1-9]{1}[\d]{0,2}(\,[\d]{3})*(\.[\d]{0,2})?|[1-9]{1}[\d]{0,}(\.[\d]{0,2})?|0(\.[\d]{0,2})?|(\.[\d]{1,2})?)$", ErrorMessage = "Formato do valor inválido. O formato deve ser 99.99" )]
        public decimal ValorLancamento { get; set; }

        public long IDPartida { get; set; }
        public long IDEntrada { get; set; }
    }

    public enum TipoLancamento: short
    {
        Deposito = 1,
        Saque,
        Entrada
    }

    public class Extrato
    {
        [Display (Name ="Data Inicial")]
        [Required( ErrorMessage = "Informe a data inicial")]
        public DateTime DtInicial { get; set; }

        [Display( Name = "Data Final" )]
        [Required( ErrorMessage = "Informe a data final" )]
        public DateTime DtFinal { get; set; }

        public List<Lancamentos> Registros { get; set; }
    }

    public class Lancamentos
    {
        public DateTime DataLanc { get; set; }
        public string NomeMercado { get; set; }
        public string TipoAposta { get; set; }
        public string Equipe1 { get; set; }
        public string Equipe2 { get; set; }
        public string Campeonato { get; set; }
        public char Natureza { get; set; }
        public decimal Valor { get; set; }
        public short AnoTemporada { get; set; }
    }
 
}