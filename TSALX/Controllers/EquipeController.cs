using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Alxware.Erro;

using TSALX.DAO;
using TSALX.Models;
using TSALX.Pesquisa;
using TSALX.Servico;
using TSALX.ViewModel;

namespace TSALX.Controllers
{
    public class EquipeController : Controller
    {
        private EquipeDAO _oDAO = new EquipeDAO();

        private EquipePesquisa obterPesquisa( List<Regiao> plstRegiao )
        {
            List<short> lstAno = new List<short>();

            List<Temporada> lstTemporada = new TemporadaDAO().listar();

            foreach ( Temporada itm in lstTemporada )
            {
                if ( !lstAno.Contains( itm.AnoInicial ) )
                    lstAno.Add( itm.AnoFinal );
            }
            
            return new EquipePesquisa()
            {
                ListaTemporadas = lstAno,
                ListaLiga = new LigaDAO().listar()
                                         .Where( l => l.IDAPI.HasValue )
                                         .ToList(),
                ListaRegiao = plstRegiao.Where( r => !string.IsNullOrWhiteSpace( r.Country.Trim() ) )
                                                           .ToList()
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


            return View( new EquipePG() 
            { 
                Titulo = "Time",
                ListaEquipe = new EquipeDAO().listar(),
                TextoMSG = strMensagem,
                TipoMSG = Convert.ToInt16( enuTipo )

            } );
        }

        [HttpGet]
        public ActionResult novo()
        {
            ModelState.Clear();

            List<Regiao> lstRegiao = new RegiaoDAO().listar();

            return View( new EquipeVM() { 
                Titulo = "Novo Time",
                ListaRegiao = lstRegiao,
                equipe = new Equipe(),
                Pesquisa = obterPesquisa( lstRegiao )

            } );
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult novo( EquipeVM pobj )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    _oDAO.salvar( pobj.equipe );
                    return RedirectToAction( "Index" );
                }
                else
                    return View();
            }
            catch( alxExcecao ex )
            {
                List<Regiao> lstRegiao = new RegiaoDAO().listar();

                return View( new EquipeVM()
                {
                    Titulo = "Novo Time",
                    ListaRegiao = lstRegiao,
                    Pesquisa = obterPesquisa( lstRegiao ),
                    TextoMSG = ex.Mensagem,
                    TipoMSG = Convert.ToInt16( ex.Tipo )
                } );
            }
        }
        
        [HttpGet]
        public ActionResult editar( int id )
        {
            ModelState.Clear();

            List<Regiao> lstRegiao = new RegiaoDAO().listar();

            return View( new EquipeVM()
            {
                Titulo = "Editar Time",
                equipe = new EquipeDAO().obter( id ),
                ListaRegiao = lstRegiao,
                Pesquisa = obterPesquisa( lstRegiao )
            } );

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editar(EquipeVM pobj )
        {
            try
            {
                if ( ModelState.IsValid )
                {
                    _oDAO.salvar( pobj.equipe );
                    return RedirectToAction( "index" );
                }
                else
                    return View();
            }
            catch ( alxExcecao ex )
            {
                List<Regiao> lstRegiao = new RegiaoDAO().listar();

                return View( new EquipeVM()
                {
                    Titulo = "Editar Time",
                    equipe = pobj.equipe,
                    ListaRegiao = lstRegiao,
                    Pesquisa = obterPesquisa( lstRegiao ),
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
            catch( alxExcecao ex )
            {
                return RedirectToAction( "index", ex );
            }
        }
        

        public JsonResult pesquisarEquipe( string nome )
        {
            APIFutebol apiEquipe = new APIFutebol();
            List<Models.API.Equipe> lstRet = apiEquipe.pesquisarEquipe( nome );
            return Json( lstRet, JsonRequestBehavior.AllowGet );
        }
        public JsonResult pesquisarEquipeLiga( string pais, short ano, int liga )
        {
            APIFutebol apiEquipe = new APIFutebol();
            List<Models.API.Equipe> lstRet = apiEquipe.pesquisarEquipe( pais, ano, liga );
            return Json( lstRet, JsonRequestBehavior.AllowGet );
        }
    }
}