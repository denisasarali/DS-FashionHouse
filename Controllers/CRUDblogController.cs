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
    public class CRUDblogController : Controller
    {
        // GET: CRUDblog
        DBcasademodaEntities db = new DBcasademodaEntities();
        public ActionResult Index(int? page, string searching)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;
            var articol = db.Articols.Where(model => model.Titlu.Contains(searching) || searching == null).OrderByDescending(model => model.DataPublicare).Include(model => model.Categorie).ToPagedList(pageNumber, pageSize);
            return View(articol);
        }

        //CREATE
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult Creeaza()
        {
            ViewBag.idCategorie = new SelectList(db.Categories, "idCategorie", "numeCategorie");
            return View();
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Creeaza(Articol articol,Membru membru , HttpPostedFileBase uploadFile)
        {
            try
            {
                var titluArticol = articol.Titlu.Trim();
                
                var verifica = db.Articols.SingleOrDefault(model => model.Titlu == titluArticol && model.IdCategorie == articol.IdCategorie);
                
                var fileName = Path.GetFileName(uploadFile.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/img/blog"), fileName);
                string extension = Path.GetExtension(uploadFile.FileName);

                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    if (verifica != null)
                    {
                        ModelState.AddModelError("", "Exista deja un articol cu acelasi titlu despre aceeasi categorie");
                    }
                    else
                    {
                        if (uploadFile == null)
                        {
                            ModelState.AddModelError("", "Error while file uploading.");
                        }
                        else
                        {

                            articol.Imagine = "~/Content/img/blog/" + fileName;

                            articol.DataPublicare = DateTime.Now;
                            articol.Status = true;
                            //articol.IdMembru = membru.IdMembru;
                            db.Articols.Add(articol);
                            if (db.SaveChanges() > 0)
                            {
                                uploadFile.SaveAs(path);
                                ModelState.Clear();
                                TempData["msgCreate"] = "Successfully create a new blog!";
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid File Type");
                }
                ViewBag.idCategorie = new SelectList(db.Categories, "idCategorie", "numeCategorie", articol.IdCategorie);
                return View(articol);
            }
            catch (Exception ex)
            {
                TempData["msgCreatefailed"] = "Create failed! " + ex.Message;
                return RedirectToAction("Creeaza");
            }
        }


        //DELETE
        public ActionResult Sterge(int? id)
        {
            try
            {
                Articol articol = db.Articols.Find(id);
                string currentImg = Request.MapPath(articol.Imagine);
                if (System.IO.File.Exists(currentImg))
                {
                    System.IO.File.Delete(currentImg);
                }
                db.Articols.Remove(articol);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["msgDelete"] = "Can't delete this! " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}