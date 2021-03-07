using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Alxware.BD;
using Alxware.Erro;

namespace TSALX.DAO
{
    public class EntradaDAO
    {
        private long _lngPartidaID;
        private BD _oBD;

        public EntradaDAO( long plngPartidaID )
        {
            _lngPartidaID = plngPartidaID;
            _oBD = new BD( Util.ConexaoBD );
        }

        private string obterIcone( char pcharCodSitu )
        {
            string strIcone = string.Empty;

            switch( pcharCodSitu )
            {
                case 'G':
                    strIcone = "fa fa-thumbs-o-up";
                    break;
                case 'P':
                    strIcone = "fa fa-thumbs-o-down";
                    break;
                case 'A':
                case 'E':
                    strIcone = "fa fa-hand-paper-o";
                    break;
                
            }

            return strIcone;
        }
        private long informarProximoID()
        {
            long lngRet = -1;

            try
            {
                DataTableReader rdProximo = _oBD.executarQuery( "SELECT IDEntrada FROM Entrada WHERE IDPartida = {0} ORDER BY IDEntrada", _lngPartidaID );

                long lngCont = 1;

                while( rdProximo.Read() )
                {
                    if( lngCont != rdProximo.GetInt64( 0 ) )
                    {
                        lngRet = lngCont;
                        break;
                    }

                    lngCont++;
                }

                lngRet = lngRet == -1 ? lngCont : lngRet;

            }
            catch( alxExcecao ex )
            {
                lngRet = -1;
                throw ex;
            }
            catch( Exception ex )
            {
                lngRet = -1;
                throw new alxExcecao( ex.Message, ErroTipo.Sistema );
            }

            return lngRet;
        }

