using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanSceneAPI.Models
{
    public class Category
    {
        
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string _id { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public string imageURL { get; set; }
            
        
    }
}