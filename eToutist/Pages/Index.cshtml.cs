using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTourist.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace NBP.Pages
{
     public class IndexModel : PageModel
    {
        public List<Hotel> popularneLokacije { get; set; }=new List<Hotel>();
        public List<Hotel> topRated { get; set; }
        public string Message {get; set;}
        

        private readonly IMongoCollection<Hotel> _dbHotels;
         private readonly IMongoCollection<Korisnik> _dbKorisnici;
      

        public IndexModel(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _dbHotels = database.GetCollection<Hotel>("hoteli");
            _dbKorisnici = database.GetCollection<Korisnik>("korisnici");
        }

        public void OnGet()
        {
           topRated= _dbHotels.AsQueryable<Hotel>().OrderByDescending(Hotel=>Hotel.brojZvezdica).ToList(); 
           //popularneLokacije=_dbHotels.AsQueryable<Hotel>().OrderByDescending(Hotel=>Hotel.Aranzmani.Count()).ToList(); 

            String email = HttpContext.Session.GetString("email");
            if(email!=null)
            {
                Korisnik k = _dbKorisnici.AsQueryable<Korisnik>().Where(x=>x.email == email).FirstOrDefault();
                if(k.tip == 0)
                    Message = "Menadzer";
                else Message = "Admin";
            }

        }
        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Remove("email");
            Message = null;
            return RedirectToPage("/Index");
            //<a asp-page="/Index" asp-page-handler="Logout">Logout</a>
        }

    }
}