        public List<Models.Entradas> listar()
        {
            List<Models.Entradas> lstRet = null;

            try
            {
                StringBuilder oStr = new StringBuilder();

                oStr.Append( "SELECT IDEntrada, NomeMercado, e.TipoAposta, e.ODD, e.ValorEntrada, e.CodSituacao, s.NomeSituacao, e.ValorEncerrado, " );
                oStr.Append( "  CASE e.CodSituacao " );
                oStr.Append( "       WHEN 'G' THEN ( ( ValorEntrada * ODD ) - ValorEntrada ) " );
                oStr.Append( "       WHEN 'P' THEN ( ValorEntrada * -1 ) " );
                oStr.Append( "       WHEN 'E' THEN ( ( ValorEntrada - ValorEncerrado ) * -1 ) " );
                oStr.Append( "       WHEN 'A' THEN ( ValorEntrada ) " );
                oStr.Append( "   END as ValorRetorno, IDLanc " );
                oStr.Append( "  FROM Entrada e " );
                oStr.Append( " INNER JOIN Situacaoentrada s on s.CodSituacao = e.CodSituacao " );
                oStr.Append( " INNER JOIN Mercado m on m.IDMercado = e.IDMercado " );
                oStr.AppendFormat( " WHERE e.IDPartida = {0}", _lngPartidaID );

                DataTableReader rd = _oBD.executarQuery( oStr.ToString() );
                lstRet = new List<Models.Entradas>();

                while( rd.Read() )
                {
                    lstRet.Add( new Models.Entradas()
                    {
                        IDEntrada = Convert.ToInt64( rd[ "IDEntrada" ] ),
                        NomeMercado = rd[ "NomeMercado" ].ToString(),
                        TipoAposta = rd[ "TipoAposta" ].ToString(),
                        ODD = Convert.ToDecimal( rd[ "ODD"] ),
                        ValorEntrada = Convert.ToDecimal( rd[ "ValorEntrada" ] ),
                        CodSitucao = rd["CodSituacao"].ToString()[ 0 ],
                        NomeSituacao = rd[ "NomeSituacao" ].ToString(),
                        ValorEncerrado = !rd.IsDBNull( 7 )?  Convert.ToDecimal( rd[ "ValorEncerrado" ] ) : 0,
                        ValorRetorno = Convert.ToDecimal( rd["ValorRetorno" ] ),
                        Icone = this.obterIcone( rd[ "CodSituacao" ].ToString()[ 0 ] ),
                        IDLancamento = !rd.IsDBNull( 9 )? Convert.ToInt64( rd[ "IDLanc" ]): 0

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
        public List<Models.SituacaoEntrada> listarSituacao()
        {
            List<Models.SituacaoEntrada> lstRet = null;

            try
            {
                lstRet = new List<Models.SituacaoEntrada>();
                DataTableReader rd = _oBD.executarQuery( "SELECT CodSituacao, NomeSituacao FROM SituacaoEntrada" );

                while( rd.Read() )
                {
                    lstRet.Add( new Models.SituacaoEntrada()
                    {
                        Codigo = rd[ "CodSituacao" ].ToString()[ 0 ],
                        Nome = rd[ "NomeSituacao" ].ToString()
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

        public Models.Entrada obterEntrada( long plngEntradaID )
        {
            Models.Entrada oRet = new Models.Entrada();

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT * FROM Entrada WHERE IDPartida = {0} AND IDEntrada = {1}", _lngPartidaID, plngEntradaID );
                
                if( rd.Read() )
                {
                    oRet.IDEntrada = plngEntradaID;
                    oRet.IDMercado = Convert.ToInt32( rd[ "IDMercado" ] );
                    oRet.TipoAposta = rd[ "TipoAposta" ].ToString();
                    oRet.ODD = Convert.ToDecimal( rd[ "ODD" ] );
                    oRet.ValorEntrada = Convert.ToDecimal( rd[ "ValorEntrada" ] );
                    oRet.CodSituacao = rd[ "CodSituacao" ].ToString()[ 0 ];
                    oRet.ValorEncerrado = !rd.IsDBNull( 7 ) ? Convert.ToDecimal( rd[ "ValorEncerrado" ] ) : decimal.Zero;
                    oRet.IDLancamento = !rd.IsDBNull( 8 ) ? Convert.ToInt64( rd[ "IDLanc" ] ) : 0;
                }

                oRet.Partida = new PartidaDAO().informar( _lngPartidaID );
                oRet.ListaMercado = new MercadoDAO().listar();
                oRet.ListaEntrada = listarSituacao();

            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                oRet = null;
            }

            return oRet;
        }

        public void salvar( Models.Entrada pobj )
        {
            try
            {
                StringBuilder oStr = new StringBuilder();

                if( pobj.IDEntrada == 0 )
                {
                    oStr.Append( "INSERT INTO Entrada " );
                    oStr.Append( "( IDPartida, IDEntrada, IDMercado, TipoAposta, ODD, ValorEntrada, CodSituacao" );

                    if( pobj.ValorEncerrado > 0 )
                        oStr.Append( ", ValorEncerrado )" );
                    else
                        oStr.Append( ") " );

                    oStr.AppendFormat( " VALUES( {0}, {1}, {2}, ", _lngPartidaID, informarProximoID() , pobj.IDMercado );
                    oStr.AppendFormat( " '{0}', {1}, ", pobj.TipoAposta, pobj.ODD );
                    oStr.AppendFormat( " {0}, '{1}' ", pobj.ValorEntrada, pobj.CodSituacao );

                    if( pobj.CodSituacao == 'E' || pobj.CodSituacao == 'A' )
                        oStr.AppendFormat( ",{0} )", pobj.ValorEncerrado );
                    else
                        oStr.Append( ")" );
                }
                else
                {
                    oStr.Append( "UPDATE Entrada SET " );
                    oStr.AppendFormat( "IDMercado = {0}, ", pobj.IDMercado );
                    oStr.AppendFormat( "TipoAposta = '{0}', ", pobj.TipoAposta );
                    oStr.AppendFormat( "ODD = {0}, ", pobj.ODD );
                    oStr.AppendFormat( "ValorEntrada = {0}, ", pobj.ValorEntrada );
                    oStr.AppendFormat( "CodSituacao = '{0}', ", pobj.CodSituacao );

                    if( pobj.CodSituacao == 'E' || pobj.CodSituacao == 'A' )
                        oStr.AppendFormat( "ValorEncerrado = {0} ", pobj.ValorEncerrado );
                    else
                        oStr.Append( "ValorEncerrado = NULL " );

                    oStr.AppendFormat( " WHERE IDPartida = {0} AND IDEntrada = {1} ", _lngPartidaID, pobj.IDEntrada );

                }

                _oBD.executarDML( oStr.ToString() );

            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw new alxExcecao( "Problema ao salvar a entrada" );
            }
        }

        public void excluir( long plngEntradaID )
        {
            try
            {
                _oBD.executarDML( "DELETE FROM Entrada WHERE IDPartida = {0} AND IDEntrada = {1}", _lngPartidaID, plngEntradaID );
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw new alxExcecao( "Não foi possível excluir a entrada" );
            }
        }
    }
}