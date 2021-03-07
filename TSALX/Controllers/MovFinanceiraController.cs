using System;
using System.Web.Mvc;

using Alxware.Erro;
using TSALX.DAO;

namespace TSALX.Controllers
{
    public class MovFinanceiraController : Controller
    {

        // GET: Lancamento
        public ActionResult Depositar()
        {
            return View( new Models.Lancamento() );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Depositar( Models.Lancamento pobjDep )
        {

            try
            {
                if( ModelState.IsValid )
                {
                    new LancamentoDAO().depositar( pobjDep );

                    ViewBag.Mensagem = "Depósito realizado com sucesso";
                    ModelState.Clear();
                }
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
            }

            return View( new Models.Lancamento() );
        }

        public ActionResult Sacar()
        {
            return View( new Models.Lancamento() );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sacar( Models.Lancamento pobjDep )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    new LancamentoDAO().sacar( pobjDep );

                    ViewBag.Mensagem = "Saque realizado com sucesso";
                    ModelState.Clear();
                }
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
            }

            return View( new Models.Lancamento() );
        }


        public ActionResult Extrato()
        {
            Models.Extrato oExtrato = new Models.Extrato() { DtInicial = DateTime.Today.AddDays( -15 ), DtFinal = DateTime.Today };
            try
            {
                oExtrato.Registros = new LancamentoDAO().emitirExtrato( DateTime.Today.AddDays( -15 ), DateTime.Today );
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
            }

            return View( oExtrato );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Extrato( DateTime DtInicial, DateTime DtFinal )
        {
            Models.Extrato oExtrato = new Models.Extrato() { DtInicial = DateTime.Today.AddDays( -15 ), DtFinal = DateTime.Today };

            try
            {
                if( DtInicial > DtFinal )
                    ModelState.AddModelError( "DataInicialMaior", "A data inicial é MAIOR que a data final" );
                
                oExtrato.DtInicial = DtInicial;
                oExtrato.DtFinal = DtFinal;
                oExtrato.Registros = new LancamentoDAO().emitirExtrato( DtInicial, DtFinal );
                    
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
            }

            return View( oExtrato );

        }
    }
}