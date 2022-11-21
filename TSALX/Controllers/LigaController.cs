using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Alxware.Erro;

using TSALX.DAO;
using TSALX.Models;
using TSALX.Pesquisa;
using TSALX.Servico;
using TSALX.ViewModel;

namespace TSALX.Controllers
{
    public class LigaController : Controller
    {
        private LigaDAO _oDAO = new LigaDAO();
        private RegiaoDAO _oRegiaoDAO = new RegiaoDAO();

        private LigaPesquisa obterPesquisa()
        {
            List<short> lstAno = new List<short>();

            List<Temporada> lstTemporada = new TemporadaDAO().listar();

            foreach ( Temporada itm in lstTemporada )
            {
                if ( !lstAno.Contains( itm.AnoInicial ) )
                    lstAno.Add( itm.AnoFinal );
            }

            return new LigaPesquisa()
            {
                ListaRegiao = _oRegiaoDAO.listar(),
                ListaTemporadas = lstAno
            };
        }

        public ActionResult Index()
        {
            string strMensagem = string.Empty;
            ErroTipo enuTipo = ErroTipo.Processo;

            if ( Request.QueryString[ "Mensagem" ] != null )
            {
                strMensagem = Request.QueryString[ "Mensagem" ];

                switch ( Request.QueryString[ "Tipo" ] )
                {
                    case "Processo":
                        enuTipo = ErroTipo.Processo;
                        break;
                    case "Dados":
                        enuTipo = ErroTipo.Dados;
                        break;
                    case "Sistema":
                        enuTipo = ErroTipo.Sistema;
                        break;
                }
            }

            return View(new LigaPG() 
            { 
                Titulo = "Liga",
                TextoMSG = strMensagem,
                TipoMSG = Convert.ToInt16( enuTipo ),
                ListaLiga = _oDAO.listar()
            } );
        }

        public ActionResult nova()
        {
            ModelState.Clear();

            return View( new LigaVM() 
            { 
                Titulo = "Nova Liga", 
                liga = new Liga(),
                Pesquisa = obterPesquisa(),
                ListaRegiao = _oRegiaoDAO.listar()
            
            } );
        }
        [HttpPost]
        public ActionResult nova( LigaVM pobj )
        {
            try
            {
                if ( ModelState.IsValid )
                {
                    _oDAO.salvar( pobj.liga );
                    return RedirectToAction( "Index" );
                }
                else
                    return View();
            }
            catch( alxExcecao ex )
            {
                return View( new LigaVM()
                {
                    Titulo = "Nova Liga",
                    liga = new Liga(),
                    Pesquisa = obterPesquisa(),
                    ListaRegiao = _oRegiaoDAO.listar(),
                    TextoMSG = ex.Mensagem,
                    TipoMSG = Convert.ToInt16( ex.Tipo )

                } );
            }
            
        }
    
        public ActionResult editar( int id )
        {
            ModelState.Clear();

            return View( new LigaVM()
            {
                Titulo = "Editar Liga",
                liga = _oDAO.obter( id ),
                Pesquisa = obterPesquisa(),
                ListaRegiao = _oRegiaoDAO.listar()

            } );
        }
        [HttpPost]
        public ActionResult editar( LigaVM pobj )
        {
            try
            {
                if ( ModelState.IsValid )
                {
                    _oDAO.salvar( pobj.liga );
                    return RedirectToAction( "Index" );
                }
                else
                    return View();
            }
            catch ( alxExcecao ex )
            {
                return View( new LigaVM()
                {
                    Titulo = "Editar Liga",
                    liga = pobj.liga,
                    Pesquisa = obterPesquisa(),
                    ListaRegiao = _oRegiaoDAO.listar(),
                    TextoMSG = ex.Mensagem,
                    TipoMSG = Convert.ToInt16( ex.Tipo )

                } );
            }

        }
        public ActionResult excluir( int id )
        {
            try
            {
                _oDAO.excluir( id );
                return RedirectToAction( "index" );
            }
            catch ( alxExcecao ex )
            {
                return RedirectToAction( "index", ex );
            }
        }

        public JsonResult pesquisarLiga( string nome )
        {
            APIFutebol apiLiga = new APIFutebol();
            List<Models.API.Liga> lstRet = apiLiga.pesquisarLiga( nome );
            return Json( lstRet, JsonRequestBehavior.AllowGet );
        }
        public JsonResult pesquisarLigaTemporada( short ano, string pais )
        {
            APIFutebol apiLiga = new APIFutebol();
            List<Models.API.Liga> lstRet = apiLiga.pesquisarLiga( ano, pais );
            return Json( lstRet, JsonRequestBehavior.AllowGet );
        }
    }
}