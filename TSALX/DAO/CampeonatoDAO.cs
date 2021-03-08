using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Alxware.BD;
using Alxware.Erro;

namespace TSALX.DAO
{
    public class CampeonatoDAO
    {
        private BD _oBD = null;

        public CampeonatoDAO()
        {
            _oBD = new BD( Util.ConexaoBD );
        }
        public List<Models.EquipeLista> listarTimes( int pintCampeonato )
        {
            List<Models.EquipeLista> lstRet = null;

            try
            {
                StringBuilder oStr = new StringBuilder();

                oStr.Append( "SELECT t.IDEquipe, NomeEquipe, NomeRegiao, SiglaRegiao " );
                oStr.Append( "  FROM Regiao r " );
                oStr.Append( " INNER JOIN Equipe e ON R.IDRegiao = E.IDRegiao " );
                oStr.Append( " INNER JOIN Temporada t on e.IDEquipe = t.IDEquipe " );
                oStr.AppendFormat( " WHERE t.IDCampeonato = {0} ", pintCampeonato );
                oStr.Append( " ORDER BY NomeEquipe " );

                DataTableReader rd = _oBD.executarQuery( oStr.ToString() );

                lstRet = new List<Models.EquipeLista>();

                while( rd.Read() )
                {
                    lstRet.Add( new Models.EquipeLista()
                    {
                        IDEquipe = Convert.ToInt32( rd[ "IDEquipe" ] ),
                         Nome = rd[ "NomeEquipe" ].ToString(),
                         NomeRegiao = rd[ "NomeRegiao" ].ToString(),
                         Bandeira = Util.informarBandeira( rd[ "SiglaRegiao" ].ToString() )
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

        private List<Models.CampeonatoLista> listar( bool pblnAtivo )
        {
            List<Models.CampeonatoLista> lst = new List<Models.CampeonatoLista>();

            try
            {
                string strQuery = string.Empty;

                if (!pblnAtivo)
                    strQuery = "SELECT IDCampeonato, NomeCampeonato, NomeRegiao, SiglaRegiao, AtivoCampeonato FROM Regiao r INNER JOIN Campeonato c ON R.IDRegiao = C.IDRegiao order by NomeCampeonato ";
                else
                    strQuery = "SELECT IDCampeonato, NomeCampeonato, NomeRegiao, SiglaRegiao, AtivoCampeonato FROM Regiao r INNER JOIN Campeonato c ON R.IDRegiao = C.IDRegiao WHERE AtivoCampeonato = 1 ORDER BY NomeCampeonato ";

                DataTableReader rd = _oBD.executarQuery( strQuery );

                while( rd.Read() )
                {
                    lst.Add( new Models.CampeonatoLista()
                    {
                        IDCampeonato = Convert.ToInt32( rd[ "IDCampeonato" ] ),
                        Nome = rd[ "NomeCampeonato" ].ToString(),
                        NomeRegiao = rd[ "NomeRegiao" ].ToString(),
                        Bandeira = Util.informarBandeira( rd[ "SiglaRegiao"].ToString() ),
                        Ativo = Convert.ToBoolean( rd[ "AtivoCampeonato" ] )
                        
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
        public List<Models.CampeonatoLista> listar()
        {
            return this.listar(false);
        }
        public List<Models.CampeonatoLista> listarAtivos()
        {
            return this.listar(true);
        }
        
        public void salvar( Models.Campeonato pobjCampeonato )
        {
            try
            {
                StringBuilder oStrDML = new StringBuilder();

                if( pobjCampeonato.IDCampeonato == 0 ) // Inserir
                {
                    int intProximoID = Util.informarProximoID( "Campeonato", "IDCampeonato" );

                    if( intProximoID > 0 )
                    {
                        oStrDML.Append( "INSERT INTO Campeonato " );
                        oStrDML.AppendFormat( "VALUES ( {0}, {1}, '{2}', {3} )", intProximoID, pobjCampeonato.IDRegiao, pobjCampeonato.Nome.Replace( "'", "''" ), pobjCampeonato.Ativo );
                    }
                    else
                        throw new alxExcecao( "Não foi informado o próximo IDCampeonato", ErroTipo.Dados );
                }
                else // Alterar
                {
                    oStrDML.Append( "UPDATE Campeonato SET " );
                    oStrDML.AppendFormat( "NomeCampeonato = '{0}' ", pobjCampeonato.Nome.Replace( "'", "''" ) );
                    oStrDML.AppendFormat( ", IDRegiao = {0}", pobjCampeonato.IDRegiao );
                    oStrDML.AppendFormat( ", AtivoCampeonato = {0}", pobjCampeonato.Ativo );
                    oStrDML.AppendFormat( " WHERE IDCampeonato = {0}", pobjCampeonato.IDCampeonato );
                }

                _oBD.executarDML( oStrDML.ToString() );
            }
            catch( alxExcecao ex )
            {
                if( ex.Mensagem.Contains( "UK_Campeonato_Nome" ) )
                    throw new alxExcecao( "O nome do campeonato '{0}' já foi cadastrado para esta região", pobjCampeonato.Nome );
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
                throw new alxExcecao( "Problema ao salvar o Campeonato" );
            }
        }
        public void excluir( int pintID )
        {

            try
            {
                _oBD.executarDML( "DELETE FROM Campeonato WHERE IDCampeonato = {0}", pintID );
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw ex;
            }
        }

        public Models.Campeonato obter( int pintID )
        {
            Models.Campeonato oRet = null;

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT IDCampeonato, IDRegiao, NomeCampeonato, AtivoCampeonato FROM Campeonato WHERE IDCampeonato = {0}", pintID );

                if( rd.Read() )
                {
                    oRet = new Models.Campeonato()
                    {
                        IDCampeonato = rd.GetInt32( 0 ),
                        IDRegiao = rd.GetInt16( 1 ),
                        Nome = rd[ 2 ].ToString(),
                        Ativo = rd.GetBoolean( 3 )
                    };

                    List<Models.EquipeLista> lstTime = this.listarTimes( pintID );

                    const int COLUNAS = 3;
                    int intQtdEquipesColuna = lstTime.Count / COLUNAS;
                    int intPos = 0;
                    if( ( lstTime.Count % COLUNAS ) != 0 ) intQtdEquipesColuna++;

                    oRet.Coluna1Equipe = lstTime.GetRange( intPos, intQtdEquipesColuna );
                    intPos += intQtdEquipesColuna;

                    if( ( intPos + intQtdEquipesColuna ) <= lstTime.Count )
                    {
                        oRet.Coluna2Equipe = lstTime.GetRange( intPos, intQtdEquipesColuna );
                        intPos += intQtdEquipesColuna;

                        if( ( intPos + intQtdEquipesColuna ) <= lstTime.Count )
                            oRet.Coluna3Equipe = lstTime.GetRange( intPos, intQtdEquipesColuna );
                        else
                            oRet.Coluna3Equipe = lstTime.GetRange( intPos, lstTime.Count - intPos ); 
                    }

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