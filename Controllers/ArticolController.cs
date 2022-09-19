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
    public class ArticolController : Controller
    {
        DBcasademodaEntities db = new DBcasademodaEntities();
        // GET: Articol
        public ActionResult ListaArticole(int? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);

            var ListBlog = db.Articols.OrderByDescending(model => model.DataPublicare).ToPagedList(pageNum, pageSize);
            return View(ListBlog);
        }
        public ActionResult DetaliiArticole(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var item = db.Articols.Where(model => model.IdArticol == id).Single();
            if (item == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(item);
        }
    }
}