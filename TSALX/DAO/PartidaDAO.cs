using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Alxware.BD;
using Alxware.Erro;

namespace TSALX.DAO
{
    public class PartidaDAO
    {
        private BD _oBD = null;

        public PartidaDAO()
        {
            _oBD = new BD( Util.ConexaoBD );
        }

        public void gravar( Models.Partida  pobjPartida )
        {
            try
            {

                // ----------------------------------------------------------------------------------
                // Validações
                // ----------------------------------------------------------------------------------

                InfoSP objInfo = new InfoSP( "SPU_Partida_ValidarTime" );

                objInfo.Parametros.Add( "p_PartidaID", pobjPartida.IDPartida );
                objInfo.Parametros.Add( "p_DataPartida", pobjPartida.DataPartida );
                objInfo.Parametros.Add( "p_Equipe1", pobjPartida.IDEquipe1 );
                objInfo.Parametros.Add( "p_Equipe2", pobjPartida.IDEquipe2 );

                DataTable tb = _oBD.executarSP( objInfo ).Tables[ 0 ];

                if( !Convert.ToBoolean( tb.Rows[ 0 ][ "Valido" ] ) )
                    throw new alxExcecao( tb.Rows[ 0 ][ "Mensagem" ].ToString() );

                // ----------------------------------------------------------------------------------
                // Gravar
                // ----------------------------------------------------------------------------------

                StringBuilder oStr = new StringBuilder();

                if( pobjPartida.IDPartida == 0 ) //Inserir
                {
                    long lngPartidaID = Convert.ToInt64( Util.informarProximoID( "Partida", "IDPartida" ) );

                    oStr.Append( "INSERT INTO Partida " );
                    oStr.Append( "VALUES " );
                    oStr.AppendFormat( "({0}, {1}, ", lngPartidaID, pobjPartida.IDCampeonato );
                    oStr.AppendFormat( "{0}, {1}, ", pobjPartida.IDEquipe1, pobjPartida.IDEquipe2 );
                    oStr.AppendFormat( "'{0:yyyy-MM-dd}' )", pobjPartida.DataPartida );
                }
                else
                {
                    oStr.Append( "UPDATE Partida SET " );
                    oStr.AppendFormat( " IDEquipe1 = {0}, IDEquipe2 = {1}, ", pobjPartida.IDEquipe1, pobjPartida.IDEquipe2 );
                    oStr.AppendFormat( " DataPartida = '{0:yyyy-MM-dd}', IDCampeonato = {1} ", pobjPartida.DataPartida, pobjPartida.IDCampeonato );
                    oStr.AppendFormat( " WHERE IDPartida = {0}", pobjPartida.IDPartida );
                }

                _oBD.executarDML( oStr.ToString() );

            }
            catch( alxExcecao ex )
            {
                if( ex.Mensagem.Contains( "UK_Partida" ) )
                {
                    string strEquipe = string.Empty;

                    if( ex.Mensagem.Contains( "UK_Partida_Equipe1" ) )
                        strEquipe = new EquipeDAO().obter( pobjPartida.IDEquipe1 ).Nome;
                    else
                        strEquipe = new EquipeDAO().obter( pobjPartida.IDEquipe2 ).Nome;

                    throw new alxExcecao( "A equipe \"{0}\" já está agenda para este dia em outro partida", strEquipe );
                }
                else if( ex.Tipo == ErroTipo.Processo )
                    throw;
                else
                {
                    new TratamentoErro( ex ).tratarErro();
                    throw new alxExcecao( "Problema ao gravar a partida" );
                }

            }
        }
        public bool excluir( long plngPartidaID )
        {
            try
            {
                List<string> lstDML = new List<string>();

                lstDML.Add( string.Format( "DELETE FROM Entrada WHERE IDPartida = {0}", plngPartidaID ) );
                lstDML.Add( string.Format( "DELETE FROM Partida WHERE IDPartida = {0}", plngPartidaID ) );

                _oBD.executarDML( lstDML );

                return true;
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                return false;
            }
        }

