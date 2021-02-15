using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace eTourist.Model
{
    public class Hotel
    {
        /*[BsonId]
        [BsonRepresentation(BsonType.ObjectId)]*/
        public ObjectId Id { get; set; }
        public string Naziv {get; set;}
        public string Adresa {get; set;}
        public string Grad { get; set; }
        public string Drzava { get; set; }
        public int brojZvezdica { get; set; }
        public string brojTelefona { get; set; }
        public List<MongoDBRef> Aranzmani { get; set; } = new List<MongoDBRef>();
        public string Opis { get; set; }
        public string GlavnaSlika { get; set; }
        public string Slika1 { get; set; }
        public string Slika2 { get; set; }
        public string Slika3 { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public List<MongoDBRef> Sobe { get; set; } = new List<MongoDBRef>();
    }
}