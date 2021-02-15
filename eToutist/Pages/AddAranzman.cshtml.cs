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
    public class AddAranzmanModel : PageModel
    {
        [BindProperty]
        public string hotelID { get; set; }
        [BindProperty]
        public Hotel hotel { get; set; }
        [BindProperty]
        public DateTime pocetak { get; set; }
        [BindProperty]
        public DateTime kraj { get; set; }
        [BindProperty]
        public int cena { get; set; }
         private readonly IMongoCollection<Hotel> _dbHoteli;
        private readonly IMongoCollection<Aranzman> _dbAranzmani;
        private readonly IMongoCollection<Korisnik> _dbKorisnici;
        public string Message{ get; set; }
        public AddAranzmanModel(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _dbHoteli=database.GetCollection<Hotel>("hoteli");
            _dbAranzmani=database.GetCollection<Aranzman>("aranzmani");
            _dbKorisnici=database.GetCollection<Korisnik>("korisnici");
        }
       
        public async Task<IActionResult> OnGet(string hotelId)
        {
             String email = HttpContext.Session.GetString("email");
              if(email==null) return RedirectToPage("/Login");
            if(email!=null)
            {
                Korisnik k = _dbKorisnici.AsQueryable<Korisnik>().Where(x=>x.email == email).FirstOrDefault();
                if(k.tip == 1)
                    {return RedirectToPage("/Login");}
                else {Message = "Menadzer";}
            }

            ObjectId objId = new ObjectId(hotelId);
            hotel=await _dbHoteli.Find(hotel=>hotel.Id==objId).FirstOrDefaultAsync();
            hotelID=hotelId;
            if(hotel==null) return RedirectToPage("/Index");
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {   
            
            if(pocetak.CompareTo(kraj)>0||cena<1) return Page();
            Aranzman noviAranzman=new Aranzman();
            noviAranzman.cena=cena;
            noviAranzman.pocetak=new DateTime(pocetak.Ticks, DateTimeKind.Utc);
            noviAranzman.kraj=new DateTime(kraj.Ticks, DateTimeKind.Utc);
            noviAranzman.hotel=new MongoDBRef("hoteli",new ObjectId(hotelID));
            await _dbAranzmani.InsertOneAsync(noviAranzman);

            var update=Builders<Hotel>.Update.Push(Hotel=>Hotel.Aranzmani,new MongoDBRef("aranzmani",noviAranzman.Id));
            await _dbHoteli.UpdateOneAsync(Hotel=>Hotel.Id==new ObjectId(hotelID),update);
            return RedirectToPage("/Hotel-Single", new{id = hotelID});
        }
    }
}
