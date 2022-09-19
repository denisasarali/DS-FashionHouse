using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CasadeModa.Models;


namespace CasadeModa.Controllers
{ [Authorize]
    public class CRUDcategorieController : Controller
    {
        DBcasademodaEntities db = new DBcasademodaEntities();
        // GET: CRUDcategorie
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult DsCategorie()
        {
            try
            {
                var dsCategorie = (from i in db.Categories
                                   select new
                                   {
                                       idCategorie = i.IdCategorie,
                                       numeCategorie = i.NumeCategorie
                                   }).ToList();

                return Json(new { code = 200, dsCategorie = dsCategorie, msg = "Lista obtinuta cu succes!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lista falsa: " + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult AdaugaCategorie(string numeCategorie)
        {
            try
            {
                var categorie = new Categorie();
                categorie.NumeCategorie = numeCategorie;

                db.Categories.Add(categorie);
                db.SaveChanges();

                return Json(new { code = 200, msg = "Categorie adaugata cu succes!!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Eroare: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Detalii(int idCategorie)
        {
            try
            {
                var detalii = (from i in db.Categories
                               select new
                               {
                                   idCategorie = i.IdCategorie,
                                   numeCategorie = i.NumeCategorie

                               }).SingleOrDefault(model => model.idCategorie == idCategorie);
                //var detail = db.ProductCategories.Where(model => model.categoryId == categoryId).SingleOrDefault();
                return Json(new { code = 200, detail = detalii, msg = "Detalii botinute cu succes!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Esuat!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        //[HttpPost]
        //public JsonResult Actualizati(int idCategorie, string numeCategorie)
        //{
        //    try
        //    {
        //        var categorie = (from i in db.Categories
        //                         select i).SingleOrDefault(model => model.IdCategorie == idCategorie);
        //        categorie.NumeCategorie = (string)numeCategorie;
        //        db.SaveChanges();
        //        return Json(new { code = 200, msg = "Actualizat cu succes!" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { code = 500, msg = "Actualizare esuata" + ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        [HttpPost]
        public JsonResult Sterge(int idCategorie)
        {
            try
            {
                Categorie categorie = (from i in db.Categories
                                       select i).SingleOrDefault(model => model.IdCategorie == idCategorie);
                db.Categories.Remove(categorie);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Stergere efectuata cu succes!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Stergere esuata! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}