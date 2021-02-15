using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Driver;
using eTourist.Model;
using Microsoft.AspNetCore.Http;

namespace MyApp.Namespace
{
    public class AdminModel : PageModel
    {
        private readonly IMongoCollection<Hotel> h;
        private readonly IMongoCollection<Aranzman> a;
        private readonly IMongoCollection<Korisnik> k;
        private readonly IMongoCollection<Rezervacija> r;
        private readonly IMongoCollection<Soba> s;
        [BindProperty]
        public List<Hotel> hoteli { get; set; }
        [BindProperty]
        public List<Korisnik> korisnici { get; set; }
        [BindProperty]
        public List<Rezervacija> rezervacije { get; set; }
        [BindProperty]
        public List<Hotel> hoteliMenadzera { get; set; }
        [BindProperty]
        public List<Aranzman> aranzmaniRezervacija { get; set; }
        [BindProperty]
        public List<Hotel> hoteliRezervacija { get; set; }
        [BindProperty]
        public List<Korisnik> menadzeriRezervacija { get; set; }
        [BindProperty]
        public List<Korisnik> menadzeri { get; set; }
        [BindProperty]
        public List<Aranzman> aranzmani { get; set; }
        [BindProperty]
        public List<Hotel> hoteliAranzmana { get; set; }
        public string Message {get; set;}

        public AdminModel(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            h = database.GetCollection<Hotel>("hoteli");
            s = database.GetCollection<Soba>("sobe");
            k = database.GetCollection<Korisnik>("korisnici");
            a = database.GetCollection<Aranzman>("aranzmani");
            r = database.GetCollection<Rezervacija>("rezervacije");
            hoteliMenadzera = new List<Hotel>();
            menadzeriRezervacija = new List<Korisnik>();
            aranzmaniRezervacija = new List<Aranzman>();
            hoteliRezervacija = new List<Hotel>();
            menadzeri = new List<Korisnik>();
            aranzmani = new List<Aranzman>();
            hoteliAranzmana = new List<Hotel>();
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


            hoteli = h.Find(x=>true).ToList();
            rezervacije = r.Find(x=>true).ToList();
            korisnici = k.Find(x=>x.tip==0).ToList();
            aranzmani = a.Find(x=>true).ToList();
            foreach(Rezervacija rez in rezervacije)
            {
                aranzmaniRezervacija.Add(a.Find(x=>x.Id.Equals(rez.Aranzman.Id)).FirstOrDefault());
                hoteliRezervacija.Add(h.Find(x=>x.Id.Equals(rez.Hotel.Id)).FirstOrDefault());
            }
            foreach(Hotel hot in hoteliRezervacija)
            {
                menadzeriRezervacija.Add(k.Find(x=>x.Hotel.Id.Equals(hot.Id)).FirstOrDefault());
            }
            foreach(Hotel hot in hoteli)
            {
                //menadzeri.Add(k.Find(x=>x.Hotel.Id.Equals(hot.Id)).FirstOrDefault());
                //string s = k.AsQueryable<Korisnik>().Select(x=>x.Hotel.Id.AsString).FirstOrDefault();
                Korisnik m = k.AsQueryable<Korisnik>().Where(x=>x.Hotel.Id == hot.Id).FirstOrDefault();
                menadzeri.Add(m);
                
            }
            foreach(Korisnik kor in menadzeri)
            {
                hoteliMenadzera.Add(h.Find(x=>x.Id.Equals(kor.Hotel.Id)).FirstOrDefault());
            }
            //PROMENI U PRIKAZU DA SE IME KORISNIKA VADI IZ REZERVACIJE, A NE IZ KORISNIKA
        }

        public IActionResult OnPostObrisiRez(string id)
        {
            List<Soba> sobe = s.Find(x=>true).ToList();
            var pull = Builders<Soba>.Update.PullFilter(x => x.Rezervacije, Builders<MongoDBRef>.Filter.Where(q => q.Id.Equals(new ObjectId(id))));
            foreach(Soba soba in sobe)
            {
                var filter = Builders<Soba>.Filter.Eq("Id",soba.Id);
                s.UpdateOne(filter,pull);
            }
            r.DeleteOne<Rezervacija>(x=>x.Id.Equals(new ObjectId(id)));
            return RedirectToPage();
        }

        public IActionResult OnPostStatusAktivno(string id)
        {
            var update = Builders<Rezervacija>.Update.Set("status", "Aktivno");
            var filter = Builders<Rezervacija>.Filter.Eq("Id", new ObjectId(id));
            r.UpdateOne(filter, update);
            return RedirectToPage();
        }

        public IActionResult OnPostObrisiHotel(string id)
        {
            Hotel hot = h.Find(x=>x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            //brisanje menadzera hotela
            var filter1 = Builders<Korisnik>.Filter.Eq("Hotel.Id", new ObjectId(id));
            k.DeleteOne(filter1);
            //brisanje soba hotela
            foreach(MongoDBRef sobaRef in hot.Sobe.ToList())
            {
                var filter2 = Builders<Soba>.Filter.Eq("Id", sobaRef.Id);
                s.DeleteOne(filter2);
            }
            //brisanje aranzmana vezanih za hotel
            foreach(MongoDBRef aranzmanRef in hot.Aranzmani.ToList())
            {
                var filter3 = Builders<Aranzman>.Filter.Eq("Id", aranzmanRef.Id);
                a.DeleteOne(filter3);
            }
            //brisanje rezervacija vezanih za taj hotel
            var filter = Builders<Rezervacija>.Filter.Eq("Hotel.Id", new ObjectId(id));
            r.DeleteMany(filter);
            //brisanje hotela
            h.DeleteOne(x=>x.Id.Equals(new ObjectId(id)));

            return RedirectToPage();
        }
    }
}
