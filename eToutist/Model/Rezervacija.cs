using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace eTourist.Model
{
    public class Rezervacija
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime datumKreiranja { get; set; }
        public string status { get; set; }
        [JsonIgnore]
        public MongoDBRef Aranzman { get; set; }
        
    }
}