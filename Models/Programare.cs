using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CasadeModa.Models
{
    public class Programare
    {
        DBcasademodaEntities db = new DBcasademodaEntities();
        public int IdProgramare { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Ora { get; set; }
        public int IdMembru { get; set; }
        public int IdClient { get; set; }
        public string Nume { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }



        public Programare(int IdProgramare)
        {
            this.IdProgramare = IdProgramare;
           
            Programari programare = db.Programaris.Single(model => model.IdProgramare == IdProgramare);
            Client client = db.Clients.Single(model => model.IdClient == IdClient);
            this.Data = programare.Data;
            this.Ora = programare.Ora;
            this.IdMembru = int.Parse(programare.IdMembru.ToString());
            this.IdClient = int.Parse(programare.IdClient.ToString());
            this.Nume = client.Nume;
            this.Email = client.Email;
            this.Telefon = client.Telefon;



        }
    }

   
}