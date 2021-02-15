using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTourist.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MyApp.Namespace
{
    public class DestinationsModel : PageModel
    {
        public List<Drzava> listaDrzava {get; set;}
        public string Message {get; set;}
        public void OnGet()
        {
            var client = new MongoClient("mongodb://localhost/?safe=true");
            var db = client.GetDatabase("eTourist");
            var collection = db.GetCollection<Hotel>("hoteli");
            var collectionGradovi = db.GetCollection<Grad>("gradovi");
            var collectionKorisnici = db.GetCollection<Korisnik>("korisnici");

            String email = HttpContext.Session.GetString("email");
            if(email!=null)
            {
                Korisnik k = collectionKorisnici.AsQueryable<Korisnik>().Where(x=>x.email == email).FirstOrDefault();
                if(k.tip == 0)
                    Message = "Menadzer";
                else Message = "Admin";
            }

            //List<string> sveDrzave = collection.AsQueryable<Hotel>().OrderBy(x=>x.Drzava).Select(x=>x.Drzava).Distinct().ToList();
            List<string> sveDrzave = collection.AsQueryable<Hotel>().Select(x=>x.Drzava).Distinct().ToList();
            listaDrzava = new List<Drzava>();

            foreach(string drzava in sveDrzave)
            {
                Drzava d = new Drzava();
                d.naziv = drzava;
                d.gradovi = new List<Grad>();
                //d.gradovi = collection.AsQueryable<Hotel>().Where(x=>x.Drzava == drzava).OrderBy(x=>x.Grad).Select(x=>x.Grad).Distinct().ToList();
                List<string> gradoviDrzave = collection.AsQueryable<Hotel>().Where(x=>x.Drzava == drzava).Select(x=>x.Grad).Distinct().ToList();
                foreach(string grad in gradoviDrzave)
                {
                    Grad g = collectionGradovi.AsQueryable<Grad>().Where(x=>x.naziv == grad).FirstOrDefault();
                    d.gradovi.Add(g);
                }
                listaDrzava.Add(d);
            }

        }
    }
}
