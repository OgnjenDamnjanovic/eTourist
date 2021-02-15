using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTourist.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.Http;

namespace MyApp.Namespace
{
    public class ProfileModel : PageModel
    {
        private readonly IMongoCollection<Hotel> h;
        private readonly IMongoCollection<Aranzman> a;
        private readonly IMongoCollection<Korisnik> k;
        private readonly IMongoCollection<Rezervacija> r;
        private readonly IMongoCollection<Soba> s;
        [BindProperty]
        public Korisnik menadzer { get; set; }
        [BindProperty]
        public Hotel hotel { get; set; }
        [BindProperty]
        public List<Soba> sobe { get; set; }
        [BindProperty]
        public List<Aranzman> aranzmani { get; set; }
        [BindProperty]
        public List<Rezervacija> rezervacije { get; set; }
        public string Message {get; set;}

        public ProfileModel(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            h = database.GetCollection<Hotel>("hoteli");
            s = database.GetCollection<Soba>("sobe");
            k = database.GetCollection<Korisnik>("korisnici");
            a = database.GetCollection<Aranzman>("aranzmani");
            r = database.GetCollection<Rezervacija>("rezervacije");
            sobe = new List<Soba>();
            aranzmani = new List<Aranzman>();
            rezervacije = new List<Rezervacija>();
        }

        public void OnGet()
        {
            String email = HttpContext.Session.GetString("email");
            if(email!=null)
            {
                Korisnik korisnik = k.AsQueryable<Korisnik>().Where(x=>x.email == email).FirstOrDefault();
                if(korisnik.tip == 0)
                    Message = "Menadzer";
                else Message = "Admin";
            }
            
            menadzer = k.Find(x=>x.tip==0 && x.email.Equals(email)).FirstOrDefault();
            String id = menadzer.Hotel.Id.ToString();
            hotel = h.AsQueryable<Hotel>().Where(x=>x.Id == menadzer.Hotel.Id).FirstOrDefault();
            foreach(MongoDBRef sobaRef in hotel.Sobe.ToList())
            {
                sobe.Add(s.Find(x=>x.Id.Equals(sobaRef.Id)).FirstOrDefault());
            }
            foreach(MongoDBRef arRef in hotel.Aranzmani.ToList())
            {
                aranzmani.Add(a.Find(x=>x.Id.Equals(arRef.Id)).FirstOrDefault());
            }
        }

        public ActionResult OnGetSoba(string oznaka)
        {
            String email = HttpContext.Session.GetString("email");
            
            menadzer = k.Find(x=>x.tip==0 && x.email.Equals(email)).FirstOrDefault();
            hotel = h.Find(x=>x.Id.Equals(menadzer.Hotel.Id)).FirstOrDefault();

            foreach(MongoDBRef sobaRef in hotel.Sobe.ToList())
            {
                sobe.Add(s.Find(x=>x.Id.Equals(sobaRef.Id)).FirstOrDefault());
            }
            Soba soba = sobe.Where(x=>x.oznaka.Equals(oznaka)).FirstOrDefault();
            List<Rezervacija> rez = new List<Rezervacija>();
            List<Aranzman> ar = new List<Aranzman>();

            foreach(MongoDBRef rezRef in soba.Rezervacije.ToList())
            {
                rez.Add(r.Find(x=>x.Id.Equals(rezRef.Id)).FirstOrDefault());
            }
            foreach(Rezervacija Rez in rez)
            {
                ar.Add(a.Find(x=>x.Id.Equals(Rez.Aranzman.Id)).FirstOrDefault());
            }

            List<string> datum = new List<string>();
            List<string> status = new List<string>();
            List<string> pocetak = new List<string>();
            List<string> kraj = new List<string>();
            for(int i = 0; i<rez.Count; i++)
            {
                datum.Add(rez.ElementAt(i).datumKreiranja.ToString("dd.MM.yyyy."));
                status.Add(rez.ElementAt(i).status);
                pocetak.Add(ar.ElementAt(i).pocetak.ToString("dd.MM.yyyy."));
                kraj.Add(ar.ElementAt(i).kraj.ToString("dd.MM.yyyy."));
            }
            var result=new { Datum=datum, Status=status, Pocetak=pocetak, Kraj=kraj };
            return new JsonResult(result);
        }
    }
}
