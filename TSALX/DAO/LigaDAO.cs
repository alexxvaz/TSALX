using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Alxware.BD;
using Alxware.Erro;

using TSALX.Models;
using TSALX.Servico;

namespace TSALX.DAO
{
    public class LigaDAO
    {
        private BD _oBD = null;

        public LigaDAO()
        {
            _oBD = new BD( Util.ConexaoBD );
        }

        public List<LigaLista> listar()
        {
            List<LigaLista> lstRet = new List<LigaLista>();

            try
            {
                StringBuilder oStr = new StringBuilder();

                oStr.Append( "SELECT IDLiga, NomeLiga, NomeRegiao, SiglaRegiao, EhSelecao, IDAPI_Liga " );
                oStr.Append( "FROM Regiao r " );
                oStr.Append( "INNER JOIN Liga l ON r.IDRegiao = l.IDRegiao " );
                oStr.Append( "ORDER BY NomeLiga" );

                DataTableReader rd = _oBD.executarQuery( oStr.ToString() );

                while( rd.Read() )
                {
                    int? intAPI_ID = null;

                    if ( !rd.IsDBNull( 5 ) )
                        intAPI_ID = Convert.ToInt32( rd[ "IDAPI_Liga" ] );

                    lstRet.Add( new LigaLista() 
                    { 
                        IDLiga = Convert.ToInt32( rd[ "IDLiga" ] ),
                        Nome = rd[ "NomeLiga" ].ToString(),
                        NomeRegiao = rd[ "NomeRegiao" ].ToString(),
                        Selecao = Convert.ToBoolean( rd[ "EhSelecao" ] ),
                        Bandeira = Util.informarBandeira( rd[ "SiglaRegiao" ].ToString() ),
                        Escudo = Util.informarEscudoLiga( intAPI_ID  ), 
                        IDAPI = intAPI_ID
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
        public void salvar( Liga pobjLiga )
        {
            try
            {
                StringBuilder oStrDML = new StringBuilder();

                if ( pobjLiga.IDLiga == 0 ) // Inserir
                {
                    int intProximoID = Util.informarProximoID( "Liga", "IDLiga" );

                    if ( intProximoID > 0 )
                    {
                        oStrDML.Append( "INSERT INTO Liga " );
                        oStrDML.AppendFormat( "VALUES ( {0}, {1}, '{2}', {3}, {4} )", intProximoID, pobjLiga.IDRegiao, pobjLiga.Nome.Replace( "'", "''" ), pobjLiga.Selecao, pobjLiga.IDAPI > 0? pobjLiga.IDAPI.ToString() : "NULL" );
                    }
                    else
                        throw new alxExcecao( "Não foi informado o próximo IDLiga", ErroTipo.Dados );
                }
                else // Alterar
                {
                    oStrDML.Append( "UPDATE Liga SET " );
                    oStrDML.AppendFormat( "NomeLiga = '{0}' ", pobjLiga.Nome.Replace( "'", "''" ) );
                    oStrDML.AppendFormat( ", IDRegiao = {0}", pobjLiga.IDRegiao );
                    oStrDML.AppendFormat( ", EhSelecao = {0}", pobjLiga.Selecao );
                    oStrDML.AppendFormat( ", IDAPI_Liga = {0}", pobjLiga.IDAPI > 0? pobjLiga.IDAPI.ToString() : "NULL" );
                    oStrDML.AppendFormat( " WHERE IDLiga = {0}", pobjLiga.IDLiga );
                }

                _oBD.executarDML( oStrDML.ToString() );
            }
            catch ( alxExcecao ex )
            {
                if ( ex.Mensagem.Contains( "UK_Liga_Nome" ) )
                    throw new alxExcecao( "O nome da liga '{0}' já foi cadastrado para esta região", pobjLiga.Nome );
                else
                {
                    if ( ex.Tipo != ErroTipo.Processo )
                        throw ex;

                    new TratamentoErro( ex ).tratarErro();

                }

            }
            catch ( Exception ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw new alxExcecao( "Problema ao salvar o Campeonato" );
            }
        }
        public void excluir( int pintID )
        {

            try
            {
                _oBD.executarDML( "DELETE FROM Liga WHERE IDLiga = {0}", pintID );
            }
            catch ( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw ex;
            }
        }
        public Liga obter( int pintID )
        {
            Liga oRet = null;

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT IDLiga, IDRegiao, NomeLiga, EhSelecao, IDAPI_Liga FROM Liga WHERE IDLiga = {0}", pintID );

                if ( rd.Read() )
                {
                    oRet = new Liga()
                    {
                        IDLiga = rd.GetInt32( 0 ),
                        IDRegiao = rd.GetInt16( 1 ),
                        Nome = rd[ 2 ].ToString(),
                        Selecao = rd.GetBoolean( 3 ),
                        IDAPI = rd.IsDBNull( 4 )? 0 : rd.GetInt32( 4 )
                    };

                }
            }
            catch ( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
            }

            return oRet;
        }
    }
}