using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace eTourist.Model
{
    public class Soba
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
       public List<MongoDBRef> Rezervacije { get; set; }
       public int brojMesta { get; set; }
        
    }
}