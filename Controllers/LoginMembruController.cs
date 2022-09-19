using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CasadeModa.Models;
using System.Web.Security;
using System.Net;
using System.Data.Entity;

namespace CasadeModa.Controllers
{
    public class LoginMembruController : Controller
    {
        DBcasademodaEntities db = new DBcasademodaEntities();
        // GET: LoginMembru
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var us = collection["username"];
            var pr = collection["parola"];
            pr = CriptareParola.MD5Hash(pr);

            var nuMembru = db.Membrus.Where(model => model.IdRol == 3).SingleOrDefault(model => model.username == us && model.parola == pr);
            var verifica = db.Membrus.Where(model => model.IdRol == 1 || model.IdRol == 2).SingleOrDefault(model => model.username == us && model.parola == pr);
            if (ModelState.IsValid)
            {
                if (verifica == null)
                {
                    if (nuMembru != null)
                    {
                        ModelState.AddModelError("", "Rolul este invalid");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Exista o problema! Verifica username-ul sau parola!");
                    }
                }

                else
                {
                    //if (!this.IsCaptchaValid(""))
                    //{
                    //    ViewBag.captcha = "Captcha is not valid";
                    //}
                    //else
                    //{
                        FormsAuthentication.SetAuthCookie(verifica.Nume, false);
                        Session["usernameAdmin"] = verifica.username;
                        Session["infoAdmin"] = verifica;
                        return RedirectToAction("Index", "Dashboard");
                    //}
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}