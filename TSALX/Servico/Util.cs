using System;
using System.Data;

using Alxware.BD;
using Alxware.Erro;

namespace TSALX.Servico
{
    public class Util
    {
        internal static readonly string ConexaoBD = "tsalx"; 

        public static int informarProximoID( string pstrTabela, string pstrID )
        {
            BD oBD = new BD( ConexaoBD );
            int intID = -1;
            
            try
            {
                DataTableReader rdProximo = oBD.executarQuery( "SELECT {0} FROM {1} ORDER BY {0}", pstrID, pstrTabela );
                
                int intCont = 1;

                while( rdProximo.Read() )
                {
                    if( intCont != Convert.ToInt32( rdProximo[ 0 ] ) )
                    {
                        intID = intCont;
                        break;
                    }

                    intCont++;
                }

                intID = intID == -1 ? intCont : intID;

            }
            catch( alxExcecao ex )
            {
                intID = -1;
                throw ex;
            }
            catch( Exception ex )
            {
                intID = -1;
                throw new alxExcecao( ex.Message, ErroTipo.Sistema );
            }

            return intID;
        }

        public static string informarBandeira( string pstrSigla )
        {
            if( string.IsNullOrWhiteSpace( pstrSigla.Trim() ) )
                return "/Content/img/fifa.png";
            else if( pstrSigla == "AM-SUL" )
                return "/Content/img/conmebol.png";
            else if( pstrSigla == "AM-NOR")
                return "/Content/img/concacaf.png";
            else
                return string.Format( "https://flagcdn.com/w40/{0}.jpg", pstrSigla.ToString().ToLower() );
        }
        public static string informarEscudoEquipe( int? pintAPIID )
        {
            if ( pintAPIID.HasValue )
            {
                if( pintAPIID.Value > 0 )
                    return $"https://media.api-sports.io/football/teams/{pintAPIID.Value}.png";
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }
    }
}