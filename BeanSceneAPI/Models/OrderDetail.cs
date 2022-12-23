using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanSceneAPI.Models
{
    public class OrderDetail
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string _id { get; set; }
        public string Id { get; set; }
        public string OrderNumber { get; set; }
        public string FoodName { get; set; }
        public string ImageURL { get; set; }
        public string Quantity { get; set; }
        public string TotalPrice { get; set; }
        public string FoodStatus { get; set; }
        
        
    }
}