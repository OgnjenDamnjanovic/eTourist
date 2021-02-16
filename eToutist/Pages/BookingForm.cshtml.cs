using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTourist.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.Http;

namespace MyApp.Namespace
{
    
    public class BookingFormModel : PageModel
    {
        public string Message {get; set;}
         [BindProperty]
        public Hotel Hotel { get; set; }
         [BindProperty]
        public Aranzman aranzman { get; set; }

        [BindProperty]
        public string HotelId { get; set; }
         [BindProperty]
        public string aranzmanId { get; set; }
        [BindProperty]
        public string ime { get; set; }
        [BindProperty]
        public string Prezime { get; set; }
        [BindProperty]
        public string brojPasosa { get; set; }
        [BindProperty]
        public string brTelefona { get; set; }
        public SelectList kapacitet { get; set; }
        [BindProperty]
        public int brojMesta { get; set; }
         private readonly IMongoCollection<Hotel> _dbHoteli;
        private readonly IMongoCollection<Aranzman> _dbAranzmani;
        private readonly IMongoCollection<Rezervacija> _dbRezervacije;
        private readonly IMongoCollection<Soba> _dbSobe;
         private readonly IMongoCollection<Korisnik> _dbKorisnici;
        public BookingFormModel(IDatabaseSettings settings)
        {
             var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _dbHoteli=database.GetCollection<Hotel>("hoteli");
            _dbAranzmani=database.GetCollection<Aranzman>("aranzmani");
            _dbRezervacije=database.GetCollection<Rezervacija>("rezervacije");
            _dbSobe=database.GetCollection<Soba>("sobe");
            _dbKorisnici=database.GetCollection<Korisnik>("korisnici");
        }
        public async Task<IActionResult> OnGet(string id, string hotel)
        {   
             String email = HttpContext.Session.GetString("email");
            if(email!=null)
            {
                Korisnik k = _dbKorisnici.AsQueryable<Korisnik>().Where(x=>x.email == email).FirstOrDefault();
                if(k.tip == 0)
                    Message = "Menadzer";
                else Message = "Admin";
            }
            
            Hotel=await _dbHoteli.Find(h =>h.Id==new ObjectId(hotel)).FirstOrDefaultAsync();
            aranzman=await _dbAranzmani.Find(a =>a.Id==new ObjectId(id)).FirstOrDefaultAsync();
            if(hotel==null||aranzman==null) 
            return RedirectToPage("/Index");
            HotelId=hotel;
            aranzmanId=id;
            // List<Aranzman> zav=_dbAranzmani.Find(ar=>true).ToList();
           List<Aranzman> zabranjeniAranzmani=_dbAranzmani.Find(ar=>ar.hotel.Id==Hotel.Id&&
                                (
                                (ar.pocetak.CompareTo(aranzman.pocetak)>=0&&ar.pocetak.CompareTo(aranzman.kraj)<=0)||
                                (ar.kraj.CompareTo(aranzman.pocetak)>=0&&ar.kraj.CompareTo(aranzman.kraj)<=0)||
                                (ar.pocetak.CompareTo(aranzman.pocetak)<0&&ar.kraj.CompareTo(aranzman.kraj)>0)
                                )).ToList();
            List<Rezervacija> zabranjeneRezervacije=new List<Rezervacija>();
            foreach(Aranzman ar in zabranjeniAranzmani)
            {
                zabranjeneRezervacije.AddRange(_dbRezervacije.Find(rez =>rez.Aranzman.Id==ar.Id).ToList());
            }
            
           List<Soba> dozvoljeneSobe=new List<Soba>();
           List<Soba> sveSobe=_dbSobe.Find(soba => soba.hotel.Id==Hotel.Id).ToList();
           
           /*dozvoljeneSobe.AddRange(_dbSobe.Find(soba => soba.hotel.Id==Hotel.Id&&soba.Rezervacije.Contains(rez.Id)).ToList()); */
               
           foreach(Soba s in sveSobe)
           {
                bool ok=true;
                        foreach(Rezervacija rez in zabranjeneRezervacije)
                        {
                            if(s.Rezervacije.Contains(new MongoDBRef("rezervacije", rez.Id)))
                            {
                                ok=false;
                                break;
                            }
                        }
                if(ok==true)
                dozvoljeneSobe.Add(s);        
           }
            
            if(zabranjeneRezervacije.Count!=0)
                kapacitet=new SelectList(dozvoljeneSobe.Select(soba =>soba.brojMesta).Distinct());
                else
               kapacitet=new SelectList(_dbSobe.Distinct(s =>s.brojMesta,soba => soba.hotel.Id==Hotel.Id).ToList());
                
           
            return Page();
        }
        
