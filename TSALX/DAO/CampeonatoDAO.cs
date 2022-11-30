using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Alxware.BD;
using Alxware.Erro;

using TSALX.Servico;
using TSALX.Models;

namespace TSALX.DAO
{
    public class CampeonatoDAO
    {
        private BD _oBD = null;

        public CampeonatoDAO()
        {
            _oBD = new BD( Util.ConexaoBD );
        }
      
        public List<CampeonatoLista> listar()
        {
            List<CampeonatoLista> lst = new List<CampeonatoLista>();

            try
            {
                StringBuilder oQuery = new StringBuilder();

                oQuery.Append( "SELECT c.IDCampeonato, l.IDAPI_Liga, t.AnoInicial, t.AnoFinal, l.NomeLiga, r.SiglaRegiao, l.EhSelecao, c.AtivoCampeonato" );
                oQuery.Append( "  FROM Campeonato c" );
                oQuery.Append( " INNER JOIN Temporada t ON t.IDTemporada = c.IDTemporada" );
                oQuery.Append( " INNER JOIN Liga l ON l.IDLiga = c.IDLiga" );
                oQuery.Append( " INNER JOIN Regiao r ON r.IDRegiao = l.IDRegiao" );

                DataTableReader rd = _oBD.executarQuery( oQuery.ToString() );

                while( rd.Read() )
                {
                    int? intIDLigaAPI = null;

                    if ( !rd.IsDBNull( 1 ) )
                        intIDLigaAPI = Convert.ToInt32( rd[ "IDAPI_Liga" ] );

                    lst.Add( new CampeonatoLista()
                    {
                        IDCampeonato = Convert.ToInt32( rd[ "IDCampeonato" ] ),
                        IDLigaAPI = intIDLigaAPI,
                        AnoInicial = Convert.ToInt16( rd[ "AnoInicial" ] ),
                        AnoFinal = Convert.ToInt16( rd[ "AnoFinal" ] ),
                        NomeLiga = rd[ "NomeLiga" ].ToString(),
                        Sigla = rd[ "SiglaRegiao" ].ToString(),
                        Ativo = Convert.ToBoolean( rd[ "AtivoCampeonato" ] ),
                        EhSelecao = Convert.ToBoolean( rd[ "EhSelecao" ] )
                        
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
        
        public void gravar( Campeonato pobjCampeonato )
        {
            try
            {
                StringBuilder oStrDML = new StringBuilder();

                int intProximoID = Util.informarProximoID( "Campeonato", "IDCampeonato" );

                if( intProximoID > 0 )
                {
                    oStrDML.Append( "INSERT INTO Campeonato (IDCampeonato, IDTemporada, IDLiga, AtivoCampeonato) " );
                    oStrDML.AppendFormat( "VALUES ( {0}, {1}, {2}, true )", intProximoID, pobjCampeonato.IDTemporada, pobjCampeonato.IDLiga );
                }
                else
                    throw new alxExcecao( "Não foi informado o próximo IDCampeonato", ErroTipo.Dados );
                
                _oBD.executarDML( oStrDML.ToString() );
            }
            catch( alxExcecao ex )
            {
                if ( ex.Mensagem.Contains( "UK_Campeonato" ) )
                    throw new alxExcecao( "O campeonato para esta temporada já foi cadastrado" );
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
                throw new alxExcecao( "Problema ao gravar o Campeonato" );
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
        public void ativar( int pintID, bool pblnAtiva )
        {
            try
            {
                _oBD.executarDML( "UPDATE Campeonato SET AtivoCampeonato = {0} WHERE IDCampeonato = {1}", pblnAtiva, pintID );
            }
            catch ( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw new alxExcecao( "Problema ao ativar ou inativar o campeoanto" );
            }
        }
        public Campeonato obter( int pintID )
        {
            Campeonato oRet = null;

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT IDCampeonato, IDTemporada, IDLiga, AtivoCampeonato FROM Campeonato WHERE IDCampeonato = {0}", pintID );

                if( rd.Read() )
                {
                    oRet = new Campeonato()
                    {
                        IDCampeonato = rd.GetInt32( 0 ),
                        IDTemporada = rd.GetInt32( 1 ),
                        IDLiga = rd.GetInt32( 2 ),
                        Ativo = rd.GetBoolean( 3 )
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