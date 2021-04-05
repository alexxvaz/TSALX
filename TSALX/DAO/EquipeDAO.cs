using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Alxware.BD;
using Alxware.Erro;

namespace TSALX.DAO
{
    public class EquipeDAO
    {
        private BD _oBD = null;

        public EquipeDAO()
        {
            _oBD = new BD( Util.ConexaoBD );
        }

        public List<Models.EquipeLista> listar()
        {
            List<Models.EquipeLista> lst = new List<Models.EquipeLista>();

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT IDEquipe, NomeEquipe, NomeRegiao, SiglaRegiao FROM Regiao r INNER JOIN Equipe e ON r.IDRegiao = e.IDRegiao ORDER BY NomeRegiao, NomeEquipe" );

                while( rd.Read() )
                {
                    lst.Add( new Models.EquipeLista()
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
                lst = null;
            }

            return lst;
        }
        public void salvar( Models.Equipe pobjEquipe )
        {
            try
            {
                StringBuilder oStrDML = new StringBuilder();

                if( pobjEquipe.IDEquipe == 0 ) // Inserir
                {
                    int intProximoID = Util.informarProximoID( "Equipe", "IDEquipe" );

                    if( intProximoID > 0 )
                    {
                        oStrDML.Append( "INSERT INTO Equipe " );
                        oStrDML.AppendFormat( "VALUES ( {0}, {1}, '{2}' )", intProximoID, pobjEquipe.IDRegiao, pobjEquipe.Nome.Replace( "'", "''" ) );
                    }
                    else
                        throw new alxExcecao( "Não foi informado o próximo IDEquipe", ErroTipo.Dados );
                }
                else // Alterar
                {
                    oStrDML.Append( "UPDATE Equipe SET " );
                    oStrDML.AppendFormat( "NomeEquipe = '{0}' ", pobjEquipe.Nome.Replace( "'", "''" ) );
                    oStrDML.AppendFormat( ", IDRegiao = {0}", pobjEquipe.IDRegiao );
                    oStrDML.AppendFormat( " WHERE IDEquipe = {0}", pobjEquipe.IDEquipe );
                }

                _oBD.executarDML( oStrDML.ToString() );
            }
            catch( alxExcecao ex )
            {
                if( ex.Mensagem.Contains( "UK_Equipe_Nome" ) )
                    throw new alxExcecao( "O nome da equipe '{0}' já foi cadastrada para esta região", pobjEquipe.Nome );
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
                throw new alxExcecao( "Problema ao salvar a Equipe" );
            }
        }
        public void excluir( int pintID )
        {

            try
            {
                _oBD.executarDML( "DELETE FROM Equipe WHERE IDEquipe = {0}", pintID );
            }
            catch( alxExcecao ex )
            {
                new TratamentoErro( ex ).tratarErro();
                throw ex;
            }
        }

        public Models.Equipe obter( int pintID )
        {
            Models.Equipe oRet = null;

            try
            {
                DataTableReader rd = _oBD.executarQuery( "SELECT IDEquipe, IDRegiao, NomeEquipe FROM Equipe WHERE IDEquipe = {0}", pintID );

                if( rd.Read() )
                {
                    oRet = new Models.Equipe()
                    {
                        IDEquipe = rd.GetInt32( 0 ),
                        IDRegiao = rd.GetInt16( 1 ),
                        Nome = rd[ 2 ].ToString()
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