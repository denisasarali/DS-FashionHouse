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
    [Authorize]

    public class CRUDfacturaController : Controller
    { 
        DBcasademodaEntities db = new DBcasademodaEntities();
        //GET: CRUDfactura
        public ActionResult Index(int? pagina)
        {

            var numarPagina = pagina ?? 1;
            var marimePagina = 10;
            var listaordonata = db.Facturas.OrderByDescending(model => model.DataComanda).Include(model => model.Membru).Include(model => model.Client).ToPagedList(numarPagina, marimePagina);
            return View(listaordonata);
        }

        public ActionResult DetaliiFactura(int IdFactura)
        {

            var info = db.Facturas.Where(model => model.IdFactura == IdFactura).Include(model => model.Client).Include(model => model.Membru).FirstOrDefault();
            Session["informatii"] = info;
            ViewBag.IdFactura = IdFactura;
            var detalii = db.DetaliiFacturas.Where(model => model.IdFactura == IdFactura).Include(model => model.Produ).Include(model => model.Marime).Include(model => model.Culoare).ToList();

            return View(detalii);
        }

        public ActionResult Livrare(int id)
        {
            Factura factura = db.Facturas.Find(id);
            factura.DataLivrare = DateTime.Now;
            factura.StatusLivrare = true;
            TempData["Livrare"] = "Comanda ID " + factura.IdFactura + " a fost livrata cu succes ";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GenerarePDF()
        {
            return new Rotativa.ActionAsPdf("DetaliiFactura");
        }
    }
}