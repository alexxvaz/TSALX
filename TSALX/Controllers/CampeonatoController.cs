using System;
using System.Linq;
using System.Web.Mvc;

using Alxware.Erro;

using TSALX.DAO;
using TSALX.Models;
using TSALX.ViewModel;

namespace TSALX.Controllers
{
    public class CampeonatoController : Controller
    {
        private CampeonatoDAO _oDAO = new CampeonatoDAO();

        public ActionResult Index(int? id)
        {
            // Tratamento da mensagem
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

            // Campeonato
            Campeonato oCampeonato = new Campeonato();

            if( id.HasValue )
                oCampeonato = _oDAO.obter( id.Value );
            
            return View( new CampeonatoPG()
            {
                Titulo = "Campeonato",
                TextoMSG = strMensagem,
                TipoMSG = Convert.ToInt16( enuTipo ),
                campeonato = oCampeonato,
                ListaTemporada = new TemporadaDAO().listar(),
                ListaLiga = new LigaDAO().listar(),
                ListaCampeonato = _oDAO.listar()
                                       .OrderByDescending( c => c.Ativo )
                                       .OrderBy( c => c.NomeLiga )
                                       .ToList()
            } );
        }
        [HttpPost]
        public ActionResult Index( CampeonatoPG pForm )
        {
            try
            {
                if ( ModelState.IsValid )
                {
                    _oDAO.gravar( pForm.campeonato );
                    return RedirectToAction( "index" );
                }
                else
                    return View();
            }
            catch( alxExcecao ex )
            {
                return RedirectToAction( "index", ex );
            }

        }
        
        [HttpGet]
        public ActionResult editar( int id )
        {
            ModelState.Clear();
            Campeonato oCampeonato = _oDAO.obter( id );
            Liga oLiga = new LigaDAO().obter( oCampeonato.IDLiga );

            return View( new CampeonatoVM() { 
                Titulo = "Editar Campeonato",
                IDCampeonato = oCampeonato.IDCampeonato,
                Ativo = oCampeonato.Ativo,
                liga = oLiga,
                temporada = new TemporadaDAO().obter( oCampeonato.IDTemporada ),
                regiao = new RegiaoDAO().obter( oLiga.IDRegiao )
            } );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editar( CampeonatoVM pForm )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    _oDAO.ativar( pForm.IDCampeonato, 
                                  pForm.Ativo );

                    return RedirectToAction( "index" );
                }
                else
                    return View();

            }
            catch( alxExcecao ex )
            {
                Campeonato oCampeonato = _oDAO.obter( pForm.IDCampeonato );

                return View( new CampeonatoVM()
                {
                    Titulo = "Editar Campeonato",
                    TextoMSG = ex.Mensagem,
                    TipoMSG = Convert.ToInt16( ex.Tipo ),
                    IDCampeonato = oCampeonato.IDCampeonato,
                    Ativo = oCampeonato.Ativo,
                    liga = new LigaDAO().obter( oCampeonato.IDLiga ),
                    temporada = new TemporadaDAO().obter( oCampeonato.IDTemporada )

                } );
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