using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CasadeModa.Models;

namespace CasadeModa.Models
{
    public class Cos
    {
        DBcasademodaEntities db = new DBcasademodaEntities();
        public int IdArticol { get; set; }
        public string NumeArticol { get; set; }
        public string ImagineArticol { get; set; }
        public int PretArticol { get; set; }
        public int unitPret { get; set; }
        public int Cantitate { get; set; }

        public int IdMarime { get; set; }
        public string DenumireMarime { get; set; }
        public int IdCuloare { get; set; }
        public string DenumireCuloare { get; set; }


        //public int Discount { get; set; }
        public Double PretTotal
        {
            get
            {
                return Cantitate * PretArticol;
            }
        }
        public Cos(int idProduct, string size, string cul)
        {
            System.Console.WriteLine(size);
            System.Console.WriteLine(cul);
            this.IdArticol = idProduct;
            //this.IdMarime= IdMarime;
            Produ item = db.Produs.Single(model => model.IdProdus == IdArticol);
            Marime marime = db.Marimes.Single(model => model.DenumireMarime == size);
            Culoare culoare = db.Culoares.Single(model => model.DenumireCuloare == cul);
            //Marime marime = db.Marimes.Single(model => model.IdMarime == IdMarime);
            this.NumeArticol = item.NumeProdus;
            this.ImagineArticol = item.Imagine;
            this.unitPret = int.Parse(item.Pret.ToString());
            this.PretArticol = int.Parse(item.Pret.ToString());
            this.Cantitate = 1;
            this.IdMarime = marime.IdMarime;
            this.DenumireMarime = marime.DenumireMarime;
            this.IdCuloare = culoare.IdCuloare;
            this.DenumireCuloare = culoare.DenumireCuloare;
            //this.IdMarime = marime.IdMarime;

        }
    }
}