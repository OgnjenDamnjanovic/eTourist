using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTourist.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace NBP.Pages
{
     public class IndexModel : PageModel
    {

        private readonly IMongoCollection<Korisnik> k;
       // public var client,database;

        public IndexModel(IDatabaseSettings settings)
        {
           // var client = new MongoClient(settings.ConnectionString);
          //  var database = client.GetDatabase(settings.DatabaseName);
          //  k = database.GetCollection<Korisnik>("Korisnik");
        }

        public void OnGet()
        {
           // List<Korisnik> knjige = k.Find(book => true).ToList();
            
        }

    }
}
