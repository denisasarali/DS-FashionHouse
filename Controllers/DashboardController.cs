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
    
    [Authorize]
    public class DashboardController : Controller
    {
        DBcasademodaEntities db = new DBcasademodaEntities();
        // GET: Dashboard
        public ActionResult Index()
        {
            
            ViewBag.profit = db.DetaliiFacturas.Sum(model => model.PretTotal);
            ViewBag.useri = db.Membrus.Where(model => model.IdRol == 3).Count();
            ViewBag.comenzi = db.Facturas.Count();
            //// Lấy số lượng người truy cập từ Application đã tạo
            //ViewBag.SoNguoiTruyCap = HttpContext.Application["SoNguoiTruyCap"].ToString();
            return View();
        }

        [HttpGet]
        public ActionResult SchimbaParola(int idmembru)
        {
            Membru membru = db.Membrus.Find(idmembru);
            return View(membru);
        }
        [HttpPost]
        public ActionResult SchimbaParola(Membru membru, FormCollection collection)
        {
            try
            {
                var ParolaCurenta = collection["ParolaCurenta"]; 
                var ParolaNoua = collection["ParolaNoua"];
                var ConfirmParola = collection["Confirm"];

                ParolaCurenta = CriptareParola.MD5Hash(ParolaCurenta.Trim());
                ParolaNoua = CriptareParola.MD5Hash(ParolaNoua.Trim());

                var verifica = db.Membrus.Where(model => model.parola == ParolaCurenta && model.IdMembru == membru.IdMembru).FirstOrDefault();
                if (verifica != null)
                {
                    verifica.parola = ParolaNoua;
                    db.SaveChanges();
                    TempData["msgChangePassword"] = "Parola schimbata cu succes!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Parola incorecta!");
                    return View(membru);
                }
                return View(membru);
            }
            catch (Exception ex)
            {
                TempData["msgChangePasswordFailed"] = "Editare esuata! " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        //public PartialViewResult InvoinceList() // Danh sách hóa đơn trong ngày
        //{
        //    var list = db.vHoaDonTrongNgays.OrderByDescending(model => model.dateOrder).ToList();
        //    return PartialView(list);
        //}
    }
}