using System;
using System.Web.Mvc;

using Alxware.Erro;

using TSALX.Models;
using TSALX.ViewModel;

namespace TSALX.Controllers
{
    public class RegiaoController : Controller
    {
        private Servico.APIFutebol _apiFutebol = new Servico.APIFutebol();
        private DAO.RegiaoDAO _oDAO = new DAO.RegiaoDAO();

        public ActionResult Index()
        {
            string strMensagem = string.Empty;
            ErroTipo enuTipo = ErroTipo.Processo;

            if( Request.QueryString[ "Mensagem" ] != null )
            {
                strMensagem = Request.QueryString[ "Mensagem" ];

                switch( Request.QueryString[ "Tipo" ] )
                {
                    case "Processo":
                        enuTipo =  ErroTipo.Processo;
                        break;
                    case "Dados":
                        enuTipo = ErroTipo.Dados;
                        break;
                    case "Sistema":
                        enuTipo = ErroTipo.Sistema;
                        break;
                }
            }
                
            return View( new RegiaoPG() 
            {
                Titulo = "Região",
                TextoMSG = strMensagem,
                TipoMSG = Convert.ToInt16( enuTipo ),
                ListaRegiao = _oDAO.listar()
            } );
        }

        [HttpGet]
        public ActionResult novo()
        {
            ModelState.Clear();

            return View( new RegiaoVM() 
            { 
                Titulo = "Nova Região",
                regiao = new Regiao(),
                ListaCountry = _apiFutebol.listarBandeira()
            } );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult novo( RegiaoVM pobj )
        {
            try
            {
                if ( ModelState.IsValid )
                {
                    if ( !string.IsNullOrWhiteSpace( pobj.regiao.CodCountry ) )
                    {
                        pobj.regiao.Country = _apiFutebol.obterNomeBandeira( pobj.regiao.CodCountry );
                        pobj.regiao.Sigla = pobj.regiao.CodCountry;
                    }

                    _oDAO.salvar( pobj.regiao );
                    return RedirectToAction( "Index" );
                }
                else
                    return View();
            }
            catch( alxExcecao ex )
            {
                return View( new RegiaoVM()
                {
                    Titulo = "Nova Região",
                    regiao = new Regiao(),
                    ListaCountry = _apiFutebol.listarBandeira(),
                    TextoMSG = ex.Mensagem,
                    TipoMSG =  Convert.ToInt16( ex.Tipo )
                } );
            }
        }

        [HttpGet]
        public ActionResult editar( int id )
        {
            return View( new RegiaoVM() 
            { 
                Titulo = "Editar Região",
                regiao = _oDAO.obter( id ), 
                ListaCountry = _apiFutebol.listarBandeira()
            } );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editar( RegiaoVM pobj )
        {
            try
            {
                if( ModelState.IsValid )
                {
                    if ( !string.IsNullOrWhiteSpace( pobj.regiao.CodCountry ) )
                    {
                        pobj.regiao.Country = _apiFutebol.obterNomeBandeira( pobj.regiao.CodCountry );
                        pobj.regiao.Sigla = pobj.regiao.CodCountry;
                    }

                    _oDAO.salvar( pobj.regiao );
                    return RedirectToAction( "index" );
                }
                else
                    return View();

            }
            catch( alxExcecao ex )
            {
                return View( new RegiaoVM()
                {
                    Titulo = "Editar Região",
                    regiao = pobj.regiao,
                    ListaCountry = _apiFutebol.listarBandeira(),
                    TextoMSG = ex.Mensagem,
                    TipoMSG = Convert.ToInt16( ex.Tipo )
                } );
            }

        }

        public ActionResult excluir( int id )
        {
            try
            {
                _oDAO.excluir( id );
                return RedirectToAction( "index" );
            }
            catch( alxExcecao ex )
            {
               return RedirectToAction( "index", ex  );
            }

        }
        
    }
}