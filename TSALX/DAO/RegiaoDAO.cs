using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Alxware.BD;
using Alxware.Erro;

namespace TSALX.DAO
{
    public class RegiaoDAO
    {
        private BD _oBD;

        public RegiaoDAO()
        {
            _oBD = new BD( Util.ConexaoBD );
        }
        
        

        public List<Models.Regiao> listar()
        {
            List<Models.Regiao> lst = new List<Models.Regiao>();

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT IDRegiao, NomeRegiao, SiglaRegiao FROM Regiao ORDER BY NomeRegiao" );

                while( rd.Read() )
                {
                    lst.Add( new Models.Regiao()
                    {
                        IDRegiao = Convert.ToInt16( rd[ "IDRegiao" ] ),
                        Nome = rd[ "NomeRegiao" ].ToString(),
                        Sigla = rd.IsDBNull( 2 ) ? string.Empty: rd[ 2 ].ToString(), 
                        Bandeira = Util.informarBandeira( rd[ 2 ].ToString() )
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

        public void salvar( Models.Regiao pobjRegiao )
        {
            try
            {
                StringBuilder oStrDML = new StringBuilder();

                if( pobjRegiao.IDRegiao == 0 ) // Inserir
                {
                    int intProximoID = Util.informarProximoID( "Regiao", "IDRegiao" );

                    if( intProximoID > 0 )
                    {
                        oStrDML.Append( "INSERT INTO Regiao " );

                        if( ! string.IsNullOrEmpty( pobjRegiao.Sigla) )
                            oStrDML.AppendFormat( "VALUES ( {0}, '{1}', '{2}' )", intProximoID, pobjRegiao.Nome.Replace( "'", "''" ), pobjRegiao.Sigla.ToUpper() );
                        else
                            oStrDML.AppendFormat( "VALUES ( {0}, '{1}', NULL )", intProximoID, pobjRegiao.Nome.Replace( "'", "''" ) );
                    }
                    else
                        throw new alxExcecao( "Não foi informado o próximo IDRegiao", ErroTipo.Dados );
                }
                else // Alterar
                {
                    oStrDML.Append( "UPDATE Regiao SET " );
                    oStrDML.AppendFormat( "NomeRegiao = '{0}' ", pobjRegiao.Nome.Replace( "'", "''" ) );

                    if( !string.IsNullOrEmpty( pobjRegiao.Sigla ) )
                        oStrDML.AppendFormat( ", SiglaRegiao = '{0}' ", pobjRegiao.Sigla.ToUpper() );
                    else
                        oStrDML.Append( ", SiglaRegiao = NULL" );

                    oStrDML.AppendFormat( " WHERE IDRegiao = {0}", pobjRegiao.IDRegiao );
                }

                _oBD.executarDML( oStrDML.ToString() );
            }
            catch( alxExcecao ex )
            {
                if( ex.Mensagem.Contains( "UK_Regiao_Nome" ) )
                    throw new alxExcecao( "O nome da região '{0}' já foi cadastrada", pobjRegiao.Nome );
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
                throw new alxExcecao( "Problema ao salvar o Região" );
            }
        }
        public void excluir( int pintID )
        {

            try
            {
                _oBD.executarDML( "DELETE FROM Regiao WHERE IDRegiao = {0}", pintID );
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw ex;
            }
        }

        public Models.Regiao obter( int pintID )
        {
            Models.Regiao oRet = null;

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT IDRegiao, NomeRegiao, SiglaRegiao FROM Regiao WHERE IDRegiao = {0}", pintID );

                if( rd.Read() )
                {
                    oRet = new Models.Regiao()
                    {
                        IDRegiao = rd.GetInt16( 0 ),
                        Nome = rd[ 1 ].ToString(),
                        Sigla = rd.IsDBNull( 2 ) ? string.Empty : rd[ 2 ].ToString(),
                        Bandeira = Util.informarBandeira( rd[ 2 ].ToString() )
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