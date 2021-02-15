using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace eTourist.Model
{
    public class Soba
    {
        /*[BsonId]
        [BsonRepresentation(BsonType.ObjectId)]*/
        public ObjectId Id { get; set; }
        public string oznaka {get; set;}
        public List<MongoDBRef> Rezervacije { get; set; } = new List<MongoDBRef>();
        public int brojMesta { get; set; }
        public MongoDBRef hotel {get;set;}
    }
}