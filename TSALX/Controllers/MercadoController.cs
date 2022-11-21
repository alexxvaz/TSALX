using Alxware.Erro;
using System;
using System.Web.Mvc;
using TSALX.DAO;
using TSALX.Models;
using TSALX.ViewModel;

namespace TSALX.Controllers
{
    public class MercadoController : Controller
    {
        private MercadoDAO _oDAO = new MercadoDAO();

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

            return View( new MercadoPG() 
            { 
                Titulo = "Mercado", 
                ListaMercado = _oDAO.listar(),
                TextoMSG = strMensagem,
                TipoMSG = Convert.ToInt16( enuTipo )
            } );
        }

        [HttpGet]
        public ActionResult novo()
        {
            ModelState.Clear();
            return View( new MercadoVM() 
            { 
                Titulo= "Novo Mercado", 
                mercado = new Mercado()
            } );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult novo( MercadoVM pobj )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    _oDAO.salvar( pobj.mercado );
                    return RedirectToAction( "Index" );
                }
                else
                    return View();
            }
            catch( alxExcecao ex )
            {
                return View( new MercadoVM()
                {
                    Titulo = "Novo Mercado",
                    mercado = new Mercado(),
                    TextoMSG = ex.Mensagem,
                    TipoMSG = Convert.ToInt16( ex.Tipo )
                } );
            }
        }

        [HttpGet]
        public ActionResult editar( int id )
        {
            return View( new MercadoVM()
            {
                Titulo = "Editar Mercado",
                mercado = _oDAO.obter( id )
            } ); ;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editar( MercadoVM pobj )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    _oDAO.salvar( pobj.mercado );
                    return RedirectToAction( "index" );
                }
                else
                    return View();

            }
            catch( alxExcecao ex )
            {
                return View( new MercadoVM()
                {
                    Titulo = "Editar Mercado",
                    mercado = pobj.mercado,
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
    }
}