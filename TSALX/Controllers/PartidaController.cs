using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Alxware.Erro;
using TSALX.DAO;

namespace TSALX.Controllers
{
    public class PartidaController : Controller
    {
        private PartidaDAO _oPartidaDAO = new PartidaDAO();

        private Models.Partida listar( int? pintCampeonato )
        {

            if( pintCampeonato == null )
                pintCampeonato = 0;

            CampeonatoDAO objCamp = new CampeonatoDAO();
            List<Models.CampeonatoLista> lstCamp = objCamp.listarAtivos();

            Models.Partida objPar = new Models.Partida()
            {
                ListaCampeonato = lstCamp,
                ListaEquipe = objCamp.listarTimes( Convert.ToInt32( pintCampeonato ) )
            };

            return objPar;
        }
        // GET: Partida
        public ActionResult Criar()
        {
            return View( listar( null ) );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Criar( int IDCampeonato )
        {
            return View( listar( IDCampeonato ) );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Gravar( Models.Partida pobjPartida )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    _oPartidaDAO.gravar( pobjPartida );
                    ViewBag.Mensagem = "Partida gravada com sucesso";
                }
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
            }
            
            return View( "Criar", listar( pobjPartida.IDCampeonato ) );
        }
        public ActionResult Editar( long ID )
        {
            return View( _oPartidaDAO.obter( ID ) );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar( Models.Partida pobjPart )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    _oPartidaDAO.gravar( pobjPart );
                    ViewBag.Mensagem = "Partida gravada com sucesso";
                }
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
            }

            return View( _oPartidaDAO.obter( pobjPart.IDPartida ) );
        }

        public ActionResult Excluir( long ID )
        {
            if( !_oPartidaDAO.excluir( ID ) )
            {
                ViewBag.ErroMensagem = "Não foi possível excluir a partida";
                return View( _oPartidaDAO.obter( ID ) );
            }
            else
                return RedirectToAction( "index", "Home" );
            
        }
        public ActionResult ExcluirEntrada( long ID, long IDEntrada )
        {
            try
            {
                new EntradaDAO( ID ).excluir( IDEntrada );
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
            }

            return View( "Editar", _oPartidaDAO.obter( ID ) );
        }

        public ActionResult Finalizar( long ID )
        {
            try
            {
                _oPartidaDAO.finalizar( ID );
                ViewBag.Mensagem = "Entradas lançadas no Movimento Financeiro";
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
            }

            return View( "Editar", _oPartidaDAO.obter( ID ) );
        }
    }
}