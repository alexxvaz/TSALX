using System;
using System.Collections.Generic;
using System.Text;

using Alxware.BD;
using Alxware.Erro;

using TSALX.Servico;

namespace TSALX.DAO
{
    public class LancamentoDAO
    {
        private BD _oBD = null;

        public LancamentoDAO()
        {
            _oBD = new BD( Util.ConexaoBD );
        }

        private long lancar( Models.Lancamento pobjLanc, Models.TipoLancamento penuTipo )
        {
            long lngProxID = -1;

            try
            {
                char chrNatureza = '\0';

                switch( penuTipo )
                {
                    case Models.TipoLancamento.Saque:
                        chrNatureza = 'D';
                        break;
                    case Models.TipoLancamento.Entrada:
                        if( pobjLanc.ValorLancamento < 0 )
                        {
                            chrNatureza = 'D';
                            pobjLanc.ValorLancamento = pobjLanc.ValorLancamento * -1;
                        }
                        else
                            chrNatureza = 'C';
                        break;
                    default:
                        chrNatureza = 'C';
                        break;
                }

                lngProxID = Util.informarProximoID( "Lancamento", "IDLanc" );

                if( ( pobjLanc.IDPartida == 0 ) && ( pobjLanc.IDEntrada == 0 ) )
                {
                    _oBD.executarDML( "INSERT INTO Lancamento ( IDLanc, DataLanc, IDTipoLanc, ValorLanc, CodNatureza ) VALUES ( {0}, '{1:yyyy-MM-dd}', {2}, {3:0.00}, '{4}' )",
                                        lngProxID, pobjLanc.DataLancamento, Convert.ToInt16( penuTipo ), pobjLanc.ValorLancamento, chrNatureza );
                }
                else
                {
                    StringBuilder oStr = new StringBuilder( "INSERT INTO Lancamento ( IDLanc, DataLanc, IDTipoLanc, IDPartida, IDEntrada, CodNatureza, ValorLanc ) VALUES " );
                    oStr.AppendFormat( "( {0}, '{1:yyyy-MM-dd}', {2}, ", lngProxID, pobjLanc.DataLancamento, Convert.ToInt16( penuTipo ) );
                    oStr.AppendFormat( "{0}, {1}, ", pobjLanc.IDPartida, pobjLanc.IDEntrada );
                    oStr.AppendFormat( "'{0}', {1:0.00} )", chrNatureza, pobjLanc.ValorLancamento );

                    _oBD.executarDML( oStr.ToString() );
                }
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw ex;
            }

            return lngProxID;
        }
        
        public decimal emitirSaldo()
        {
            try
            {
                object oSaldo = _oBD.executarScalar( "SELECT ( sum( if( CodNatureza = 'C', ValorLanc, 0 ) ) - " +
                                                           "         sum( if( CodNatureza = 'D', ValorLanc, 0 ) )  ) " +
                                                           " FROM Lancamento " );

                return oSaldo != DBNull.Value ? Convert.ToDecimal( oSaldo ) : 0;
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                return -1;
                     
            }
        }
        public List<Models.Lancamentos> emitirExtrato( DateTime pdtmInicial, DateTime pdtmFinal )
        {
            List<Models.Lancamentos> lstRet = null;

            try
            {
                StringBuilder oStr = new StringBuilder();

                oStr.Append( "SELECT DataLanc, NomeMercado, e.TipoAposta, e1.NomeEquipe as Equipe1, e2.NomeEquipe as Equipe2, c.NomeCampeonato, " );
                oStr.Append( " CodNatureza, ValorLanc " );
                oStr.Append( " FROM Lancamento l " );
                oStr.Append( " LEFT JOIN Entrada e ON l.IDLanc = e.IDLanc " );
                oStr.Append( " LEFT JOIN Partida p ON p.IDPartida = e.IDPartida " );
                oStr.Append( " LEFT JOIN Equipe e1 ON e1.IDEquipe = p.IDEquipe1 " );
                oStr.Append( " LEFT JOIN Equipe e2 ON e2.IDEquipe = p.IDEquipe2 " );
                oStr.Append( " LEFT JOIN Campeonato c ON c.IDCampeonato = p.IDCampeonato " );
                oStr.Append( " LEFT JOIN Mercado m ON m.IDMercado = e.IDMercado " );

                oStr.AppendFormat( " WHERE DataLanc BETWEEN '{0:yyyyMMdd}' AND '{1:yyyyMMdd}'", pdtmInicial, pdtmFinal );
                oStr.Append( " ORDER BY DataLanc, e.IDPartida, e.IDEntrada" );

                System.Data.DataTableReader rd = _oBD.executarQuery( oStr.ToString() );
                lstRet = new List<Models.Lancamentos>();

                while( rd.Read() )
                {
                    lstRet.Add( new Models.Lancamentos()
                    {
                        DataLanc = Convert.ToDateTime( rd[ "DataLanc" ] ),
                        NomeMercado = rd[ "NomeMercado"].ToString(),
                        TipoAposta = rd[ "TipoAposta"].ToString(),
                        Equipe1 = rd[ "Equipe1"].ToString(),
                        Equipe2 = rd[ "Equipe2" ].ToString(),
                        Campeonato = rd[ "NomeCampeonato" ].ToString(),
                        Natureza = Convert.ToChar( rd[ "CodNatureza" ] ),
                        Valor = Convert.ToDecimal( rd[ "ValorLanc" ] )

                    } );

                }

            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
            }

            return lstRet;
        }
        public void depositar( Models.Lancamento pobjDep )
        {
            try
            {
                this.lancar( pobjDep, Models.TipoLancamento.Deposito );
            }
            catch( alxExcecao )
            {
                throw new alxExcecao( "Não foi possível realizar o depósito" );
            }
        }
        public void sacar( Models.Lancamento pobjSaq )
        {
            try
            {
                this.lancar( pobjSaq, Models.TipoLancamento.Saque );
            }
            catch( alxExcecao )
            {
                throw new alxExcecao( "Não foi possível realizar o saque" );
            }

        }
        public long entrar( Models.Lancamento pobjEntrada )
        {
            try
            {
               return this.lancar( pobjEntrada, Models.TipoLancamento.Entrada);
            }
            catch( alxExcecao )
            {
                throw new alxExcecao( "Não foi possível realizar o saque" );
            }
        }
    }
}