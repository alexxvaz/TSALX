using System;
using System.Collections.Generic;
using System.Net.Http;

using Newtonsoft.Json.Linq;

using TSALX.Models.API;

namespace TSALX.Servico
{

    public class APIFutebol
    {
        private HttpRequestMessage _oRequest;

        public APIFutebol()
        {
            string strKey = System.Configuration.ConfigurationManager.AppSettings[ "ChaveAPI" ];

            _oRequest = new HttpRequestMessage();
            _oRequest.Method = HttpMethod.Get;
            _oRequest.Headers.Add( "Accept", "*/*" );
            _oRequest.Headers.Add( "User-Agent", "TSALX" );
            _oRequest.Headers.Add( "x-rapidapi-host", "v3.football.api-sports.io" );
            _oRequest.Headers.Add( "x-rapidapi-key", strKey );
        }
        
        public List<Bandeira> listarBandeira()
        {
            List<Bandeira> lstRet = null;

            _oRequest.Headers.Remove( "x-rapidapi-host" );
            _oRequest.Headers.Remove( "x-rapidapi-key" );
            _oRequest.RequestUri = new Uri( "https://flagcdn.com/en/codes.json" );
            
            HttpClient oBrowse = new HttpClient();

            HttpResponseMessage oResposta = oBrowse.SendAsync( _oRequest ).Result;

            if ( oResposta.IsSuccessStatusCode )
            {
                string strDados = oResposta.Content
                                          .ReadAsStringAsync()
                                          .Result;
                
                lstRet = new List<Bandeira>();
                JObject oSigla = JObject.Parse( strDados );

                foreach( var itm in oSigla )
                {
                    lstRet.Add( new Bandeira() 
                    { 
                        Sigla = itm.Key.ToUpper(),
                        NomePais = itm.ToString()
                    } );
                }

            }

            return lstRet;
        }

        #region Liga
        public List<Liga> pesquisarLiga( string pstrNomeLiga ) 
        {
            List<Liga> lstRet = null;
            _oRequest.RequestUri = new Uri( $"https://v3.football.api-sports.io/leagues?serach={pstrNomeLiga}" );
            HttpClient oBrowse = new HttpClient();

            HttpResponseMessage oResposta = oBrowse.SendAsync( _oRequest ).Result;

            if ( oResposta.IsSuccessStatusCode )
            {
                string strDados = oResposta.Content
                                           .ReadAsStringAsync()
                                           .Result;

                IEnumerable<JToken> oLiga = JObject.Parse( strDados )
                                                   .SelectToken( "response" );

                lstRet = new List<Liga>();

                foreach ( JToken itm in oLiga )
                {
                    JToken tokenLiga = itm.SelectToken( "league" );
                    JToken tokenPais = itm.SelectToken( "country" );

                    lstRet.Add( new Liga()
                    {
                        ID = tokenLiga.Value<int>( "id" ),
                        Nome = tokenLiga.Value<string>( "name" ),
                        NomePais = tokenPais.Value<string>( "name" )
                    } );
                }
            }

            return lstRet;
        }
        #endregion

        #region Equipe
        private List<Equipe> listarEquipe( IEnumerable<JToken> tokens )
        {
            List<Equipe> lstRet = new List<Equipe>();

            foreach ( JToken itm in tokens )
            {
                JToken tokenEquipe = itm.SelectToken( "team" );

                lstRet.Add( new Equipe()
                {
                    ID = tokenEquipe.Value<int>( "id" ),
                    Nome = tokenEquipe.Value<string>( "name" ),
                    NomePais = tokenEquipe.Value<string>( "country" ),
                    Selecao = tokenEquipe.Value<bool>( "national" )
                } );
            }

            return lstRet;
        }
        public List<Equipe> pesquisarEquipe( string pstrNomeEquipe )
        {
            List<Equipe> lstRet = null;
            _oRequest.RequestUri = new Uri( $"https://v3.football.api-sports.io/teams?search={pstrNomeEquipe}" );
            HttpClient oBrowse = new HttpClient();

            HttpResponseMessage oResposta = oBrowse.SendAsync( _oRequest ).Result;

            if ( oResposta.IsSuccessStatusCode )
            {
                string strDados = oResposta.Content
                                           .ReadAsStringAsync()
                                           .Result;

                IEnumerable<JToken> oEquipe = JObject.Parse( strDados )
                                                   .SelectToken( "response" );

                lstRet = this.listarEquipe( oEquipe );
            }

            return lstRet;
        }
        public List<Equipe> pesquisarEquipe( string pstrPais, short pshtTemporada, int pintLiga )
        {
            List<Equipe> lstRet = null;
            _oRequest.RequestUri = new Uri( $"https://v3.football.api-sports.io/teams?country={pstrPais}&league={pintLiga}&season={pshtTemporada}" );
            HttpClient oBrowse = new HttpClient();

            HttpResponseMessage oResposta = oBrowse.SendAsync( _oRequest ).Result;

            if ( oResposta.IsSuccessStatusCode )
            {
                string strDados = oResposta.Content
                                           .ReadAsStringAsync()
                                           .Result;

                IEnumerable<JToken> oEquipe = JObject.Parse( strDados )
                                                   .SelectToken( "response" );

                lstRet = this.listarEquipe( oEquipe );
            }

            return lstRet;
        }
        public Equipe obterEquipe( int pintID )
        {
            Equipe oRet = null;
            _oRequest.RequestUri = new Uri( $"https://v3.football.api-sports.io/teams?id={pintID}" );
            HttpClient oBrowse = new HttpClient();

            HttpResponseMessage oResposta = oBrowse.SendAsync( _oRequest ).Result;

            if ( oResposta.IsSuccessStatusCode )
            {
                string strDados = oResposta.Content
                                           .ReadAsStringAsync()
                                           .Result;

                IEnumerable<JToken> oEquipe = JObject.Parse( strDados )
                                                   .SelectToken( "response" );

                oRet = this.listarEquipe( oEquipe )[ 0 ];
            }

            return oRet;
        }
        #endregion

    }
}