using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        #region bandeira
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

                foreach ( var itm in oSigla )
                {
                    lstRet.Add( new Bandeira()
                    {
                        Sigla = itm.Key.ToUpper(),
                        NomePais = itm.Value.ToString()
                    } );
                }

            }

            return lstRet;
        }
        public string obterNomeBandeira( string pstrSigla )
        {
            return listarBandeira().Where( b => b.Sigla == pstrSigla )
                                   .FirstOrDefault()
                                   .NomePais;
        }
        #endregion
        #region Liga
        public List<Liga> pesquisarLiga( string pstrNomeLiga )
        {
            List<Liga> lstRet = null;
            _oRequest.RequestUri = new Uri( $"https://v3.football.api-sports.io/leagues?search={pstrNomeLiga}" );
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
        public List<Liga> pesquisarLiga( short pshtAno, string pstrPais )
        {
            List<Liga> lstRet = null;
            _oRequest.RequestUri = new Uri( $"https://v3.football.api-sports.io/leagues?country={pstrPais}&season={pshtAno}" );
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

            return lstRet.OrderBy( e => e.Nome )
                         .ToList();
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
        public List<Equipe> pesquisarEquipe( string pstrPais, short pshtAno, int pintLiga )
        {
            List<Equipe> lstRet = null;
            _oRequest.RequestUri = new Uri( $"https://v3.football.api-sports.io/teams?country={pstrPais}&league={pintLiga}&season={pshtAno}" );
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