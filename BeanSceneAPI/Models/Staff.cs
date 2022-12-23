﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanSceneAPI.Models
{
    public class Staff
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string _id { get; set; }
        public string id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }  
        public string username { get; set; }  
        public string password { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string imageURL { get; set; }
        public string role { get; set; }
    }
}