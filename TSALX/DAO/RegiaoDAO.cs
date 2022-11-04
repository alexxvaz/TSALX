using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using TSALX.Servico;
using TSALX.Models.Regiao;

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
        
        public List<ItemRegiao> listar()
        {
            List<ItemRegiao> lst = new List<ItemRegiao>();

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT IDRegiao, NomeRegiao, SiglaRegiao, Country FROM Regiao ORDER BY NomeRegiao" );

                while( rd.Read() )
                {
                    lst.Add( new ItemRegiao()
                    {
                        IDRegiao = Convert.ToInt16( rd[ "IDRegiao" ] ),
                        Nome = rd[ "NomeRegiao" ].ToString(),
                        Sigla = rd[ "SiglaRegiao" ].ToString(), 
                        Bandeira = Util.informarBandeira( rd[ 2 ].ToString() ),
                        Country = rd[ "Country" ].ToString()
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

        public void salvar( ItemRegiao pobjRegiao )
        {
            short shtRegiaoID = -1;
            try
            {
                StringBuilder oStrDML = new StringBuilder();

                if( pobjRegiao.IDRegiao == 0 ) // Inserir
                {
                    int intProximoID = Util.informarProximoID( "Regiao", "IDRegiao" );

                    if( intProximoID > 0 )
                    {
                        oStrDML.Append( "INSERT INTO Regiao " );
                        oStrDML.AppendFormat( "VALUES ( {0}, NULL, '{1}' ", intProximoID, pobjRegiao.Nome.Replace( "'", "''" ) );

                        // SiglaRegiao
                        if ( ! string.IsNullOrEmpty( pobjRegiao.Sigla) )
                            oStrDML.AppendFormat( ", '{0}' ", pobjRegiao.Sigla.ToUpper() );
                        else
                            oStrDML.Append( ", NULL " );

                        // Country
                        if ( !string.IsNullOrEmpty( pobjRegiao.Country ) )
                            oStrDML.AppendFormat( ", '{0}' ", pobjRegiao.Country );
                        else
                            oStrDML.Append( ", NULL " );

                        oStrDML.Append( " )" );

                        shtRegiaoID = (short) intProximoID;
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

                    if ( !string.IsNullOrEmpty( pobjRegiao.Country ) )
                        oStrDML.AppendFormat( ", Country = '{0}' ", pobjRegiao.Country );
                    else
                        oStrDML.Append( ", Country = 'NULL " );

                    oStrDML.AppendFormat( " WHERE IDRegiao = {0}", pobjRegiao.IDRegiao );

                    shtRegiaoID = pobjRegiao.IDRegiao;
                }

                _oBD.executarDML( oStrDML.ToString() );

                // -----------------------------------------------------------------------------------------
                // Se tiver seleção nacional
                // -----------------------------------------------------------------------------------------

                if ( pobjRegiao.TemSelecao )
                {

                    object oEquipeID = _oBD.executarScalar( "SELECT IDEquipe FROM Regiao WHERE IDRegiao = {0}", shtRegiaoID );
                    int intEquipeID = 0;

                    if ( oEquipeID != DBNull.Value )
                        intEquipeID = Convert.ToInt32( oEquipeID );

                    intEquipeID = new EquipeDAO()
                                     .salvar( new Models.Equipe()
                                     {
                                         IDEquipe = intEquipeID,
                                         IDRegiao = shtRegiaoID,
                                         Nome = pobjRegiao.Nome
                                     });

                    _oBD.executarDML( "UPDATE Regiao SET IDEquipe = {0} WHERE IDRegiao = {1}", intEquipeID, shtRegiaoID );
                }
                else
                    _oBD.executarDML( "UPDATE Regiao SET IDEquipe = NULL WHERE IDRegiao = {0}", shtRegiaoID );

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

        public ItemRegiao obter( int pintID )
        {
            ItemRegiao oRet = null;

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT IDRegiao, NomeRegiao, SiglaRegiao, IDEquipe, Country FROM Regiao WHERE IDRegiao = {0}", pintID );

                if( rd.Read() )
                {
                    oRet = new ItemRegiao()
                    {
                        IDRegiao = rd.GetInt16( 0 ),
                        Nome = rd[ 1 ].ToString(),
                        Sigla = rd.IsDBNull( 2 ) ? string.Empty : rd[ 2 ].ToString(),
                        Bandeira = Util.informarBandeira( rd[ 2 ].ToString() ),
                        TemSelecao = !rd.IsDBNull( 3 ),
                        Country = rd[ 4 ].ToString()
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