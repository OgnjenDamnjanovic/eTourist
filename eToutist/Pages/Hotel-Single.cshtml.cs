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
    public class Hotel_SingleModel : PageModel
    {
        private readonly IMongoCollection<Hotel> h;
        private readonly IMongoCollection<Aranzman> a;
        private readonly IMongoCollection<Soba> s;
        private readonly IMongoCollection<Korisnik> k;
        public Hotel hotel { get; set; }
        public List<Aranzman> aranzmani { get; set; }
        public List<Soba> sobe { get; set; }
        public string Message {get; set;}

        public Hotel_SingleModel(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            h = database.GetCollection<Hotel>("hoteli");
            s = database.GetCollection<Soba>("sobe");
            k = database.GetCollection<Korisnik>("korisnici");
            a = database.GetCollection<Aranzman>("aranzmani");
            sobe = new List<Soba>();
            aranzmani = new List<Aranzman>();
        }

        public void OnGet(string id)
        {
            String email = HttpContext.Session.GetString("email");
            if(email!=null)
            {
                Korisnik korisnik = k.AsQueryable<Korisnik>().Where(x=>x.email == email).FirstOrDefault();
                if(korisnik.tip == 0)
                    Message = "Menadzer";
                else Message = "Admin";
            }
            ObjectId idHotela = new ObjectId(id);
            hotel = h.Find(x=>x.Id.Equals(idHotela)).FirstOrDefault();
            foreach (MongoDBRef ar in hotel.Aranzmani.ToList())
            {
                aranzmani.Add(a.Find(x=>x.Id.Equals(ar.Id)).FirstOrDefault());
            }
            foreach (MongoDBRef soba in hotel.Sobe.ToList())
            {
                sobe.Add(s.Find(x=>x.Id.Equals(soba.Id)).FirstOrDefault());
            }
        }
    }
}
