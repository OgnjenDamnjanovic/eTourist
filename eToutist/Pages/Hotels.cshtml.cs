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
    public class HotelsModel : PageModel
    {
        private readonly IMongoCollection<Hotel> h;
        private readonly IMongoCollection<Korisnik> k;
        public List<Hotel> hoteli { get; set; }
        public string Message {get; set;}

        public HotelsModel(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            h = database.GetCollection<Hotel>("hoteli");
            k = database.GetCollection<Korisnik>("korisnici");
        }

        public void OnGet(string grad, string drzava)
        {
            String email = HttpContext.Session.GetString("email");
            if(email!=null)
            {
                Korisnik korisnik = k.AsQueryable<Korisnik>().Where(x=>x.email == email).FirstOrDefault();
                if(korisnik.tip == 0)
                    Message = "Menadzer";
                else Message = "Admin";
            }

            hoteli = h.Find(x=>x.Grad.Equals(grad) && x.Drzava.Equals(drzava)).ToList();
        }
    }
}
