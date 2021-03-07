using System;
using System.Web.Mvc;
using System.Web.Security;

using Alxware.Erro;
using TSALX.DAO;

namespace TSALX.Controllers
{
    public class HomeController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            Models.Inicio objIni = new Models.Inicio()
            {
                DtInicial = DateTime.Today.AddDays( -10 ),
                DtFinal = DateTime.Today,
                ListaPartidas = new PartidaDAO().listar( DateTime.Today.AddDays( -10 ), DateTime.Today )
            };
            
            return View( objIni );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index( DateTime DtInicial, DateTime DtFinal )
        {
            Models.Inicio objIni = null;

            try
            {
                if( DtInicial <= DtFinal )
                {
                    if( DtFinal.Subtract( DtInicial ).Days <= 30 )
                    {
                        objIni = new Models.Inicio()
                        {
                            DtInicial = DtInicial,
                            DtFinal = DtFinal,
                            ListaPartidas = new PartidaDAO().listar( DtInicial, DtFinal )
                        };
                    }
                    else
                        ModelState.AddModelError( "Periodo30dias", "O período deve ser no máximo de 30 dias entre as datas" );
                }
                else
                    ModelState.AddModelError( "DataInicialMaior", "A data inicial é MAIOR que a data final" );
            }
            catch( alxExcecao ex )
            {
                ViewBag.ErroMensagem = ex.Mensagem;
            }
            finally
            {
                if( objIni == null )
                {
                    objIni = new Models.Inicio()
                    {
                        DtInicial = DtInicial,
                        DtFinal = DtFinal,
                        ListaPartidas = new PartidaDAO().listar( DateTime.Today.AddDays( -10 ), DateTime.Today )
                    };

                }
            }

            return View( objIni );

        }

        public PartialViewResult Saldo()
        {
            return PartialView( new LancamentoDAO().emitirSaldo() );            
        }

        public ActionResult acesso()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult acesso( Models.Autorizacao oAcesso )
        {
            if( ModelState.IsValid )
            {
                string strFixo = System.Configuration.ConfigurationManager.AppSettings[ "codigo" ];
                long lngVariavel = Convert.ToInt64( DateTime.Today.ToString( "yyyyMMdd" ) );

                string strCodigo = $"{strFixo}{lngVariavel:X}";

                if( oAcesso.Codigo == strCodigo )
                {
                    FormsAuthentication.SetAuthCookie( strFixo, false );
                    return RedirectToAction( "Index", "Home" );
                }
                else
                {
                    ModelState.AddModelError( "CodigoInvalido", "Código invalido" );
                    return View();
                }
            }
            else
                return View();
        }

        public ActionResult sair()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction( "acesso", "Home" );
        }
    }
}