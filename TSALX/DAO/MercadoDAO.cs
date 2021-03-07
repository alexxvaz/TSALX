using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Alxware.BD;
using Alxware.Erro;

namespace TSALX.DAO
{
    public class MercadoDAO 
    {
        private BD _oBD = null; 

        public MercadoDAO()
        {
            _oBD = new BD( Util.ConexaoBD );
        }

        public List<Models.Mercado> listar()
        {
            List<Models.Mercado> lst = new List<Models.Mercado>();

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT IDMercado, NomeMercado FROM Mercado" );

                while( rd.Read() )
                {
                    lst.Add( new Models.Mercado()
                    {
                        IDMercado = Convert.ToInt32( rd[ "IDMercado" ] ),
                        Nome = rd[ "NomeMercado" ].ToString()
                    } );
                }

            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                lst = null;
            }

            return lst;
        }

        public void salvar( Models.Mercado pobjMercado )
        {
            try
            {
                StringBuilder oStrDML = new StringBuilder();

                if( pobjMercado.IDMercado == 0 ) // Inserir
                {
                    int intProximoID = Util.informarProximoID( "Mercado", "IDMercado" );

                    if( intProximoID > 0 )
                    {
                        oStrDML.Append( "INSERT INTO Mercado " );
                        oStrDML.AppendFormat( "VALUES ( {0}, '{1}' )", intProximoID, pobjMercado.Nome.Replace( "'", "''" ) );
                    }
                    else
                        throw new alxExcecao( "Não foi informado o próximo IDMercado", ErroTipo.Dados );
                }
                else // Alterar
                {
                    oStrDML.Append( "UPDATE Mercado SET " );
                    oStrDML.AppendFormat( "NomeMercado = '{0}' ", pobjMercado.Nome.Replace( "'", "''" ) );
                    oStrDML.AppendFormat( " WHERE IDMercado = {0}", pobjMercado.IDMercado ); 
                }

                _oBD.executarDML( oStrDML.ToString() );
            }
            catch( alxExcecao ex )
            {
                if( ex.Mensagem.Contains( "UK_Mercado_Nome" ) )
                    throw new alxExcecao( "O nome do mercado '{0}' já foi cadastrado", pobjMercado.Nome );
                else
                {
                    if( ex.Tipo != ErroTipo.Processo )
                        throw ex;

                    new TratamentoErro( ex ).tratarErro();
                    
                }
               
            }
            catch( Exception ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw new alxExcecao( "Problema ao salvar o Mercado" );
            }
        }
        public void excluir( int pintID )
        {

            try
            {
                _oBD.executarDML( "DELETE FROM Mercado WHERE IDMercado = {0}", pintID );
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw ex;
            }
        }

        public Models.Mercado obter( int pintID )
        {
            Models.Mercado oRet = null;

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT IDMercado, NomeMercado FROM Mercado WHERE IDMercado = {0}", pintID );

                if( rd.Read() )
                {
                    oRet = new Models.Mercado()
                    {
                        IDMercado = rd.GetInt32( 0 ),
                        Nome = rd[ 1 ].ToString()
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