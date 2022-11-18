using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Alxware.BD;
using Alxware.Erro;

using TSALX.Servico;
using TSALX.Models;

namespace TSALX.DAO
{
    public class TemporadaDAO
    {
        private BD _oBD;

        public TemporadaDAO( )
        {
            _oBD = new BD( Util.ConexaoBD );
        }

        public List<Temporada> listar()
        {
            List<Temporada> lstRet = new List<Temporada>();

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT IDTemporada, AnoInicial, AnoFinal FROM Temporada" );

                while ( rd.Read() )
                {
                    lstRet.Add( new Temporada()
                    {
                        IDTemporada = Convert.ToInt32( rd["IDTemporada"]),
                        AnoInicial = Convert.ToInt16( rd["AnoInicial"]),
                        AnoFinal = Convert.ToInt16( rd[ "AnoFinal" ] )
                    } );
                }
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                lstRet = null;
            }

            return lstRet;
         
        }
        public void salvar( Temporada pobjTemp )
        {
            try
            {
                StringBuilder oStrDML = new StringBuilder();

                if ( pobjTemp.IDTemporada == 0 ) // Inserir
                {
                    int intProximoID = Util.informarProximoID( "Temporada", "IDTemporada" );

                    if ( intProximoID > 0 )
                    {
                        oStrDML.Append( "INSERT INTO Temporada " );
                        oStrDML.AppendFormat( "VALUES ( {0}, {1}, {2} )", intProximoID, pobjTemp.AnoInicial, pobjTemp.AnoFinal );

                    }
                    else
                        throw new alxExcecao( "Não foi informado o próximo IDTemporada", ErroTipo.Dados );
                }
                else // Alterar
                {
                    oStrDML.Append( "UPDATE Temporada SET " );
                    oStrDML.AppendFormat( "AnoInicial = {0} ", pobjTemp.AnoInicial );
                    oStrDML.AppendFormat( " ,AnoFinal = {0} ", pobjTemp.AnoFinal );
                    oStrDML.AppendFormat( " WHERE IDTemporada = {0}", pobjTemp.IDTemporada );
                }

                _oBD.executarDML( oStrDML.ToString() );
            }
            catch ( alxExcecao ex )
            {
                if ( ex.Mensagem.Contains( "UK_Temporada_Ano" ) )
                    throw new alxExcecao( "A temporada informada já está cadastrada ano inicial: {0} e ano final: {1}", pobjTemp.AnoInicial, pobjTemp.AnoFinal );
                else
                {
                    if ( ex.Tipo != ErroTipo.Processo )
                        new TratamentoErro( ex ).tratarErro();

                    throw ex;
                }

            }
            catch ( Exception ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw new alxExcecao( "Problema ao salvar a Temporada" );
            }

           
        }

        public void excluir( int pintID )
        {
            try
            {
                _oBD.executarDML( "DELETE FROM Temporada WHERE IDTemporada = {0}", pintID );
            }
            catch ( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw ex;
            }
        }
        public Temporada obter( int pintID )
        {
            Temporada oRet = null;

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT  AnoInicial, AnoFinal FROM Temporada WHERE IDTemporada = {0}", pintID );

                if( rd.Read() )
                {
                    oRet = new Temporada()
                    {
                        IDTemporada = pintID,
                        AnoInicial = Convert.ToInt16( rd[ "AnoInicial" ] ),
                        AnoFinal = Convert.ToInt16( rd[ "AnoFinal" ] )
                    };
                }
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
            }

            return oRet;
        }
    }
}
