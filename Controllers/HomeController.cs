using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CasadeModa.Models;

namespace CasadeModa.Controllers
{
    public class HomeController : Controller
    {
        DBcasademodaEntities db = new DBcasademodaEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public ActionResult Contact(Contact contact)
        {
            contact.DataContact = DateTime.Now;
            db.Contacts.Add(contact);
            var rezultat = db.SaveChanges();
            if (rezultat > 0)
            {
                ViewBag.MessageSuccess = "Mesajul dumneavoastra a fost trimis cu succes. Te vom contacta in cel mai scurt timp posibil";
            }
            return View(contact);
        }



        public PartialViewResult ProduseAcasa()
        {
            var list = db.Produs.OrderByDescending(model => model.DataCreare).Take(8).ToList();
            return PartialView(list);
        }

        [HttpGet]
        public ActionResult ComandaMea(int idmembru)
        {
            var comenzi = db.Facturas.OrderByDescending(model => model.DataComanda).Where(model => model.IdMembru == idmembru).ToList();
            return View(comenzi);
        }
        [HttpGet]
        public ActionResult DetaliiFactura(int idfactura)
        {
            var detail = db.DetaliiFacturas.Where(model => model.IdFactura == idfactura).Include(model => model.Produ).ToList();
            var infor = db.Facturas.Where(model => model.IdFactura == idfactura).Include(model => model.Membru).FirstOrDefault();
            ViewBag.idfactura = idfactura;
            Session["informatii"] = infor;

            return View(detail);
        }



        [HttpGet]
        public ActionResult SchimbaParola(int idmembru)
        {
            Membru membru = db.Membrus.Find(idmembru);
            return View();
        }
        [HttpPost]
        public ActionResult SchimbaParola(Membru membru, FormCollection collection)
        {
            try
            {
                var ParolaCurenta = collection["ParolaCurenta"]; 
                var ParolaNoua = collection["ParolaNoua"]; 
                var ConfirmParola = collection["Confirm"];
                //ParolaCurenta = ParolaCurenta.Trim();
                ParolaCurenta = CriptareParola.MD5Hash(ParolaCurenta.Trim());
                ParolaNoua = CriptareParola.MD5Hash(ParolaNoua.Trim());
                //ParolaCurenta = ParolaCurenta.Trim();
                //ParolaNoua =ParolaNoua.Trim();
                var check = db.Membrus.Where(model => model.parola == ParolaCurenta && model.IdMembru == membru.IdMembru).FirstOrDefault();
                if (check != null)
                {
                    check.parola = ParolaNoua;
                    db.SaveChanges();
                    TempData["msgChangePassword"] = "Parola schimbata cu succes!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Parola incorecta!");
                    return View(membru);
                }
            }
            catch (Exception ex)
            {
                TempData["msgChangePasswordFailed"] = "Editare esuata! " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}