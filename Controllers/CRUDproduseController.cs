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
{   [Authorize]
    public class CRUDproduseController : Controller
    {

        DBcasademodaEntities db = new DBcasademodaEntities();
        // GET: CRUDproduse
        public ActionResult Index(int? pagina, string cautare)
        {
            var numarPagina = pagina ?? 1;
            var dimensiunePagina = 10;
            var produse = db.Produs.Where(model => model.NumeProdus.Contains(cautare) || cautare == null).OrderByDescending(model => model.DataCreare).Include(model => model.Categorie).ToPagedList(numarPagina, dimensiunePagina);
            return View(produse);
          
        }

        //CREEAZA
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult Creeaza()
        {
            ViewBag.idCategorie = new SelectList(db.Categories, "idCategorie", "numeCategorie");
            return View();
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Creeaza(Produ produs, HttpPostedFileBase uploadFile)
        {
            try
            {
                var numeProdus = produs.NumeProdus.Trim();
                // Obțineți numele produsului pentru a verifica dacă este același
                var verifica = db.Produs.SingleOrDefault(model => model.NumeProdus == numeProdus && model.IdCategorie==produs.IdCategorie);
                
                // procesarea imaginii
                var fileName = Path.GetFileName(uploadFile.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/img/produs"), fileName);
                string extensie = Path.GetExtension(uploadFile.FileName);
                
                    if (extensie.ToLower() == ".jpg" || extensie.ToLower() == ".jpeg" || extensie.ToLower() == ".png")
                    {
                        if (verifica != null)
                        {
                            ModelState.AddModelError("", "Numele produsului e deja existent");
                        }
                        else
                        {
                            if (uploadFile == null)
                            {
                                ModelState.AddModelError("", "Eroare la incarcarea imaginii.");
                            }
                            else
                            {

                                produs.NumeProdus = produs.NumeProdus.Trim();
                                produs.Imagine = "~/Content/img/produs/" + fileName;
                                //produs.username = Session["usernameAdmin"].ToString();
                                produs.DataCreare = DateTime.Now;
                                produs.Status = true;
                                db.Produs.Add(produs);
                                if (db.SaveChanges() > 0)
                                {
                                    uploadFile.SaveAs(path);
                                    ModelState.Clear();
                                    TempData["msjAdaugare"] = "Produs adaugat cu succes!";
                                    return RedirectToAction("Index");
                                }

                            }
                        }

                    } 
                else
                {
                    ModelState.AddModelError("", "Invalid");
                }
                ViewBag.idCategorie = new SelectList(db.Categories, "idCategorie", "numeCategorie", produs.IdCategorie);

                return View(produs);
            }
            catch (Exception ex)
            {
                TempData["msjAdaugareEsuata"] = "Adaugarea a esuat! " + ex.Message;
                return RedirectToAction("Creeaza");
            }
        }


        //EDIT
        [HttpGet]
        public ActionResult Editeaza(int? id)
        {
            Produ produs = db.Produs.Find(id);
            Session["caleImg"] = produs.Imagine;
            ViewBag.IdCategorie = new SelectList(db.Categories, "IdCategorie", "NumeCategorie", produs.IdCategorie);
            return View(produs);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Editeaza(Produ produs, HttpPostedFileBase incarcaFisier, FormCollection colectie)
        {
            try
            {
                var descriere = colectie["descriere"];
                if (ModelState.IsValid)
                {
                    if (incarcaFisier != null)
                    {
                        var fileName = Path.GetFileName(incarcaFisier.FileName);
                        var cale = Path.Combine(Server.MapPath("~/Content/img/produs"), fileName);

                        produs.Imagine = "~/Content/img/produs/" + fileName;
                        produs.Descriere = descriere;
                        db.Entry(produs).State = EntityState.Modified;
                        string caleImagineVeche = Request.MapPath(Session["caleImg"].ToString());
                        if (db.SaveChanges() > 0)
                        {
                            TempData["msjEditare"] = "Actualizarea a fost realizata cu succes! " + produs.NumeProdus;
                            incarcaFisier.SaveAs(cale);
                            if (System.IO.File.Exists(caleImagineVeche))
                            {
                                System.IO.File.Delete(caleImagineVeche);
                            }
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        produs.Descriere = descriere;
                        produs.Imagine = Session["caleImg"].ToString();
                        db.Entry(produs).State = EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            TempData["msjEditare"] = "Produsul editat cu succes are Id-ul: " + produs.IdProdus;
                            return RedirectToAction("index");
                        }
                    }
                }
                ViewBag.IdCategorie = new SelectList(db.Categories, "IdCategorie", "NumeCategorie", produs.IdCategorie);
                return View(produs);
            }
            catch (Exception ex)
            {
                TempData["msjEditareEsuata"] = "Editare esuata! " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        //DELETE
        public ActionResult Sterge(int? id)
        {
            try
            {
                var verificaFactura = db.DetaliiFacturas.FirstOrDefault(model => model.IdProdus == id);
                //Verificați dacă id-ul produsului există în tabelul DetaliiFactura?
                if (verificaFactura != null) // Dacă este valid, trimiteți un mesaj de eroare
                {
                    TempData["msjStergere"] = "Nu se poate sterge!";
                    return RedirectToAction("Index");
                }
                else
                {
                    Produ produs = db.Produs.Find(id);
                    string imagineCurenta = Request.MapPath(produs.Imagine);
                    if (System.IO.File.Exists(imagineCurenta))
                    {
                        System.IO.File.Delete(imagineCurenta);
                    }
                    db.Produs.Remove(produs);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["msjStergere"] = "Nu se poate sterge! " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}