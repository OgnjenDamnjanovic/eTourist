using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTourist.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace MyApp.Namespace
{
    public class AboutUsModel : PageModel
    {
        public string Message {get; set;}
        public void OnGet()
        {
            var client = new MongoClient("mongodb://localhost/?safe=true");
            var db = client.GetDatabase("eTourist");
            var collection = db.GetCollection<Korisnik>("korisnici");
            
            String email = HttpContext.Session.GetString("email");
            if(email!=null)
            {
                Korisnik k = collection.AsQueryable<Korisnik>().Where(x=>x.email == email).FirstOrDefault();
                if(k.tip == 0)
                    Message = "Menadzer";
                else Message = "Admin";
            }
        }
    }
}
