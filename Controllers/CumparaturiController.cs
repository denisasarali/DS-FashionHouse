using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CasadeModa.Models;
using PagedList;


namespace CasadeModa.Controllers
{
    public class CumparaturiController : Controller
    {
        DBcasademodaEntities db = new DBcasademodaEntities();
        // GET: Cumparaturi
        public ActionResult ListaProduse()
        {
            return View();
        }


        public PartialViewResult ListaArticol(int? categorii, int? page, string searching)
        {
            var pageNumber = page ?? 1;
            var pageSize = 9;
            if (searching != null)
            {
                ViewBag.categories = categorii;
                var list = db.Produs.Where(model => model.NumeProdus.Contains(searching) || searching == null && model.Status == true).OrderByDescending(model => model.DataCreare).ToPagedList(pageNumber, pageSize);
                return PartialView(list);
            }
            else
            {

                if (categorii != null)
                {
                    ViewBag.categories = categorii;
                    var list = db.Produs.OrderByDescending(model => model.DataCreare).Where(model => model.IdCategorie == categorii && model.Status == true).ToPagedList(pageNumber, pageSize);
                    return PartialView(list);
                }
                else
                {
                    var list = db.Produs.OrderByDescending(model => model.DataCreare).Where(model => model.Status == true).ToPagedList(pageNumber, pageSize);
                    return PartialView(list);
                }
            }
        }
        public PartialViewResult Categorii() // List categories
        {
            var list = db.Categories.ToList();
            return PartialView(list);
        }
        

        public ActionResult DetaliiProdus(int? id, int? categorie)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var detalii = db.Produs.Where(model => model.IdProdus == id).Single();
            //ViewBag.PretNou = detail.Pret - ((detail.Pret * detail.discount) / 100);
            if (detalii == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(detalii);
        }
    }
}