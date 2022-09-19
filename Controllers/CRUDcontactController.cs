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
using System.Net.Mail;
using System.Text;

namespace CasadeModa.Controllers
{
    public class CRUDcontactController : Controller
    {
        // GET: CRUDcontact
        DBcasademodaEntities db = new DBcasademodaEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult AfiseazaContact()
        {
            try
            {
                var afContact = (from i in db.Contacts
                                 orderby i.DataContact descending
                                 select new
                                 {
                                     id = i.IdContact,
                                     nume = i.Nume,
                                     email = i.Email,
                                     mesaj = i.Mesaj
                                 }).ToList();

                return Json(new { code = 200, afContact = afContact, msg = "Lista contacte afisate cu succes!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lista contacte falsa: " + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult Sterge(int id)
        {
            try
            {
                Contact contact = (from i in db.Contacts
                                   select i).SingleOrDefault(model => model.IdContact == id);
                db.Contacts.Remove(contact);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Sters cu succes!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "stergere esuata! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult Raspuns(int id)
        {
            try
            {
                var detail = (from i in db.Contacts
                              select new
                              {
                                  id = i.IdContact,
                                  nume = i.Nume,
                                  email = i.Email
                              }).SingleOrDefault(model => model.id == id);
                //var detail = db.ProductCategories.Where(model => model.categoryId == categoryId).SingleOrDefault();
                return Json(new { code = 200, detail = detail, msg = "Obtinut cu succes!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Esuat!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult TrimiteEmailCatreUtilizator(string nume, string email, string subiect)
        {
            bool result = false;

            result = TrimiteEmail(email, "Raspuns", "<p>Buna ziua " + nume + ",<br /> " + subiect + " <br />O zi buma!.</p>");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public bool TrimiteEmail(string toEmail, string subiect, string emailBody)
        {
            try
            {
                string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                MailMessage mailMessage = new MailMessage(senderEmail, toEmail, subiect, emailBody);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                client.Send(mailMessage);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}