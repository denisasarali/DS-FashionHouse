using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CasadeModa.Models;


namespace CasadeModa.Controllers
{
    public class CosCumparaturiController : Controller
    {
        DBcasademodaEntities db = new DBcasademodaEntities();
        public List<Cos> getCos() // Create list cart and save in session 
        {
            List<Cos> listCart = Session["CosCumparaturi"] as List<Cos>;
            if (listCart == null)
            {
                // If the list item in the cart empty, it will create a list contains items
                listCart = new List<Cos>();
                Session["CosCumparaturi"] = listCart;
            }
            return listCart;
        }
        // GET: CosCumparaturi
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdaugaInCos(int id, string marime, string culoare, string strURL) //  Add item in cart 
        {
            List<Cos> listCart = getCos();
            Cos item = listCart.Find(model => model.IdArticol == id);
            //Cos marime = listCart.Find(model => model.IdMarime == id);
            if (item == null)
            {
                item = new Cos(id,marime, culoare);
                
                listCart.Add(item);
                return Redirect(strURL);
            }
            else
            {
                item.Cantitate++;
                return Redirect(strURL);
            }
        }

        

        private int Cantitate() // Obțineți numărul total de produse curente din coș
        {
            int amount = 0;
            List<Cos> listCart = Session["CosCumparaturi"] as List<Cos>;
            if (listCart != null)
            {
                amount = listCart.Sum(model => model.Cantitate);
            }
            return amount;
        }
        private double PretTotal() 
        {
            double total = 0;
            List<Cos> listCart = Session["CosCumparaturi"] as List<Cos>;
            if (listCart != null)
            {
                total = listCart.Sum(model => model.PretTotal);
            }
            return total;
        }

        


        public PartialViewResult BaraNavigare() //Afișează cantitatea de produs și banii pe bara de navigare
        {
            ViewBag.cantitateArticol = Cantitate();
            ViewBag.pretTotal = PretTotal();
            return PartialView();
        }
        public ActionResult NoICos()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Cos()
        {
            List<Cos> listCart = getCos();
            Session["CosCumparaturi"] = listCart;
            if (listCart.Count == 0)
            {
                return RedirectToAction("NoICos", "CosCumparaturi");
            }
            ViewBag.cantitateArticol = Cantitate();
            ViewBag.pretTotal = PretTotal();
            return View(listCart);
        }

