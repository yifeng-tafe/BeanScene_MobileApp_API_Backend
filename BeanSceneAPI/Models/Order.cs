using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanSceneAPI.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string _id { get; set; }
        public string Id { get; set; }
        public string OrderTime { get; set; }   
        public string OrderNumber { get; set; }
        public string OrderDetailNumber { get; set; }
        public string GuestTitle { get; set; }
        public string GuestName { get; set; }
        public string AreaName { get; set; }
        public string TableNumber { get; set; }
        public string NumberOfGuest { get; set; }
        public string Requirement { get; set; }
        public string OrderStatus { get; set; }
        
    }
}