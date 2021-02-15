using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eTourist.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MyApp.Namespace
{
    public class AddHotelModel : PageModel
    {
        private IWebHostEnvironment  _environment;
        private readonly IMongoCollection<Korisnik> _dbKorisnici;
        private readonly IMongoCollection<Hotel> _dbHoteli;
        private readonly IMongoCollection<Soba> _dbSobe;
         private readonly IMongoCollection<Grad> _dbGradovi;
        public AddHotelModel (IWebHostEnvironment ev,IDatabaseSettings settings)
        {
            _environment=ev;
             var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _dbKorisnici = database.GetCollection<Korisnik>("korisnici");
            _dbHoteli=database.GetCollection<Hotel>("hoteli");
             _dbSobe=database.GetCollection<Soba>("sobe");
             _dbGradovi=database.GetCollection<Grad>("gradovi");
           
        }
        [BindProperty]
        public string glavnaSlika { get; set; }
        [BindProperty]
        public string slika1 { get; set; }
        [BindProperty]
        public string slika2 { get; set; }
        [BindProperty]
        public string slika3 { get; set; }
        [BindProperty]
        public Hotel noviHotel { get; set; }
        [BindProperty]
        public List<string> sobe { get; set; }
        public string Message {get; set;}
       
       public  ActionResult OnGet()
        {           
            String email = HttpContext.Session.GetString("email");
            if(email==null) return RedirectToPage("/Login");
            if(email!=null)
            {
                Korisnik k = _dbKorisnici.AsQueryable<Korisnik>().Where(x=>x.email == email).FirstOrDefault();
                if(k.tip == 1)
                return RedirectToPage("/Login");
                else {Message = "Menadzer"; }
            }
            return Page();  
        } 
       public async Task<IActionResult> OnPostAsync()
       {
          if(HttpContext.Session.GetString("email")==null)
          return RedirectToPage("/Index");
           Korisnik kor=await _dbKorisnici.Find(kor =>kor.email==HttpContext.Session.GetString("email")).FirstOrDefaultAsync();
           
           List<Soba> noveSobe=new List<Soba>();
           foreach(string soba in sobe)
           {
               Soba novaSoba=new Soba();
               string labela=soba.Substring(0,soba.LastIndexOf('|'));
               int krevetnost;
               bool uspesno=Int32.TryParse(soba.Substring(soba.LastIndexOf('|')+1), out krevetnost);
               if(!uspesno) return RedirectToPage("/Error");
               novaSoba.brojMesta=krevetnost;
               novaSoba.oznaka=labela;
               noveSobe.Add(novaSoba);
           }

            int validImageCount=0;
            if(!string.IsNullOrEmpty(slika1))
            validImageCount++;
              if(!string.IsNullOrEmpty(slika2))
            validImageCount++;
              if(!string.IsNullOrEmpty(slika3))
            validImageCount++;
             
            if(validImageCount!=3||string.IsNullOrEmpty(glavnaSlika)||!validNoviHotel())
            return RedirectToPage();

            string folderName=System.Guid.NewGuid().ToString();
            string fileName="";
            try
            {
              
                fileName=saveBase64AsImage(slika1,folderName);
                noviHotel.Slika1="images/"+folderName+"/"+fileName;
              
                fileName=saveBase64AsImage(slika2,folderName);
                noviHotel.Slika2="images/"+folderName+"/"+fileName;
              
                fileName=saveBase64AsImage(slika3,folderName);
                noviHotel.Slika3="images/"+folderName+"/"+fileName;
             
                fileName=saveBase64AsImage(glavnaSlika,folderName);
                noviHotel.GlavnaSlika="images/"+folderName+"/"+fileName;
            }
         
            catch(FormatException fe)
            {
                RedirectToPage("/Error?errorCode="+fe);
            }
            await _dbHoteli.InsertOneAsync(noviHotel);
            foreach(Soba s in noveSobe)
            {
                 s.hotel=new MongoDBRef("hoteli",noviHotel.Id);
            }
            
           
            await _dbSobe.InsertManyAsync(noveSobe);
            
            
            var update=Builders<Hotel>.Update.PushEach(Hotel=>Hotel.Sobe,noveSobe.Select(soba =>new MongoDBRef("sobe", soba.Id)));
            await _dbHoteli.UpdateOneAsync(hotel=>hotel.Id==noviHotel.Id,update);

            var filter=Builders<Korisnik>.Filter.Eq(kori =>kori.Id,kor.Id);
            var up =Builders<Korisnik>.Update.Set("Hotel",new MongoDBRef("hoteli", noviHotel.Id));
            if(_dbGradovi.Find(grad =>grad.naziv==noviHotel.Grad).FirstOrDefault()==null)
                _dbGradovi.InsertOne(new Grad{naziv=noviHotel.Grad, slika="/images/g6.jpg", opis="Jedan od najlepsih gradova u svetu"});

            await _dbKorisnici.UpdateOneAsync(filter,up);         
            return RedirectToPage("/Index");


        }

        public string saveBase64AsImage(string img,string folderName)
        {
            img=img.Substring(img.IndexOf(',') + 1);
            var imgConverted=Convert.FromBase64String(img);
            
            string imgName=System.Guid.NewGuid().ToString();

            var file = Path.Combine(_environment.ContentRootPath, "wwwroot/images/"+folderName+"/"+imgName+".jpg");
           Directory.CreateDirectory(Path.GetDirectoryName(file));
           

            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                fileStream.Write(imgConverted, 0, imgConverted.Length);
                    fileStream.Flush();
                
            }
            return imgName+".jpg";
        }
        
        public bool validNoviHotel()
        {
            if(string.IsNullOrEmpty(noviHotel.Adresa)||(string.IsNullOrWhiteSpace(noviHotel.Adresa)))
            return false;
            if(string.IsNullOrEmpty(noviHotel.Grad)||(string.IsNullOrWhiteSpace(noviHotel.Grad)))
            return false;
            if(string.IsNullOrEmpty(noviHotel.Opis)||(string.IsNullOrWhiteSpace(noviHotel.Opis)))
            return false;
            if(string.IsNullOrEmpty(noviHotel.Naziv)||(string.IsNullOrWhiteSpace(noviHotel.Naziv)))
            return false;
            if(string.IsNullOrEmpty(noviHotel.brojTelefona)||(string.IsNullOrWhiteSpace(noviHotel.brojTelefona)))
            return false;
            if(string.IsNullOrEmpty(noviHotel.Drzava)||(string.IsNullOrWhiteSpace(noviHotel.Drzava)))
            return false;
            if(noviHotel.brojZvezdica<1||noviHotel.brojZvezdica>5)
            return false;
            if(noviHotel.longitude<-180||noviHotel.longitude>180||noviHotel.latitude<-90||noviHotel.latitude>90)
            return false;
            
            return true;

        }
        

       
    }
}
