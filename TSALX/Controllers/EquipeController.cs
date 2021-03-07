using Alxware.Erro;
using System.Web.Mvc;
using TSALX.DAO;

namespace TSALX.Controllers
{
    public class EquipeController : Controller
    {
        // GET: Equipe
        public ActionResult Index()
        {
            if( Request.QueryString[ "Mensagem" ] != null )
                ViewBag.ErroMensagem = Request.QueryString[ "Mensagem" ];

            return View( new EquipeDAO().listar() );
        }

        [HttpGet]
        public ActionResult novo()
        {
            ModelState.Clear();

            Models.Equipe objEquipe = new Models.Equipe();
            objEquipe.ListaRegiao = new RegiaoDAO().listar();

            return View( objEquipe );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult novo( Models.Equipe pobj )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    EquipeDAO obj = new EquipeDAO();
                    obj.salvar( pobj );

                    return RedirectToAction( "Index" );
                }
                else
                    return View();
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
                pobj.ListaRegiao = new RegiaoDAO().listar();
                return View( pobj );
            }
        }
        
        [HttpGet]
        public ActionResult editar( int id )
        {
            ModelState.Clear();

            Models.Equipe objEquipe = new EquipeDAO().obter( id );
            objEquipe.ListaRegiao = new RegiaoDAO().listar();

            return View( objEquipe );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editar( Models.Equipe pobj )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    EquipeDAO obj = new EquipeDAO();
                    obj.salvar( pobj );

                    return RedirectToAction( "index" );
                }
                else
                    return View();

            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
                pobj.ListaRegiao = new RegiaoDAO().listar();
                return View( pobj );
            }

        }

        public ActionResult excluir( int id )
        {
            try
            {
                new EquipeDAO().excluir( id );
                return RedirectToAction( "index" );
            }
            catch( alxExcecao ex )
            {
                return RedirectToAction( "index", ex );
            }
        }
        
    }
}