        public Models.Partida obter( long plngPartidaID )
        {
            Models.Partida objRet = null;

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT * FROM Partida WHERE IDPartida = {0}", plngPartidaID );

                if( rd.Read() )
                {
                    objRet = new Models.Partida()
                    {
                        IDPartida = plngPartidaID, 
                        IDCampeonato = Convert.ToInt32( rd[ "IDCampeonato"] ),
                        IDEquipe1 = Convert.ToInt32( rd[ "IDEquipe1" ] ),
                        IDEquipe2 = Convert.ToInt32( rd[ "IDEquipe2" ] ),
                        DataPartida = Convert.ToDateTime( rd[ "DataPartida" ] )
                    };
                }

                // Adicionar campeonato
                CampeonatoDAO objCamp = new CampeonatoDAO();

                objRet.ListaCampeonato = objCamp.listar();
                objRet.ListaEquipe = objCamp.listarTimes( objRet.IDCampeonato );
                objRet.ListaEntradas = new EntradaDAO( objRet.IDPartida ).listar();

                objRet.Saldo = objRet.ListaEntradas.Sum( s => s.ValorRetorno );

                if( objRet.ListaEntradas.Count > 0 )
                    objRet.FoiLancado = ( objRet.ListaEntradas.FirstOrDefault().IDLancamento > 0 );
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                objRet = null;
            }

