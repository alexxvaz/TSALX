using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using TSALX.DAO;

namespace TSALX.Controllers
{
    public class TemporadaController : Controller
    {

        private Models.Temporada carregarPagina( int? pintCampeonato )
        {
            ModelState.Clear();

            List<Models.CampeonatoLista> lstCamp = new CampeonatoDAO().listar();

            if( pintCampeonato == null )
                pintCampeonato = lstCamp.FirstOrDefault().IDCampeonato;

            Dictionary<int, List<Models.TemporadaEquipe>> dicEquipe = new TemporadaDAO( Convert.ToInt32( pintCampeonato ) ).listar();

            
            return new Models.Temporada()
            {
                ListaCampeonato = lstCamp,
                IDCampeonato = Convert.ToInt32( pintCampeonato ),
                Coluna1Equipe = dicEquipe[ 1 ],
                Coluna2Equipe = dicEquipe[ 2 ],
                Coluna3Equipe = dicEquipe[ 3 ],
                Coluna4Equipe = dicEquipe[ 4 ]
            };

        }
        // GET: Temporada   
        public ActionResult Index()
        {
            return View( this.carregarPagina( null ) );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index( int IDCampeonato )
        {
            return View( this.carregarPagina( IDCampeonato ) );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Gravar( FormCollection frmTemp )
        {
            int intCampeonato = int.Parse( frmTemp[ "IDCampeonato" ] );

            IEnumerable<string> lstEquipe = frmTemp[ "equipe.Participa" ]
                                            .Split( ',' )
                                            .Where( e => e != "false" );

            if( new TemporadaDAO( intCampeonato ).gravar( lstEquipe ) )
                ViewBag.Mensagem = "Temporada salva com sucesso";
            else
                ViewBag.ErroMensagem = "Não foi possível gravar a temporada selecionada";

            return View( "Index", this.carregarPagina( intCampeonato ) );
        }
        
    }
}