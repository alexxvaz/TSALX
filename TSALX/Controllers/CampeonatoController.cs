using System.Web.Mvc;
using Alxware.Erro;
using TSALX.DAO;

namespace TSALX.Controllers
{
    public class CampeonatoController : Controller
    {
        // GET: Campeonato
        public ActionResult Index()
        {
            if( Request.QueryString[ "Mensagem" ] != null )
                ViewBag.ErroMensagem = Request.QueryString[ "Mensagem" ];

            return View( new CampeonatoDAO().listar() );
        }

        [HttpGet]
        public ActionResult novo()
        {
            ModelState.Clear();

            Models.Campeonato objCampeonato = new Models.Campeonato()
            {
                ListaRegiao = new RegiaoDAO().listar(),
                Ativo = true
            };
            
            return View( objCampeonato );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult novo( Models.Campeonato pobj )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    CampeonatoDAO obj = new CampeonatoDAO();
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
            
            Models.Campeonato objCampeonato = new CampeonatoDAO().obter( id );
            objCampeonato.ListaRegiao = new RegiaoDAO().listar();
            
            return View( objCampeonato );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editar( Models.Campeonato pobj )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    CampeonatoDAO obj = new CampeonatoDAO();
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
                new CampeonatoDAO().excluir( id );
                return RedirectToAction( "index" );
            }
            catch( alxExcecao ex )
            {
                return RedirectToAction( "index", ex );
            }
        }
    }
}