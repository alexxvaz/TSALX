using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Alxware.BD;
using Alxware.Erro;

using TSALX.Servico;

namespace TSALX.DAO
{
    public class TemporadaDAO
    {
        private BD _oBD;
        private int _intCampeonato;
        private bool _blnSelecao;

        public TemporadaDAO( int pintCampeonato )
        {
            _intCampeonato = pintCampeonato;
            _oBD = new BD( Util.ConexaoBD );
            _blnSelecao = Convert.ToBoolean( _oBD.executarScalar( "SELECT SelecaoCampeonato FROM Campeonato WHERE IDCampeonato = {0}", _intCampeonato ) );
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
            const short EUA_MLS = 44;

            Dictionary<int, List<Models.TemporadaEquipe>> dicRet = null;

            try
            {
                StringBuilder oStrQuery = new StringBuilder();
               
                oStrQuery.Append( "SELECT r.IDRegiao " );
                oStrQuery.Append( "  FROM Regiao r " );
                oStrQuery.Append( " INNER JOIN Campeonato c ON r.IDRegiao = c.IDRegiao " );
                oStrQuery.Append( " WHERE ( SiglaRegiao IS NOT NULL AND SiglaRegiao NOT IN ('EU', 'AM-SUL', 'AM-NOR') ) " );
                oStrQuery.AppendFormat(" AND IDCampeonato = {0} ", _intCampeonato );

                short shtRegiaoID = Convert.ToInt16( _oBD.executarScalar( oStrQuery.ToString() ) );
                
                // Listar os times
                oStrQuery.Clear();

                if ( shtRegiaoID == EUA_MLS ) // +1 Região (EUA e Canadá)
                {
                    oStrQuery.Append( "SELECT e.IDEquipe, NomeEquipe, SiglaRegiao, r.IDRegiao, NomeRegiao,  " );
                    oStrQuery.Append( " (SELECT IDEquipe FROM Regiao r WHERE IDEquipe = e.IDEquipe) AS Selecao " );
                    oStrQuery.Append( "  FROM Regiao r " );
                    oStrQuery.Append( " INNER JOIN Equipe e ON r.IDRegiao = e.IDRegiao " );
                    oStrQuery.Append( " WHERE r.IDRegiao IN (44, 45) " );
                    oStrQuery.Append( " ORDER BY NomeEquipe " );
                }
                else if ( shtRegiaoID > 0 ) 
                {
                    oStrQuery.Append( "SELECT e.IDEquipe, NomeEquipe, SiglaRegiao, r.IDRegiao, NomeRegiao,  " );
                    oStrQuery.Append( " (SELECT IDEquipe FROM Regiao r WHERE IDEquipe = e.IDEquipe) AS Selecao " );
                    oStrQuery.Append( "  FROM Regiao r " );
                    oStrQuery.Append( " INNER JOIN Equipe e ON r.IDRegiao = e.IDRegiao " );
                    oStrQuery.AppendFormat( " WHERE r.IDRegiao = {0} ", shtRegiaoID );
                    oStrQuery.Append( " ORDER BY NomeEquipe " );
                }
                else if ( _blnSelecao )
                {
                    oStrQuery.Append( "SELECT e.IDEquipe, NomeEquipe, SiglaRegiao, r.IDRegiao, NomeRegiao, 1 AS Selecao " );
                    oStrQuery.Append( "  FROM Regiao r " );
                    oStrQuery.Append( " INNER JOIN Equipe e ON e.IDEquipe = r.IDEquipe" );
                    oStrQuery.Append( " ORDER BY NomeEquipe " );
                }
                else
                {
                    oStrQuery.Append( "SELECT e.IDEquipe, NomeEquipe, SiglaRegiao, r.IDRegiao, NomeRegiao,  " );
                    oStrQuery.Append( " (SELECT IDEquipe FROM Regiao r WHERE IDEquipe = e.IDEquipe) AS Selecao " );
                    oStrQuery.Append( "  FROM Regiao r " );
                    oStrQuery.Append( " INNER JOIN Equipe e ON r.IDRegiao = e.IDRegiao " );
                    oStrQuery.Append( " ORDER BY NomeEquipe " );
                }

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
                        Selecao = !rd.IsDBNull( 5 ) // Seleção

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