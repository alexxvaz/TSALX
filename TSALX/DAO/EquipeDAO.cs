using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Alxware.BD;
using Alxware.Erro;
using TSALX.Servico;

namespace TSALX.DAO
{
    public class EquipeDAO
    {
        private BD _oBD = null;

        public EquipeDAO()
        {
            _oBD = new BD( Util.ConexaoBD );
        }

        public List<Models.EquipeLista> listar()
        {
            List<Models.EquipeLista> lst = new List<Models.EquipeLista>();

            try
            {
                StringBuilder oStrSQL = new StringBuilder();

                oStrSQL.Append( " SELECT e.IDEquipe, NomeEquipe, NomeRegiao, SiglaRegiao, " );
                oStrSQL.Append( " (SELECT IDEquipe FROM Regiao r WHERE IDEquipe = e.IDEquipe) AS Selecao, " );
                oStrSQL.Append( " e.IDAPI_Equipe " );
                oStrSQL.Append( " FROM Regiao r " );
                oStrSQL.Append( " INNER JOIN Equipe e ON r.IDRegiao = e.IDRegiao " );
                oStrSQL.Append( " ORDER BY NomeRegiao, NomeEquipe" );

                DataTableReader rd = _oBD.executarQuery( oStrSQL.ToString() );

                while( rd.Read() )
                {
                    lst.Add( new Models.EquipeLista()
                    {
                        IDEquipe = Convert.ToInt32( rd[ "IDEquipe" ] ),
                        Nome = rd[ "NomeEquipe" ].ToString(),
                        NomeRegiao = rd[ "NomeRegiao" ].ToString(),
                        Bandeira = Util.informarBandeira( rd[ "SiglaRegiao" ].ToString() ),
                        Selecao = !rd.IsDBNull( 4 ),
                        Escudo = Util.informarEscudoEquipe(rd.IsDBNull(5)? null : (int?) rd[ "IDAPI_Equipe" ] )
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
        public int salvar( Models.Equipe pobjEquipe )
        {
            int intIDRet = -1;

            try
            {
                StringBuilder oStrDML = new StringBuilder();

                if( pobjEquipe.IDEquipe == 0 ) // Inserir
                {
                    int intProximoID = Util.informarProximoID( "Equipe", "IDEquipe" );

                    if( intProximoID > 0 )
                    {
                        oStrDML.Append( "INSERT INTO Equipe " );
                        oStrDML.AppendFormat( "VALUES ( {0}, {1}, '{2}', {3} )", intProximoID, pobjEquipe.IDRegiao, pobjEquipe.Nome.Replace( "'", "''" ), pobjEquipe.IDAPI > 0? pobjEquipe.IDAPI.ToString(): "NULL" );

                        intIDRet = intProximoID;
                    }
                    else
                        throw new alxExcecao( "Não foi informado o próximo IDEquipe", ErroTipo.Dados );
                }
                else // Alterar
                {
                    oStrDML.Append( "UPDATE Equipe SET " );
                    oStrDML.AppendFormat( "NomeEquipe = '{0}' ", pobjEquipe.Nome.Replace( "'", "''" ) );
                    oStrDML.AppendFormat( ", IDRegiao = {0}", pobjEquipe.IDRegiao );
                    oStrDML.AppendFormat( ", IDAPI_Equipe = {0}", pobjEquipe.IDAPI > 0? pobjEquipe.IDAPI.ToString():"NULL"  );
                    oStrDML.AppendFormat( " WHERE IDEquipe = {0}", pobjEquipe.IDEquipe );

                    intIDRet = pobjEquipe.IDEquipe;
                }

                _oBD.executarDML( oStrDML.ToString() );
            }
            catch( alxExcecao ex )
            {
                intIDRet = -1;

                if( ex.Mensagem.Contains( "UK_Equipe_Nome" ) )
                    throw new alxExcecao( "O nome da equipe '{0}' já foi cadastrada para esta região", pobjEquipe.Nome );
                else
                {
                    if( ex.Tipo != ErroTipo.Processo )
                        throw ex;

                    new TratamentoErro( ex ).tratarErro();
                }

            }
            catch( Exception ex )
            {
                intIDRet = -1;

                new TratamentoErro( ex ).tratarErro();
                throw new alxExcecao( "Problema ao salvar a Equipe" );
            }
            
            return intIDRet;
            
        }
        public void excluir( int pintID )
        {

            try
            {
                List<string> lstExcluir = new List<string>();

                lstExcluir.Add( string.Format( "UPDATE Regiao SET IDEquipe = NULL WHERE IDEquipe = {0}", pintID ) );
                lstExcluir.Add( string.Format( "DELETE FROM Equipe WHERE IDEquipe = {0}", pintID ) );

                _oBD.executarDML( lstExcluir );
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw ex;
            }
        }

        public Models.Equipe obter( int pintID )
        {
            Models.Equipe oRet = null;

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT e.IDEquipe, e.IDRegiao, e.NomeEquipe, if( r.IDEquipe IS NOT NULL, true, false ) AS Selecao, IDAPI_Equipe " +
                                                           "FROM Equipe e " +
                                                           "LEFT JOIN Regiao r ON r.IDEquipe = e.IDEquipe " + 
                                                          "WHERE e.IDEquipe = {0}", pintID );

                if( rd.Read() )
                {
                    oRet = new Models.Equipe()
                    {
                        IDEquipe = rd.GetInt32( 0 ),
                        IDRegiao = rd.GetInt16( 1 ),
                        Nome = rd[2].ToString(),
                        Selecao = Convert.ToBoolean( rd[ "Selecao" ] ),
                        IDAPI = rd.IsDBNull(4)? 0 : rd.GetInt32(4)
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