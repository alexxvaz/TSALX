using Alxware.Erro;
using System.Web.Mvc;
using TSALX.DAO;

namespace TSALX.Controllers
{
    public class MercadoController : Controller
    {
        // GET: Mercado
        public ActionResult Index()
        {
            if( Request.QueryString[ "Mensagem" ] != null )
                ViewBag.ErroMensagem = Request.QueryString[ "Mensagem" ];

            return View( new MercadoDAO().listar() );
        }

        [HttpGet]
        public ActionResult novo()
        {
            ModelState.Clear();
            return View( new Models.Mercado() );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult novo( Models.Mercado pobj )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    MercadoDAO obj = new MercadoDAO();
                    obj.salvar( pobj );

                    return RedirectToAction( "Index" );
                }
                else
                    return View();
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
                return View();
            }
        }

        [HttpGet]
        public ActionResult editar( int id )
        {
            return View( new MercadoDAO().obter( id ) ); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editar( Models.Mercado pobj )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    MercadoDAO obj = new MercadoDAO();
                    obj.salvar( pobj );

                    return RedirectToAction( "index" );
                }
                else
                    return View();

            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
                return View();
            }

        }

        public ActionResult excluir( int id )
        {
            try
            {
                new MercadoDAO().excluir( id );
                return RedirectToAction( "index" );
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
                return RedirectToAction( "index", ex );
            }
        }
    }
}