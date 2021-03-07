using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace ImportarTSALX
{
    class Program
    {
        static void Main( string[] args )
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

        public class Aposta 
        {
            public DateTime DataPartida { get; set; }
            public string Campeonato { get; set; }
            public string Equipe1 { get; set; }
            public string Equipe2 { get; set; }
            public string Mercado { get; set; }
            public string TipoAposta { get; set; }
            public decimal ODD { get; set; }
            public decimal ValorAposta { get; set; }
            public char Situacao { get; set; }
            public decimal ValorRetorno { get; set; }
            public long PartidaID { get; set; }

        }

        public class CompararPartida : IEqualityComparer<Aposta>
        {
            public bool Equals( Aposta x, Aposta y )
            {
                return x.PartidaID == y.PartidaID;
            }

            public int GetHashCode( Aposta obj )
            {
                return Convert.ToInt32( obj.PartidaID );
            }
        }
    }
}
