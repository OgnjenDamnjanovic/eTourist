using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace eTourist.Model
{
    public class Aranzman
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public DateTime pocetak { get; set; }
        public DateTime kraj { get; set; }
        public int cena { get; set; }
        


    }
}