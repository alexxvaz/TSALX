using Alxware.Erro;
using System.Web.Mvc;
using TSALX.DAO;

namespace TSALX.Controllers
{
    public class RegiaoController : Controller
    {
        // GET: Regiao
        public ActionResult Index()
        {
            if( Request.QueryString[ "Mensagem" ] != null )
                ViewBag.ErroMensagem = Request.QueryString[ "Mensagem" ];

            return View( new RegiaoDAO().listar() );
        }

        [HttpGet]
        public ActionResult novo()
        {
            ModelState.Clear();
            return View( new Models.Regiao() );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult novo( Models.Regiao pobj )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    RegiaoDAO obj = new RegiaoDAO();
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
            return View( new RegiaoDAO().obter( id ) );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editar( Models.Regiao pobj )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    RegiaoDAO obj = new RegiaoDAO();
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
                new RegiaoDAO().excluir( id );
                return RedirectToAction( "index" );
            }
            catch( alxExcecao ex )
            {
               return RedirectToAction( "index", ex  );
            }

        }
        
    }
}