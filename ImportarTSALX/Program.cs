using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Data;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using Alxware.BD;
using Alxware.Erro;
using System.Net;

namespace ImportarTSALX
{
    class Program
    {
        static void Main( string[] args ) 
        { 
            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine( new String( '-', 80 ) );
                Console.WriteLine( "API do Futebol" );
                Console.WriteLine( new String( '-', 80 ) );
                Console.WriteLine( "\nIniciando....\n" );

                //criarScriptSportsbetTXT();
                // criarScriptLiga();
                //pesquisarLiga( "Ligue" );
                //listarBandeiras();
                //pesquisarEquipe( "Toronto" );
                criarScriptCampeonato();

            }
            catch ( alxExcecao ex )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine( $"Erro: { ex.Mensagem }" );
            }
            catch ( IOException ex )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine( $"Erro: { ex.Message }" );
            }
            catch ( Exception ex )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine( $"Erro: { ex.Message }" );
            }
            finally
            {
                Console.WriteLine( "\nProcesso finalizado..." );
                Console.ReadLine();

            }
        }

        #region Criação de script para as apostas antes do sistema
        static void criarScriptSportsbetTXT()
        {
            StreamReader oLeitura = new StreamReader( @"C:\Users\Alex\OneDrive\Projetos\Trade Sport ALX\Sportsbet.IO.txt" );
            List<Aposta> lstAposta = new List<Aposta>();
            

            // -----------------------------------------------------------------------------------------
            // Lendo o arquivo
            // -----------------------------------------------------------------------------------------
            while( oLeitura.Peek() > 0  )
            {
                string strLinha = oLeitura.ReadLine();

                if( !strLinha.Contains( "Data\tCampeonato\tJogo\tMercado" ) )
                {
                    string[] arrLinha = strLinha.Split( '\t' );
                    int intPosVS = arrLinha[ 2 ].IndexOf( "vs" );

                    string strEquipe1 = arrLinha[ 2 ].Substring( 0, intPosVS - 1 );
                    string strEquipe2 = arrLinha[ 2 ].Substring( intPosVS + 3 );

                    lstAposta.Add( new Aposta()
                    {
                        DataPartida = Convert.ToDateTime( arrLinha[ 0 ] ),
                        Campeonato = arrLinha[ 1 ],
                        Equipe1 = strEquipe1,
                        Equipe2 = strEquipe2,
                        Mercado = arrLinha[ 3 ],
                        TipoAposta = arrLinha[ 4 ],
                        ODD = Convert.ToDecimal( arrLinha[ 5 ] ),
                        ValorAposta = Convert.ToDecimal( arrLinha[ 6 ].Replace("$", String.Empty ) ),
                        Situacao = arrLinha[ 7 ][ 0 ],
                        ValorRetorno = arrLinha[ 7 ][ 0 ] == 'E' ? Convert.ToDecimal( arrLinha[ 8 ].Replace( "$", String.Empty ) ) : 0

                    } );

                }
            }
            long lngPartidaID = 1;

            foreach( var itmAposta in lstAposta.GroupBy( a => a.DataPartida.ToString("yyyyMMdd") 
                                                       + a.Campeonato 
                                                       + a.Equipe1.ToString() 
                                                       + a.Equipe2.ToString() ) )
            {
                foreach( var itm in itmAposta )
                    itm.PartidaID = lngPartidaID;

                lngPartidaID++;
            }


            oLeitura.Close();

            // -----------------------------------------------------------------------------------------
            // Colentando Equipe, Campeonato e Mercado
            // -----------------------------------------------------------------------------------------
            int intID = 0;

            Dictionary<string, int> dicEquipe = new Dictionary<string, int>();
            Dictionary<string, int> dicCampeonato = new Dictionary<string, int>();
            Dictionary<string, int> dicMercado = new Dictionary<string, int>();

            foreach( Aposta itm in lstAposta.OrderBy( a => a.DataPartida ) )
            {
                // Campeonato
                if( !dicCampeonato.ContainsKey( itm.Campeonato ) )
                {
                    intID = ( dicCampeonato.Values.Count > 0 ? dicCampeonato.Values.Max() : 0 ) + 1;
                    dicCampeonato.Add( itm.Campeonato, intID );
                }
                
                // Mercado
                if( !dicMercado.ContainsKey( itm.Mercado ) )
                {
                    intID = ( dicMercado.Values.Count > 0 ? dicMercado.Values.Max() : 0 ) + 1;
                    dicMercado.Add( itm.Mercado, intID );
                }

                // Equipe 1
                if( !dicEquipe.ContainsKey( itm.Equipe1 ) )
                {
                    intID = ( dicEquipe.Values.Count > 0 ? dicEquipe.Values.Max() : 0 ) + 1;
                    dicEquipe.Add( itm.Equipe1, intID );
                }

                // Equipe 2
                if( !dicEquipe.ContainsKey( itm.Equipe2 ) )
                {
                    intID = ( dicEquipe.Values.Count > 0 ? dicEquipe.Values.Max() : 0 ) + 1;
                    dicEquipe.Add( itm.Equipe2, intID );
                }

            }

            // ------------------------------------------------------------------------------------------------------------            
            // Escrevendo no SCRIPT
            // ------------------------------------------------------------------------------------------------------------

            StreamWriter oScriptBD = new StreamWriter( @"C:\Users\Alex\OneDrive\Projetos\Trade Sport ALX\script_apostas.sql", false, Encoding.UTF8 );
            StringBuilder oStrInsert = new StringBuilder();

            // Cabeçalho
            string strData = string.Format( System.Globalization.CultureInfo.CreateSpecificCulture( "pt-BR" ), "{0:dd-MMM-yyyy}", DateTime.Today );
            oScriptBD.WriteLine( escreverComentario( $"Script de importação de apostas - Data: {strData}", true ) );

            oScriptBD.WriteLine( escreverComentario( "Limpando as tabelas", false ) );

            oScriptBD.WriteLine( "TRUNCATE TABLE Entrada;" );
            oScriptBD.WriteLine( "TRUNCATE TABLE Partida;" );
            oScriptBD.WriteLine( "TRUNCATE TABLE Campeonato;" );
            oScriptBD.WriteLine( "TRUNCATE TABLE Equipe;" );
            oScriptBD.WriteLine( "TRUNCATE TABLE Mercado;\r\n" );
            
            oScriptBD.Flush();

            // Escrevendo Campeonato
            oScriptBD.WriteLine( escreverComentario( "Campeonato", false ) );

            oStrInsert.Clear();
            foreach( KeyValuePair<string, int> itm in dicCampeonato )
            {
                oStrInsert.Append( "INSERT INTO Campeonato VALUES " );
                oStrInsert.AppendFormat( "( {0}, 1, '{1}' );", itm.Value, itm.Key );
                oStrInsert.AppendLine();
            }
            oScriptBD.WriteLine( oStrInsert.ToString() );
            oScriptBD.Flush();


            // Escrevendo Mercado
            oScriptBD.WriteLine( escreverComentario( "Mercado", false ) );

            oStrInsert.Clear();
            foreach( KeyValuePair<string, int> itm in dicMercado )
            {
                oStrInsert.Append( "INSERT INTO Mercado VALUES " );
                oStrInsert.AppendFormat( "( {0}, '{1}' );", itm.Value, itm.Key );
                oStrInsert.AppendLine();
            }
            oScriptBD.WriteLine( oStrInsert.ToString() );
            oScriptBD.Flush();

            // Escrevendo Equipe
            oScriptBD.WriteLine( escreverComentario( "Equipe", false ) );

            oStrInsert.Clear();
            foreach( KeyValuePair<string, int> itm in dicEquipe )
            {
                oStrInsert.Append( "INSERT INTO Equipe VALUES " );
                oStrInsert.AppendFormat( "( {0}, 1, '{1}' );", itm.Value, itm.Key.Replace( "'", "''" ) );
                oStrInsert.AppendLine();
            }
            oScriptBD.WriteLine( oStrInsert.ToString() );
            oScriptBD.Flush();

            // Partidas
            oScriptBD.WriteLine( escreverComentario( "Partidas", true ) );
          
            oStrInsert.Clear();
            foreach( Aposta itm in lstAposta.Distinct( new CompararPartida() ).OrderBy( p => p.DataPartida ) )
            {
                oStrInsert.Append( "INSERT INTO Partida VALUES " );
                oStrInsert.AppendFormat( "( {0}, {1}, ", itm.PartidaID, dicCampeonato[ itm.Campeonato ] );
                oStrInsert.AppendFormat( "{0}, {1}, ", dicEquipe[ itm.Equipe1 ], dicEquipe[ itm.Equipe2 ] );
                oStrInsert.AppendFormat( "'{0:yyyyMMdd}' );", itm.DataPartida );
                oStrInsert.AppendLine();

            }
            oScriptBD.WriteLine( oStrInsert.ToString() );
            oScriptBD.Flush();


            // Entrada
            oScriptBD.WriteLine( escreverComentario( "Entrada", true ) );

            
            oStrInsert.Clear();
            foreach( var itm in lstAposta.GroupBy( e => e.PartidaID ) )
            {
                int intEntradaID = 0;

                foreach( var itmEntrada in itm )
                {
                    oStrInsert.Append( "INSERT INTO Entrada VALUES " );
                    oStrInsert.AppendFormat( "( {0}, {1}, {2}, ", itmEntrada.PartidaID, ++intEntradaID, dicMercado[ itmEntrada.Mercado ] );
                    oStrInsert.AppendFormat( "'{0}', {1}, {2}, '{3}', ", itmEntrada.TipoAposta.Replace( "'", "''" ), itmEntrada.ODD, itmEntrada.ValorAposta, itmEntrada.Situacao );

                    if( itmEntrada.ValorRetorno == 0 )
                        oStrInsert.Append( " NULL, NULL );" );
                    else
                        oStrInsert.AppendFormat( "{0}, NULL );", itmEntrada.ValorRetorno );

                    oStrInsert.AppendLine();

                }

            }
            oScriptBD.WriteLine( oStrInsert.ToString() );
            oScriptBD.Flush();

            oScriptBD.Close();
             
        }
        static string escreverComentario( string pstrComentario, bool pblnDestaque )
        {
            StringBuilder oScript = new StringBuilder();

            if( pblnDestaque )
                oScript.Append( $"-- { new string( '-', 80 )}\r\n" );
                
            
            oScript.Append( $"-- {pstrComentario}\r\n" );

            if( pblnDestaque )
                oScript.Append( $"-- { new string( '-', 80 )}\r\n" );


            return oScript.ToString();
        }
        #endregion

        #region Criação de script para API
        static void criarScriptLiga()
        {
            DataTableReader rd  = new BD( "tsalx" ).executarQuery( "SELECT * FROM BKP_Campeonato WHERE IDCampeonato >= 7 AND AtivoCampeonato = 0" );
            StreamWriter oScript = new StreamWriter( @"C:\Users\alexx\OneDrive\Projetos\Trade Sport ALX\BD\script_liga.sql" );
            List<string> lstLiga = new List<string>() { "Brasileirão - Série A" ,  "CONMEBOL Libertadores", "UEFA Champions League", "Premier League", "LaLiga", "UEFA Europa League" };
            int intPos = 0;

            while ( rd.Read() )
            {
                string strNome = rd[ "NomeCampeonato" ].ToString();
                
                if( strNome.Contains( "(2") )
                {
                    intPos = strNome.IndexOf( "(2" );
                    strNome = strNome.Remove( intPos, 6 )
                                     .Trim();
                }

                if ( strNome.Contains( "202" ) )
                {
                    intPos = strNome.IndexOf( "202" );
                    strNome = strNome.Remove( intPos, 4 )
                                     .Trim();
                }

                if ( strNome.Contains( "20/" ) )
                {
                    intPos = strNome.IndexOf( "20/" );
                    strNome = strNome.Remove( intPos, 5 )
                                     .Trim();
                }

                if ( strNome.Contains( "21/" ) )
                {
                    intPos = strNome.IndexOf( "21/" );
                    strNome = strNome.Remove( intPos, 5 )
                                     .Trim();
                }


                if ( !lstLiga.Contains( strNome ) )
                {
                    StringBuilder oStrInsert = new StringBuilder();

                    oStrInsert.Append( "INSERT INTO Liga VALUES ( " );
                    oStrInsert.AppendFormat( "{0}, {1}, ", rd[ "IDCampeonato" ], rd[ "IDRegiao" ] );
                    oStrInsert.AppendFormat( "'{0}', {1}, NULL );", strNome, rd[ "SelecaoCampeonato" ] );
                    
                    oScript.WriteLine( oStrInsert.ToString() );

                    lstLiga.Add( strNome );
                }
            }
            
            rd.Close();

            oScript.Flush();
            oScript.Close();

        }
        static void criarScriptCampeonato()
        {
            BD oBD = new BD( "tsalx" );

            DataTableReader rd = oBD.executarQuery( "SELECT * FROM BKP_Campeonato" );
            StreamWriter oScript = new StreamWriter( @"C:\Users\alexx\OneDrive\Projetos\Trade Sport ALX\BD\script_campeonato.sql" );
            List<CampeonatoBKP> lstCamp = new List<CampeonatoBKP>();
            Dictionary<int, int> dicCamp = new Dictionary<int, int>();
            
            dicCamp.Add( 20, 20 );
            dicCamp.Add( 46, 20 );
            dicCamp.Add( 58, 20 );
            dicCamp.Add( 59, 22 );
            dicCamp.Add( 23, 23 );
            dicCamp.Add( 25, 25 );
            dicCamp.Add( 28, 28 );
            dicCamp.Add( 29, 29 );
            dicCamp.Add( 31, 31 );
            dicCamp.Add( 33, 33 );
            dicCamp.Add( 36, 36 );
            dicCamp.Add( 50, 50 );
            dicCamp.Add( 55, 26 );

            while ( rd.Read() )
            {
                lstCamp.Add( new CampeonatoBKP() 
                { 
                    ID = rd.GetInt32( 0 ), // IDCampeonato
                    IDRegiao = rd.GetInt16( 1 ), // IDRegiao
                    Nome = rd["NomeCampeonato"].ToString(),
                    Ativo = rd.GetBoolean( 3 ), // AtivoCampeonato
                    Selecao = rd.GetBoolean( 4 ) // É seleção
                
                } );
            }

            rd.Close();
            
            rd = oBD.executarQuery( "SELECT * FROM Temporada" );
            List<Temporada> lstTemporada = new List<Temporada>();

            while( rd.Read() )
            {
                lstTemporada.Add( new Temporada() 
                { 
                    ID = rd.GetInt32( 0 ), 
                    AnoInicial = rd.GetInt16( 1 ),
                    AnoFinal = rd.GetInt16( 2 )
                } );
            }
            oScript.WriteLine( "DELETE FROM Campeonato;" );
            //lstCamp = lstCamp.Where( e => e.Nome.Contains( "/" ) ).ToList();
            //lstCamp = lstCamp.Where( e => e.ID == 20 || e.ID == 59 || e.ID == 58 ).ToList();

            foreach ( CampeonatoBKP itm in lstCamp )
            {
                string strNome = string.Empty;
                int intTempID = 0;
                int intLigaID = 0;

                if ( !itm.Nome.Contains( "/") )
                {
                    short shtAno = 0;
                    int intPos = itm.Nome.IndexOf( "20" );

                    if( intPos > -1 )
                    {
                        strNome = itm.Nome.Substring( 0, intPos - 1 );
                        shtAno = short.Parse( itm.Nome.Substring( intPos, 4 ) );
                        intTempID = lstTemporada.FirstOrDefault( t => t.AnoInicial == shtAno && t.AnoFinal == shtAno ).ID;
                        intLigaID = Convert.ToInt32( oBD.executarScalar( "select l.IDLiga from Liga l WHERE IDRegiao = {0} and l.NomeLiga = '{1}'", itm.IDRegiao, strNome ) );
                    }
                }
                else
                {
                    int intPos = itm.Nome.IndexOf( "2" );

                    if ( intPos > -1 )
                    {
                        strNome = itm.Nome.Substring( 0, intPos - 1 );
                        short shtAnoInicial = short.Parse( "20" + itm.Nome.Substring( intPos, 5 ).Split( '/' )[ 0 ] );
                        short shtAnoFinal = short.Parse( "20" + itm.Nome.Substring( intPos, 5 ).Split( '/' )[ 1 ] );
                        intTempID = lstTemporada.FirstOrDefault( t => t.AnoInicial == shtAnoInicial && t.AnoFinal == shtAnoFinal ).ID;
                        intLigaID = Convert.ToInt32( oBD.executarScalar( "SELECT l.IDLiga FROM Liga l WHERE IDRegiao = {0} and l.NomeLiga LIKE '{1}%'", itm.IDRegiao, strNome ) );
                    }
                }

                if ( intLigaID == 0 )
                {
                    if( dicCamp.ContainsKey( itm.ID ) )
                        oScript.WriteLine( $"INSERT INTO Campeonato (IDCampeonato, IDTemporada, IDLiga, AtivoCampeonato) VALUES ( {itm.ID}, {intTempID}, {dicCamp[ itm.ID ]}, {itm.Ativo} );" );

                    Console.WriteLine( "Problema na Região: {0} e Liga: {1}", itm.IDRegiao, strNome );
                }
                else
                    oScript.WriteLine( $"INSERT INTO Campeonato (IDCampeonato, IDTemporada, IDLiga, AtivoCampeonato) VALUES ( {itm.ID}, {intTempID}, {intLigaID}, {itm.Ativo} );" );
            }

            oScript.Flush();
            oScript.Close();


        }
        #endregion

        #region API do Futebol
        public static void pesquisarLiga( string pstrNomeLiga )
        {

            HttpRequestMessage _oRequest = new HttpRequestMessage();
            string strKey = System.Configuration.ConfigurationManager.AppSettings[ "ChaveAPI" ];

            _oRequest = new HttpRequestMessage();
            _oRequest.Method = HttpMethod.Get;
            _oRequest.Headers.Add( "Accept", "*/*" );
            _oRequest.Headers.Add( "User-Agent", "Thunder Client (https://www.thunderclient.com)" );
            _oRequest.Headers.Add( "x-rapidapi-host", "v3.football.api-sports.io" );
            _oRequest.Headers.Add( "x-rapidapi-key", strKey );
            _oRequest.RequestUri = new Uri( $"https://v3.football.api-sports.io/leagues?search={pstrNomeLiga}" );
            
            HttpClient oBrowse = new HttpClient();

            HttpResponseMessage oResposta = oBrowse.SendAsync( _oRequest ).Result;

            if ( oResposta.IsSuccessStatusCode )
            {
                string strDados = oResposta.Content
                                           .ReadAsStringAsync()
                                           .Result;

                //StreamReader oLer = new StreamReader( @"C:\Temp\mls.json" );
                //string strDados = string.Empty;

                //if ( oLer.Peek() > 0 )
                //    strDados = oLer.ReadToEnd();

                //oLer.Close();

                IEnumerable<JToken> oLiga = JObject.Parse( strDados )
                                                   .SelectToken( "response" );

                List<Liga> lstLiga = new List<Liga>();

                foreach( JToken itm in oLiga )
                {
                    JToken tokenLiga = itm.SelectToken( "league" );
                    JToken tokenPais = itm.SelectToken( "country" );

                    lstLiga.Add( new Liga()
                    {
                        IDLiga = tokenLiga.Value<int>( "id" ),
                        Nome = tokenLiga.Value<string>( "name" ),
                        NomePais = tokenPais.Value<string>("name")
                    } ); 
                }

            lstLiga.ForEach( l => Console.WriteLine( $"ID: {l.IDLiga}\tNome: {l.Nome}\tPais: {l.NomePais}") );
            }



        }
        public static void listarBandeiras()
        {
            HttpRequestMessage _oRequest = new HttpRequestMessage();
            
            _oRequest = new HttpRequestMessage();
            _oRequest.Method = HttpMethod.Get;
            _oRequest.Headers.Add( "Accept", "*/*" );
            _oRequest.RequestUri = new Uri( "https://flagcdn.com/en/codes.json" );

            HttpClient oBrowse = new HttpClient();

            HttpResponseMessage oResposta = oBrowse.SendAsync( _oRequest ).Result;

            if ( oResposta.IsSuccessStatusCode )
            {
                string strDados = oResposta.Content
                                          .ReadAsStringAsync()
                                          .Result;

                List<Bandeira> lstBand = new List<Bandeira>();
                JObject oSigla = JObject.Parse( strDados );
                
                foreach (var itm in oSigla )
                {
                    lstBand.Add( new Bandeira()
                    {
                        Sigla = itm.Key,
                        NomePais = itm.Value.ToString()
                    } );
                }

                lstBand.ForEach( p => Console.WriteLine( $"{p.Sigla} => {p.NomePais}" ) );
            }
        }
        public static void pesquisarEquipe( string pstrNomeEquipe )
        {
            HttpRequestMessage _oRequest = new HttpRequestMessage();
            string strKey = System.Configuration.ConfigurationManager.AppSettings[ "ChaveAPI" ];

            _oRequest = new HttpRequestMessage();
            _oRequest.Method = HttpMethod.Get;
            _oRequest.Headers.Add( "Accept", "*/*" );
            _oRequest.Headers.Add( "User-Agent", "Thunder Client (https://www.thunderclient.com)" );
            _oRequest.Headers.Add( "x-rapidapi-host", "v3.football.api-sports.io" );
            _oRequest.Headers.Add( "x-rapidapi-key", strKey );
            _oRequest.RequestUri = new Uri( $"https://v3.football.api-sports.io/teams?search={pstrNomeEquipe}" );

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpClient oBrowse = new HttpClient();

            HttpResponseMessage oResposta = oBrowse.SendAsync( _oRequest ).Result;

            if ( oResposta.IsSuccessStatusCode )
            {
                string strDados = oResposta.Content
                                           .ReadAsStringAsync()
                                           .Result;

                IEnumerable<JToken> oEquipe = JObject.Parse( strDados )
                                                     .SelectToken( "response" );

                List<Equipe> lstEquipe = new List<Equipe>();

                foreach ( JToken itm in oEquipe )
                {
                    JToken tokenEquipe = itm.SelectToken( "team" );

                    lstEquipe.Add( new Equipe()
                    {
                        ID = tokenEquipe.Value<int>( "id" ),
                        Nome = tokenEquipe.Value<string>( "name" ),
                        NomePais = tokenEquipe.Value<string>( "country" ),
                        Selecao = tokenEquipe.Value<bool>( "national" )
                    } );
                }

                lstEquipe.ForEach( l => Console.WriteLine( $"ID: {l.ID}\tNome: {l.Nome}\tPais: {l.NomePais}\tSeleção:{l.Selecao}" ) );
            }



        }
        #endregion
    }
}
