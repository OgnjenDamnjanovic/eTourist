using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace eTourist.Model
{
    public class Rezervacija
    {
        /*[BsonId]
        [BsonRepresentation(BsonType.ObjectId)]*/
        public ObjectId Id { get; set; }
        public DateTime datumKreiranja { get; set; }
        public string status { get; set; } = "Na cekanju";
        public MongoDBRef Aranzman { get; set; }
        public MongoDBRef Hotel {get; set;}
        public string Ime {get; set;}
        public string Prezime {get; set;}
        public string BrojPasosa {get; set;}
        public string BrojTelefona {get; set;}
    }
}