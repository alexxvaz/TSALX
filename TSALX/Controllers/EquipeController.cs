using Alxware.Erro;
using System.Collections.Generic;
using System.Web.Mvc;
using TSALX.DAO;
using System.Linq;
using TSALX.Models.Regiao;

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

            Models.EquipePagina objEquipe = new Models.EquipePagina();
            objEquipe.ListaRegiao = new RegiaoDAO().listar();

            return View( objEquipe.equipe );
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
                //pobj.ListaRegiao = new RegiaoDAO().listar();
                return View( pobj );
            }
        }
        
        [HttpGet]
        public ActionResult editar( int id )
        {
            ModelState.Clear();

            List<ItemRegiao> lstRegiao = new RegiaoDAO().listar();
                                                          
            Models.EquipePesquisa oPesquisa = new Models.EquipePesquisa()
            {
                ListaLiga = new List<Models.Campeonato>(),
                ListaTemporadas = new List<Models.Temporada>(),
                ListaRegiao = lstRegiao.Where( r => !string.IsNullOrWhiteSpace( r.Country.Trim() ) )
                                                           .ToList()
            };

            return View( new Models.EquipePagina() 
            {
                equipe = new EquipeDAO().obter( id ),
                ListaRegiao = lstRegiao,
                Pesquisa = oPesquisa
            } );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editar( Models.EquipePagina pobj )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    new EquipeDAO().salvar( pobj.equipe );
                    return RedirectToAction( "index" );
                }
                else
                    return View();
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
                List<ItemRegiao> lstRegiao = new RegiaoDAO().listar();

                Models.EquipePesquisa oPesquisa = new Models.EquipePesquisa()
                {
                    ListaRegiao = lstRegiao,
                    ListaLiga = new List<Models.Campeonato>(),
                    ListaTemporadas = new List<Models.Temporada>()
                };
                return View( new Models.EquipePagina()
                {
                    equipe = pobj.equipe,
                    ListaRegiao = lstRegiao,
                    Pesquisa = oPesquisa
                } );
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