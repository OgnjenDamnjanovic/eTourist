using MongoDB.Bson;
using MongoDB.Driver;
using eTourist.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace eTourist.Model
{
    public class Korisnik
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string email{ get; set; }
        public string sifra{ get; set; }
        public string ime{ get; set; }
        public string prezime{ get; set; }
        public string brtelefona{ get; set; }
        public string grad{ get; set; }
        public string adresa{ get; set; }
        public int tip{ get; set; }
    }
}