        public ActionResult StergeCos(int id)
        {
            List<Cos> listCart = getCos();
            Cos item = listCart.SingleOrDefault(model => model.IdArticol == id);
            if (item != null)
            {
                listCart.RemoveAll(model => model.IdArticol == id);
                return RedirectToAction("Cos", "CosCumparaturi");
            }
            if (listCart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Cos", "CosCumparaturi");
        }
        public ActionResult ActualizatiCos(FormCollection form)
        {
            string[] cantitate = form.GetValues("cantitate");
            List<Cos> listaCos = getCos();
            for (int i = 0; i < listaCos.Count; i++)
            {
                listaCos[i].Cantitate = Convert.ToInt32(cantitate[i]);
            }
            if (Cantitate() == 0)
            {
                Session["Cos"] = null;
                return RedirectToAction("NoICos", "CosCumparaturi");
            }
            else
            {
                return RedirectToAction("Cos", "CosCumparaturi");
            }
        }


        public static string ConvertTimeTo24(string hour)
        {
            string h = "";
            switch (hour)
            {
                case "1":
                    h = "13";
                    break;
                case "2":
                    h = "14";
                    break;
                case "3":
                    h = "15";
                    break;
                case "4":
                    h = "16";
                    break;
                case "5":
                    h = "17";
                    break;
                case "6":
                    h = "18";
                    break;
                case "7":
                    h = "19";
                    break;
                case "8":
                    h = "20";
                    break;
                case "9":
                    h = "21";
                    break;
                case "10":
                    h = "22";
                    break;
                case "11":
                    h = "23";
                    break;
                case "12":
                    h = "0";
                    break;
            }
            return h;
        }
        public static string CreeazaCheie(string ch) // Generați șirul de codificare dată-oră pentru factură
        {
            string cheie = ch;
            string[] partsDay;
            partsDay = DateTime.Now.ToShortDateString().Split('/');
            //07/08/2009
            string d = String.Format("{0}{1}{2}", partsDay[0], partsDay[1], partsDay[2]);
            cheie = cheie + d;
            string[] partsTime;
            partsTime = DateTime.Now.ToLongTimeString().Split(':');
            //7:08:03 PM sau 7:08:03 AM
            if (partsTime[2].Substring(3, 2) == "PM")
                partsTime[0] = ConvertTimeTo24(partsTime[0]);
            if (partsTime[2].Substring(3, 2) == "AM")
                if (partsTime[0].Length == 1)
                    partsTime[0] = "0" + partsTime[0];
            //Eliminați spațiile albe și PM sau AM
            partsTime[2] = partsTime[2].Remove(2, 3);
            string t;
            t = String.Format("{0}{1}{2}", partsTime[0], partsTime[1], partsTime[2]);
            cheie = cheie + t;
            return cheie;
        }
        public ActionResult Verificare()
        {
            List<Cos> listaCos = getCos();
            ViewBag.cantitateArticol = Cantitate();
            ViewBag.pretTotal = PretTotal();

            return View(listaCos);
        }
        [HttpPost]
        public ActionResult Verificare(FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Factura factura = new Factura();
                    Membru membru = (Membru)Session["info"]; // Obțineți informații despre cont din sesiune
                    List<Cos> listaCos = getCos();
                    factura.NrFactura = CreeazaCheie("DS");
                    factura.IdMembru = membru.IdMembru;
                    factura.DataComanda = DateTime.Now;
                    factura.Status = true;
                    factura.DataLivrare = null;
                    factura.StatusLivrare = false;

                    // Variabila totalmney stochează cantitatea totală de produs din coș
                    int total = 0;
                    foreach (var item in listaCos)
                    {
                        total += Convert.ToInt32(item.PretTotal);
                    }
                    factura.CostTotal = total;
                    db.Facturas.Add(factura);
                    db.SaveChanges();
                    foreach (var item in listaCos)
                    {
                        DetaliiFactura df = new DetaliiFactura();
                        df.IdFactura = factura.IdFactura;
                        df.IdProdus = item.IdArticol;
                        df.CantitateProdus = item.Cantitate;
                        df.PretUnit = item.unitPret;
                        df.PretTotal = (int?)(long)item.PretTotal;
                        df.IdCuloare = item.IdCuloare;
                        df.IdMarime = item.IdMarime;
                        //df.totalDiscount = item.Discount * item.Quantity;
                        db.DetaliiFacturas.Add(df);
                    }
                    db.SaveChanges();
                    return RedirectToAction("TrimiteFactura", "CosCumparaturi");
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        //
        [HttpGet]
        public ActionResult VerificareFaraCont()
        {
            List<Cos> listaCos = getCos();
            ViewBag.cantitateArticol = Cantitate();
            ViewBag.pretTotal = PretTotal();
            return View(listaCos);
        }
        [HttpPost]
        public ActionResult VerificareFaraCont(Client client)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var verifica = db.Clients.Where(model => model.Nume == client.Nume && model.Email == client.Email).FirstOrDefault();
                    // Verificați dacă numele clientului cu e-mailul introdus există sau nu
                    if (verifica == null) // Dacă nu, salvați informațiile în tabelul client
                    { 
                        db.Clients.Add(client);
                        db.SaveChanges();
                        Factura factura = new Factura();
                        List<Cos> listaCos = getCos();
                        factura.IdClient = client.IdClient;
                        factura.NrFactura = CreeazaCheie("DS");
                        factura.DataComanda = DateTime.Now;
                        factura.Status = true;
                        factura.DataLivrare = null;
                        factura.StatusLivrare = false;
                        int total = 0;
                        foreach (var articol in listaCos)
                        {
                            total += Convert.ToInt32(articol.PretTotal);
                        }
                        factura.CostTotal = total;
                        db.Facturas.Add(factura);
                        db.SaveChanges();
                        foreach (var articol in listaCos)
                        {
                            DetaliiFactura df = new DetaliiFactura();
                            df.IdFactura = factura.IdFactura;
                            df.IdProdus = articol.IdArticol;
                            df.CantitateProdus = articol.Cantitate;
                            df.PretUnit = articol.unitPret;
                            df.PretTotal = (int?)(long)articol.PretTotal;
                            df.IdCuloare = articol.IdCuloare;
                            df.IdMarime = articol.IdMarime;
                            //df.totalDiscount = item.Discount * item.Quantity;
                            db.DetaliiFacturas.Add(df);
                        }
                        db.SaveChanges();
                        return RedirectToAction("TrimiteFactura", "CosCumparaturi");
                    }
                    else
                    {
                        Factura factura = new Factura();
                        List<Cos> listaCos = getCos();
                        factura.IdClient = verifica.IdClient;
                        factura.NrFactura = CreeazaCheie("DS");
                        factura.DataComanda = DateTime.Now;
                        factura.Status = true;
                        factura.DataLivrare = null;
                        factura.StatusLivrare = false;
                        int total = 0;
                        foreach (var articol in listaCos)
                        {
                            total += Convert.ToInt32(articol.PretTotal);
                        }
                        factura.CostTotal = total;
                        db.Facturas.Add(factura);
                        db.SaveChanges();
                        foreach (var articol in listaCos)
                        {
                            DetaliiFactura df = new DetaliiFactura();
                            df.IdFactura = factura.IdFactura;
                            df.IdProdus = articol.IdArticol;
                            df.CantitateProdus = articol.Cantitate;
                            df.PretUnit = articol.unitPret;
                            df.PretTotal = (int?)(long)articol.PretTotal;
                            df.IdCuloare = articol.IdCuloare;
                            df.IdMarime = articol.IdMarime;
                            //df.totalDiscount = item.Discount * item.Quantity;
                            db.DetaliiFacturas.Add(df);
                        }
                        db.SaveChanges();
                        return RedirectToAction("TrimiteFactura", "CosCumparaturi");
                    }

                }

                return View();
                
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public ActionResult TrimiteFactura()
        {
            ViewBag.cantitateArticol = Cantitate();
            ViewBag.pretTotal = PretTotal();
            return View();
        }
        [HttpPost]
        public ActionResult TrimiteFactura(FormCollection form)
        {
            Session.Remove("CosCumparaturi");
            //Session["CosCumparaturi"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}