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
    public class CRUDclientiController : Controller
    {

        DBcasademodaEntities db = new DBcasademodaEntities();
        // GET: CRUDclienti
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult AfiseazaClienti()
        {
            try
            {
                var afClienti = (from i in db.Clients
                                  orderby i.Nume ascending
                                  select new
                                  {
                                      IdClient = i.IdClient,
                                      Prenume = i.Prenume,
                                      Nume = i.Nume,
                                      Email = i.Email,
                                      Telefon = i.Telefon,
                                      Adresa = i.Adresa
                                  }).ToList();

                return Json(new { code = 200, afClienti = afClienti, msg = "Lista actualizata cu succes!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lista falsa: " + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
    }
}