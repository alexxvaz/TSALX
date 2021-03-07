using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Alxware.BD;
using Alxware.Erro;

namespace TSALX.DAO
{
    public class TemporadaDAO
    {
        private BD _oBD;
        private int _intCampeonato;

        public TemporadaDAO( int pintCampeonato )
        {
            _intCampeonato = pintCampeonato;
            _oBD = new BD( Util.ConexaoBD );
        }

        private List<int> listarEquipes()
        {
            List<int> lstRet = null;

            try
            {
                
                DataTableReader rd = _oBD.executarQuery( "SELECT IDEquipe FROM Temporada WHERE IDCampeonato = {0}", _intCampeonato );
                lstRet = new List<int>();

                while( rd.Read() )
                    lstRet.Add( rd.GetInt32( 0 ) );
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                lstRet = null;
            }

            return lstRet;
        }

        public Dictionary<int, List<Models.TemporadaEquipe>> listar()
        {
            
            Dictionary<int, List<Models.TemporadaEquipe>> dicRet = null;

            try
            {
                StringBuilder oStrQuery = new StringBuilder();

                oStrQuery.Append( "SELECT e.IDEquipe, NomeEquipe, SiglaRegiao, NomeRegiao " );
                oStrQuery.Append( "  FROM Regiao r " );
                oStrQuery.Append( " INNER JOIN Equipe e ON R.IDRegiao = E.IDRegiao " );
                oStrQuery.Append( " ORDER BY NomeEquipe " );

                DataTableReader rd = _oBD.executarQuery( oStrQuery.ToString() );

                
                List<int> lstTemporada = this.listarEquipes();
                List<Models.TemporadaEquipe> lstEquipe = new List<Models.TemporadaEquipe>();

                while( rd.Read() )
                {
                    lstEquipe.Add( new Models.TemporadaEquipe()
                    {
                        IDEquipe = rd.GetInt32( 0 ), // IDEquipe
                        NomeEquipe = rd[ "NomeEquipe" ].ToString(),
                        Bandeira = Util.informarBandeira( rd[ "SiglaRegiao" ].ToString() ),
                        Participa = lstTemporada.Contains( rd.GetInt32( 0 ) ),
                        NomeRegiao = rd["NomeRegiao" ].ToString()
                    } );
                }

                const int COLUNAS = 4;
                dicRet = new Dictionary<int, List<Models.TemporadaEquipe>>();
                int intQtdMaxEquipeColuna = lstEquipe.Count / COLUNAS;
                int intPos = 0;

                if( ( lstEquipe.Count() % COLUNAS ) != 0 ) intQtdMaxEquipeColuna++;

                for( int intColuna = 1; intColuna <= COLUNAS; intColuna++ )
                {
                    dicRet.Add( intColuna, lstEquipe.GetRange( intPos, intQtdMaxEquipeColuna ) );

                    intPos += intQtdMaxEquipeColuna;

                    if( ( intPos + intQtdMaxEquipeColuna ) > lstEquipe.Count )
                        intQtdMaxEquipeColuna = lstEquipe.Count - intPos;
                    
                }

            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                dicRet = null;
            }

            return dicRet;
        }

        public bool gravar( IEnumerable<string> plstEquipe )
        {
            try
            {
                List<string> lstDML = new List<string>();

                lstDML.Add( String.Format( "DELETE FROM Temporada WHERE IDCampeonato = {0}", _intCampeonato ) );

                foreach( string intEquipe in plstEquipe )
                    lstDML.Add( string.Format( "INSERT INTO Temporada VALUES ( {0}, {1} )", _intCampeonato, intEquipe ) );

                _oBD.executarDML( lstDML );

                return true;

            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                return false;
            }

        }
    }
}