            return objRet;
        }
        public Models.Partidas informar( long plngPartidaID )
        {
            Models.Partidas oRet = null;

            try
            {
            
                StringBuilder oStr = new StringBuilder();

                oStr.Append( "SELECT IDPartida, DataPartida, " );
                oStr.Append( "e1.NomeEquipe as Equipe1, r1.SiglaRegiao as BanEquipe1, " );
                oStr.Append( "e2.NomeEquipe as Equipe2, r2.SiglaRegiao as BanEquipe2, " );
                oStr.Append( "c.NomeCampeonato, r3.SiglaRegiao as BanCamp " );
                oStr.Append( " FROM Partida p " );
                oStr.Append( "INNER JOIN Equipe e1 on e1.IDEquipe = p.IDEquipe1 " );
                oStr.Append( "INNER JOIN Equipe e2 on e2.IDEquipe = p.IDEquipe2 " );
                oStr.Append( "INNER JOIN Campeonato c on c.IDCampeonato = p.IDCampeonato " );
                oStr.Append( "INNER JOIN Regiao r1 on r1.IDRegiao = e1.IDRegiao " );
                oStr.Append( "INNER JOIN Regiao r2 on r2.IDRegiao = e2.IDRegiao " );
                oStr.Append( "INNER JOIN Regiao r3 on r3.IDRegiao = c.IDRegiao " );
                oStr.AppendFormat( "WHERE IDPartida = {0}", plngPartidaID );
                
                DataTableReader rd = _oBD.executarQuery( oStr.ToString() );

                if( rd.Read() )
                {
                    oRet = new Models.Partidas()
                    {
                        IDPartida = Convert.ToInt64( rd[ "IDPartida" ] ),
                        DataPartida = Convert.ToDateTime( rd[ "DataPartida" ] ),
                        Equipe1 = rd[ "Equipe1" ].ToString(),
                        BanEquipe1 = Util.informarBandeira( rd[ "BanEquipe1" ].ToString() ),
                        Equipe2 = rd[ "Equipe2" ].ToString(),
                        BanEquipe2 = Util.informarBandeira( rd[ "BanEquipe2" ].ToString() ),
                        Campeonato = rd[ "NomeCampeonato" ].ToString(),
                        BanCamp = Util.informarBandeira( rd[ "BanCamp" ].ToString() )
                        
                    };
                }

            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                oRet = null;
            }

            return oRet;
        }
        public List<Models.Partidas> listar( DateTime pdtmInicial, DateTime pdtmFinal )
        {
            List<Models.Partidas> lstRet = null;

            try
            {
                StringBuilder oStr = new StringBuilder();

                oStr.Append( "SELECT IDPartida, DataPartida, " );
                oStr.Append( "e1.NomeEquipe as Equipe1, r1.SiglaRegiao as BanEquipe1, " );
                oStr.Append( "e2.NomeEquipe as Equipe2, r2.SiglaRegiao as BanEquipe2, " );
                oStr.Append( "c.NomeCampeonato, r3.SiglaRegiao as BanCamp " );
                oStr.Append( " FROM Partida p " );
                oStr.Append( "INNER JOIN Equipe e1 on e1.IDEquipe = p.IDEquipe1 " );
                oStr.Append( "INNER JOIN Equipe e2 on e2.IDEquipe = p.IDEquipe2 " );
                oStr.Append( "INNER JOIN Campeonato c on c.IDCampeonato = p.IDCampeonato " );
                oStr.Append( "INNER JOIN Regiao r1 on r1.IDRegiao = e1.IDRegiao " );
                oStr.Append( "INNER JOIN Regiao r2 on r2.IDRegiao = e2.IDRegiao " );
                oStr.Append( "INNER JOIN Regiao r3 on r3.IDRegiao = c.IDRegiao " );
                oStr.AppendFormat( "WHERE DataPartida BETWEEN '{0:yyyy-MM-dd}' AND '{1:yyyy-MM-dd}'", pdtmInicial, pdtmFinal );
                oStr.Append( "ORDER BY P.DataPartida DESC, p.IDPartida " );

                lstRet = new List<Models.Partidas>();
                DataTableReader rd = _oBD.executarQuery( oStr.ToString() );
                
                while( rd.Read() )
                {
                    lstRet.Add( new Models.Partidas()
                    {
                        IDPartida = Convert.ToInt64( rd[ "IDPartida" ] ),
                        DataPartida = Convert.ToDateTime( rd[ "DataPartida" ] ),
                        Equipe1 = rd[ "Equipe1" ].ToString(),
                        BanEquipe1 = Util.informarBandeira( rd[ "BanEquipe1"].ToString() ),
                        Equipe2 = rd[ "Equipe2" ].ToString(),
                        BanEquipe2 = Util.informarBandeira( rd[ "BanEquipe2" ].ToString() ),
                        Campeonato = rd[ "NomeCampeonato" ].ToString(),
                        BanCamp = Util.informarBandeira( rd["BanCamp"].ToString() ),
                        ListaEntradas = new EntradaDAO( Convert.ToInt64( rd[ "IDPartida" ] ) ).listar()
                        
                    } );
                }

            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
            }

            return lstRet;
        }
        public void finalizar( long plngPartidaID )
        {
            try
            {
                // -------------------------------------------------------------------------------------------------
                // Gera movimento financeiro
                // -------------------------------------------------------------------------------------------------
                DateTime dtPartida = this.obter( plngPartidaID ).DataPartida;
                List<Models.Entradas> lstEntradas = new EntradaDAO( plngPartidaID ).listar();
                Dictionary<long, long> dicLanc = new Dictionary<long, long>();

                foreach( Models.Entradas item in lstEntradas )
                {
                    Models.Lancamento oEntrada = new Models.Lancamento()
                    {
                        DataLancamento = dtPartida,
                        IDPartida = plngPartidaID,
                        IDEntrada = item.IDEntrada,
                        ValorLancamento = item.ValorRetorno
                    };

                    long lngLanc = new LancamentoDAO().entrar( oEntrada );

                    dicLanc.Add( item.IDEntrada, lngLanc );
                }

                // -------------------------------------------------------------------------------------------------
                // Atualiza os lançamentos na tabela ENTRADA
                // -------------------------------------------------------------------------------------------------
                List<string> lstAtuEntrada = new List<string>();
                foreach( KeyValuePair<long, long> itm in dicLanc )
                    lstAtuEntrada.Add( string.Format( "UPDATE Entrada SET IDLanc = {0} WHERE IDPartida = {1} AND IDEntrada = {2}", itm.Value, plngPartidaID, itm.Key ) );

                _oBD.executarDML( lstAtuEntrada );
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw new alxExcecao( "Problema ao fazer lançamentos da entradas" );
            }
        }
    }
}