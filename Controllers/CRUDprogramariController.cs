using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CasadeModa.Models;
using System.IO;
using PagedList;
using PagedList.Mvc;
namespace CasadeModa.Controllers
{
    public class CRUDprogramariController : Controller
    {
        DBcasademodaEntities db = new DBcasademodaEntities();
        // GET: CRUDprogramari
        public ActionResult Index(int? pagina)
        {

            var numarPagina = pagina ?? 1;
            var marimePagina = 10;

            var listaordonata = db.Programaris.OrderByDescending(model => model.Data).Include(model => model.Membru).Include(model => model.Client).ToPagedList(numarPagina, marimePagina);
            return View(listaordonata);
        }

        public ActionResult DetaliiProgramare(int IdProgramare)
        {

            var info = db.Programaris.Where(model => model.IdProgramare == IdProgramare).Include(model => model.Client).Include(model => model.Membru).FirstOrDefault();
            Session["informatii"] = info;
            ViewBag.IdFactura = IdProgramare;
           
            return View(info);
        }
    }
}