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
using System.Web.Security;

namespace CasadeModa.Controllers
{
    public class CRUDmembruController : Controller
    {
        DBcasademodaEntities db = new DBcasademodaEntities();
        // GET: CRUDmembru
        public ActionResult Index(int? pagina, string cautare)
        {

            var numarPagina = pagina ?? 1;
            var marimePagina = 10;
            var membru = db.Membrus.Where(model => model.username.Contains(cautare) || cautare == null).OrderByDescending(model => model.DataInregistrare).Include(model => model.Rol).ToPagedList(numarPagina, marimePagina);
            return View(membru);
        }

        //CREEAZA
        [HttpGet]
        public ActionResult Creeaza()
        {
            ViewBag.IdRol = new SelectList(db.Rols, "IdRol", "NumeRol");
            return View();
        }
        [HttpPost]
        public ActionResult Creeaza(Membru membru)
        {
            try
            {
                var username = membru.username.Trim();
                // Obțineți numele de utilizator introdus pentru a verifica dacă se potrivește
                var verifica = db.Membrus.SingleOrDefault(model => model.username == username);
               
                    if (verifica != null)
                    {
                        ModelState.AddModelError("", "Username deja existent");
                    }
                    else
                    {
                        
                            membru.Prenume = membru.Prenume.Trim();
                            membru.Nume = membru.Nume.Trim();
                            membru.Email = membru.Email.Trim();
                            membru.Adresa = membru.Adresa.Trim();
                            membru.Telefon = membru.Telefon.Trim();
                            membru.DataInregistrare = DateTime.Now;
                            membru.Status = true;
                            membru.parola = CriptareParola.MD5Hash(membru.parola);
                            db.Membrus.Add(membru);
                            if (db.SaveChanges() > 0)
                            {
                               
                                ModelState.Clear();
                                TempData["msgCreate"] = "Membru creat cu succes!";
                                return RedirectToAction("Index");
                            }
                        
                    }
               
              

                ViewBag.IdRol = new SelectList(db.Rols, "IdRol", "NumeRol", membru.IdRol);
                return View(membru);
            }
            catch (Exception ex)
            {
                TempData["msgCreatefailed"] = "Creare esuata! " + ex.Message;
                return RedirectToAction("Creeaza");
            }
        }

        // EDITEAZA
        [HttpGet]
        public ActionResult Editeaza(int IdMembru)
        {
            Membru membru = db.Membrus.Find(IdMembru);
           
            ViewBag.IdRol = new SelectList(db.Rols, "IdRol", "NumeRol", membru.IdRol);
            return View(membru);
        }
        [HttpPost]
        public ActionResult Editeaza(Membru membru)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                        
                        db.Entry(membru).State = EntityState.Modified;
                        
                        if (db.SaveChanges() > 0)
                        {
                            TempData["msgEdit"] = "Editat cu succes!";
                           
                        }
                        return RedirectToAction("Index");
                   
                      
                    
                }
                ViewBag.IdRol = new SelectList(db.Rols, "IdRol", "NumeRol", membru.IdRol);
                return View(membru);
            }
            catch (Exception ex)
            {
                TempData["msgEditFailed"] = "Editare esuata! " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // DELETE
        public ActionResult Sterge(int IdMembru)
        {
            try
            {
                Membru membru = db.Membrus.Find(IdMembru);
                //var checkArticle = db.Articles.FirstOrDefault(model => model.userName == userName);
                var verificaFactura = db.Facturas.FirstOrDefault(model => model.IdMembru == IdMembru);
                //var checkProduct = db.Produs.FirstOrDefault(model => model.id == userName);

                //Verificați dacă contul se află în tabelul Articole, Tabelul Facturi sau Tabelul Produse
                if (/*checkArticle != null ||*/ verificaFactura != null ) 
                {
                    TempData["msgDeleteFailed"] = "Nu se poate sterge! ";
                    return RedirectToAction("Index");
                }
                else
                {
                    // Verificați dacă contul de conectare curent este același cu contul șters
                    if (Session["usernameAdmin"].ToString() == membru.username) // Dacă este același, ștergeți contul și deconectați-vă, reveniți la pagina de conectare
                    {
                        
                        db.Membrus.Remove(membru);
                        db.SaveChanges();
                        FormsAuthentication.SignOut();
                        return RedirectToAction("Login", "LoginMembru");
                    }
                    else
                    {
                        
                        db.Membrus.Remove(membru);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msgDeleteFailed"] = "Nu se poate sterge! " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}