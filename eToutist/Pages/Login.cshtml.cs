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
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string email {get; set;}
        [BindProperty]
        public string sifra {get; set;}
        public string ErrorMessage{get; set;}
        public string Message {get; set;}
        public void OnGet()
        {
        }

        public IActionResult OnPostLogin()
        {
            var client = new MongoClient("mongodb://localhost/?safe=true");
            var db = client.GetDatabase("eTourist");
            var collection = db.GetCollection<Korisnik>("korisnici");

            /*var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("email", email),
                Builders<BsonDocument>.Filter.Eq("sifra", sifra)
            );*/

            Korisnik k = collection.AsQueryable<Korisnik>().Where(x=>x.email == email && x.sifra == sifra).FirstOrDefault();
            
            if(k!=null)
            {
                HttpContext.Session.SetString("email", email);
                return RedirectToPage("/Index");
            }
            ErrorMessage = "Invalid email address or password!";
            return Page();
        } 
    }
}
