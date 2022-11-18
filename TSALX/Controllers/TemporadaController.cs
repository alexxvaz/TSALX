using System;
using System.Web.Mvc;

using Alxware.Erro;

using TSALX.DAO;
using TSALX.Models;
using TSALX.ViewModel;

namespace TSALX.Controllers
{
    public class TemporadaController : Controller
    {
        private TemporadaDAO _oDAO = new TemporadaDAO();

        public ActionResult Index(int? id)
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

            Temporada oTemp = new Temporada()
            {
                AnoInicial = (short) DateTime.Today.Year,
                AnoFinal = (short) DateTime.Today.AddYears( 1 ).Year
            };

            if ( id.HasValue )
                oTemp = _oDAO.obter( id.Value );
            
            return View( new TemporadaVM()
            {
                Titulo = "Temporada",
                ListaTemporada = _oDAO.listar(),
                TextoMSG = strMensagem,
                TipoMSG = Convert.ToInt16( enuTipo ),
                temporada = oTemp
            } );
            
        }


        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult Index( TemporadaVM pForm )
        {
            try
            {
                if ( ModelState.IsValid )
                {
                    _oDAO.salvar( pForm.temporada );
                    return RedirectToAction( "index" );
                }
                else
                {
                    return View( new TemporadaVM()
                    {
                        Titulo = "Temporada",
                        ListaTemporada = _oDAO.listar(),
                        temporada = new Temporada()
                        {
                            AnoInicial = (short) DateTime.Today.Year,
                            AnoFinal = (short) DateTime.Today.AddYears( 1 ).Year
                        }
                    } );
                }
            }
            catch( alxExcecao ex )
            {
                return View( new TemporadaVM()
                {
                    Titulo = "Temporada",
                    ListaTemporada = _oDAO.listar(),
                    TipoMSG = Convert.ToInt16( ex.Tipo ), 
                    TextoMSG = ex.Mensagem,
                    temporada = new Temporada()
                    {
                        AnoInicial = (short) DateTime.Today.Year,
                        AnoFinal = (short) DateTime.Today.AddYears( 1 ).Year
                    }

                } );
            }
        }
        public ActionResult editar( int id )
        {
            return RedirectToAction( "Index", new { id = id } );
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
    }
}