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
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public Korisnik NoviKorisnik {get; set;}
        public string ErrorMessage {get; set;}
        public string Message {get; set;}
        public void OnGet()
        {
        }

        public IActionResult OnPostRegister()
        {
            var client = new MongoClient("mongodb://localhost/?safe=true");
            var db = client.GetDatabase("eTourist");
            var collection = db.GetCollection<Korisnik>("korisnici");

            Korisnik k = collection.AsQueryable<Korisnik>().Where(x=>x.email == NoviKorisnik.email).FirstOrDefault();

            if(k!=null)
            {
                ErrorMessage = "This email address is already used";
                return Page();
            }
            NoviKorisnik.tip = 0;
            collection.InsertOne(NoviKorisnik);
            HttpContext.Session.SetString("email", NoviKorisnik.email);
            return RedirectToPage("/AddHotel");
        }
    }
}
