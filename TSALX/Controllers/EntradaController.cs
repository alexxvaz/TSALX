using System.Linq;
using System.Web.Mvc;

using Alxware.Erro;
using TSALX.DAO;

namespace TSALX.Controllers
{
    public class EntradaController : Controller
    {
        // GET: Entrada
        public ActionResult Index( long ID, long IDEntrada )
        {
            Models.Entrada objEnt = new EntradaDAO( ID ).obterEntrada( IDEntrada );
            return View( objEnt );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index( long ID, Models.Entrada pobjEntrada )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    long lngPartida = long.Parse( Request.Url.Segments.LastOrDefault() );
                    new EntradaDAO( lngPartida ).salvar( pobjEntrada );
                    return RedirectToAction( "Editar", "Partida", new { ID = lngPartida } );
                }
                else
                {
                    Models.Entrada objEnt = new EntradaDAO( ID ).obterEntrada( 0 );
                    return View( objEnt );
                }
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
                return View( pobjEntrada );
            }
        }

        
    }
}