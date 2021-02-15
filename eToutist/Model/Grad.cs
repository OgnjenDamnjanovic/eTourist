using MongoDB.Bson;

namespace eTourist.Model
{
    public class Grad
    {
        public ObjectId Id {get; set;}
        public string naziv {get; set;}
        public string slika {get; set;}
        public string opis {get; set;}
    }
}