using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using System.Windows.Forms;
using CasadeModa.Models;

namespace CasadeModa.Controllers
{
    public class ProgramariController : Controller
    {
        DBcasademodaEntities db = new DBcasademodaEntities();
        // GET: Programari
        public ActionResult Index()
        {
            return View();
        }

        public List<Programare> getProgramare() // Create list cart and save in session 
        {
            List<Programare> listaProgramari = Session["Programari"] as List<Programare>;
            if (listaProgramari == null)
            {
                // If the list item in the cart empty, it will create a list contains items
                listaProgramari = new List<Programare>();
                Session["Programare"] = listaProgramari;
            }
            return listaProgramari;
        }
        //public ActionResult CreeazaProgramare(int id) //  Add item in cart 
        //{
        //    List<Programari> listaProgramari = getProgramare();
        //    Programari programari = listaProgramari.Find(model => model.IdProgramare == id);
            
          
        //        programari = new Programari(id);

        //        listaProgramari.Add(programari);
        //        return Redirect("Home");
            
            
        //}
        [HttpGet]
        public ActionResult Programare()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Programare(Programari programari)
        {
            try
            {

                DateTime data = programari.Data;
               
                var verificaData= db.Programaris.Where(model => model.Data == data /*&& model.Ora == programari.Ora*/);
               
                TimeSpan ora = programari.Ora;
                var verificaOra = db.Programaris.SingleOrDefault(model => model.Ora == ora);
                Membru membru = (Membru)Session["info"];
                //if(DateTime.Parse(verificaData)<DateTime.Now)
                if (data<DateTime.Now )
                {
                    ModelState.AddModelError("", " Selectati o data valida");
                }
                else
                {
                    if (verificaData != null && verificaOra != null)
                    {
                        ModelState.AddModelError("", "Programare deja existenta");

                    }
                    else
                    {

                        programari.IdMembru = membru.IdMembru;
                        programari.Data = programari.Data;
                        programari.Ora = programari.Ora;


                        db.Programaris.Add(programari);
                        db.SaveChanges();
                        
                        //System.Windows.MessageBox.Show("Programarea a fost efectuata cu succes!");
                        
                        


                    }

                    
                }
                return View(programari);
            }
                

            catch (Exception ex)
            {
                return RedirectToAction("Programare", "Programari");
            }
        }
        [HttpGet]
        public ActionResult ProgramareFaracont()
        {
            
            
            return View();
        }
        [HttpPost]
        public ActionResult ProgramareFaraCont(Programari programari, Client client)
        {
            try
            {
               
                DateTime data = programari.Data;
                var verificaData = db.Programaris.Where(model => model.Data == data);
                TimeSpan ora = programari.Ora;
                var verificaOra = db.Programaris.SingleOrDefault(model => model.Ora == ora);
                
                var verifica = db.Clients.Where(model => model.Nume == client.Nume && model.Email == client.Email).FirstOrDefault();
                if (data < DateTime.Now)
                {
                    ModelState.AddModelError("", " Selectati o data valida");
                }
                else
                {
                    if (verificaData != null && verificaOra != null)
                    {
                        ModelState.AddModelError("", "Programare deja existenta!");

                    }
                    else
                    {

                        //Verificați dacă numele clientului cu e-mailul introdus există sau nu
                        if (verifica == null) // Dacă nu, salvați informațiile în tabelul client
                        {
                            
                            db.Clients.Add(client);
                            db.SaveChanges();
                            programari.IdClient = client.IdClient;
                            programari.Data = programari.Data;
                            programari.Ora = programari.Ora;
                            //programari.Nume = client.Nume;
                            //programari.Email = client.Email;
                            //programari.Telefon = client.Telefon;
                            db.Programaris.Add(programari);
                            db.SaveChanges();
                            //System.Windows.MessageBox.Show("Programarea a fost efectuata cu succes!");
                        }
                        else
                        {
                            programari.IdClient = verifica.IdClient;
                            programari.Data = programari.Data;
                            programari.Ora = programari.Ora;

                            db.Programaris.Add(programari);
                            db.SaveChanges();
                            //System.Windows.MessageBox.Show("Programarea a fost efectuata cu succes!");

                        }
                    }
                    
                }
               
                return View(programari);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ProgramareFaraCont", "Programari");
            }

           


        }

    }
}