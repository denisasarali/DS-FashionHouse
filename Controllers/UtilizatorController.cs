using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CasadeModa.Models;
//using CaptchaMvc.HtmlHelpers;
//using NPOI.POIFS.Crypt;
using System.Security.Cryptography;
using System.Web.Security;

namespace CasadeModa.Controllers
{
    public class UtilizatorController : Controller
    {
        DBcasademodaEntities db = new DBcasademodaEntities();
        [HttpGet]
        // GET: Utilizator
        public ActionResult Login()
        {
            if (Session["info"] != null) // Dacă sunteți deja autentificat,
                                         // editați linkul pentru a vă autentifica și veți fi redirecționat către pagina de start
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)  //autentificare
        {
            var us = collection["username"];
            var pr = collection["parola"];
            pr = CriptareParola.MD5Hash(pr);

            //var verific = db.Membrus.SingleOrDefault(model => model.username == us && model.parola == pr);
            //if (ModelState.IsValid)
            //{
            //    if (verific == null)
            //    {
            //        ModelState.AddModelError("", "Exista o problema la autentificare. Verifica username-ul sau parola.");
            //    }
            //    else
            //    {
            //            Session["info"] = verific;
            //            return RedirectToAction("Index", "Home");


            //    }
            //}
            var membru = db.Membrus.Where(model => model.IdRol == 3).SingleOrDefault(model => model.username == us && model.parola == pr);
            var administrator = db.Membrus.Where(model => model.IdRol == 1 || model.IdRol == 2).SingleOrDefault(model => model.username == us && model.parola == pr);

            if (ModelState.IsValid)
            {
                if (administrator == null)
                {
                    if (membru != null)
                    {
                        Session["info"] = membru;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Exista o problema! Verifica username-ul sau parola!");
                    }
                }

                else
                {
                    
                    FormsAuthentication.SetAuthCookie(administrator.Nume, false);
                    Session["usernameAdmin"] = administrator.username;
                    Session["infoAdmin"] = administrator;
                    return RedirectToAction("Index", "Dashboard");
                    
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Membru membru)  //inregistrare
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var verific = db.Membrus.Where(model => model.username == membru.username || model.Email == membru.Email).FirstOrDefault();
                    if (verific != null)
                    {
                        ModelState.AddModelError("", "Exista deja un cont cu acelasi username sau care foloseste acelasi email!");
                        return View(membru);
                    }
                    else
                    {
                        membru.parola = CriptareParola.MD5Hash(membru.parola);

                        membru.DataInregistrare = DateTime.Now;
                        membru.Status = true;
                        membru.IdRol = 3;


                        db.Membrus.Add(membru);
                        var result = db.SaveChanges();
                        if (result > 0)
                        {
                            TempData["msjSucces"] = "Cont creat cu succes!";
                            return RedirectToAction("Login");
                        }
                    }
                }
                return View(membru);
            }
            catch (Exception ex)
            {
                TempData["msjEsuat"] = "Nu s-a putut creea contul! " + ex.Message;
                return RedirectToAction("Login");
            }
        }
        public ActionResult Logout()
        {
            Session.Remove("info");
            //Session["info"] = null;
            //Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}