        public async Task<IActionResult> OnPost()
        {  if(String.IsNullOrEmpty(ime)||String.IsNullOrEmpty(Prezime)||String.IsNullOrEmpty(brTelefona)||String.IsNullOrEmpty(brojPasosa)||brojMesta==0)
            return  Page();

            Hotel=await _dbHoteli.Find(h =>h.Id==new ObjectId(HotelId)).FirstOrDefaultAsync();
            aranzman=await _dbAranzmani.Find(a =>a.Id==new ObjectId(aranzmanId)).FirstOrDefaultAsync();
            List<Aranzman> zabranjeniAranzmani=_dbAranzmani.Find(ar=>ar.hotel.Id.Equals(Hotel.Id)&&
                                (
                                (ar.pocetak.CompareTo(aranzman.pocetak)>=0&&ar.pocetak.CompareTo(aranzman.kraj)<=0)||
                                (ar.kraj.CompareTo(aranzman.pocetak)>=0&&ar.kraj.CompareTo(aranzman.kraj)<=0)||
                                (ar.pocetak.CompareTo(aranzman.pocetak)<0&&ar.kraj.CompareTo(aranzman.kraj)>0)
                                )).ToList();

            List<Rezervacija> zabranjeneRezervacije=new List<Rezervacija>();
            foreach(Aranzman ar in zabranjeniAranzmani)
            {
                zabranjeneRezervacije.AddRange(_dbRezervacije.Find(rez =>rez.Aranzman.Id==ar.Id).ToList());
            }
            Soba sobaZaRezervisanje=null;
            List<Soba> sveSobe=_dbSobe.Find(Soba=>Soba.hotel.Id==Hotel.Id&&Soba.brojMesta==this.brojMesta).ToList();
            foreach(Soba soba in sveSobe)
            {   
                bool nadjeno=true;
                foreach(Rezervacija rezervacija in zabranjeneRezervacije)
                {
                    if(soba.Rezervacije.Contains(new MongoDBRef("rezervacije", rezervacija.Id)))
                    {
                        nadjeno=false;
                        break;
                    }
                }
                if(nadjeno)
                {
                    sobaZaRezervisanje=soba;
                    break;
                }

            }


            if(sobaZaRezervisanje==null){
                return RedirectToPage("/Error");
            }

            Rezervacija novaRezervacija=new Rezervacija();
            novaRezervacija.Ime=ime;
            novaRezervacija.Prezime=Prezime;
            novaRezervacija.BrojPasosa=brojPasosa;
            novaRezervacija.BrojTelefona=brTelefona;
            novaRezervacija.datumKreiranja=DateTime.Now;
            novaRezervacija.Aranzman=new MongoDBRef("aranzmani",aranzman.Id);
            novaRezervacija.Hotel=new MongoDBRef("hoteli",Hotel.Id );
            _dbRezervacije.InsertOne(novaRezervacija);

            var update=Builders<Soba>.Update.Push(soba=>soba.Rezervacije,new MongoDBRef("rezervacije", novaRezervacija.Id));
            await _dbSobe.UpdateOneAsync(soba=>soba.Id==sobaZaRezervisanje.Id,update);


            return RedirectToPage("/Index");
        }
        
    }
}
