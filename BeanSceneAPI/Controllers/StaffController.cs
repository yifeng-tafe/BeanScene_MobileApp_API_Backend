using BeanSceneAPI.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using BCrypt.Net;

namespace BeanSceneAPI.Controllers
{
    [BasicAuthentication]
    public class StaffController : ApiController
    {
        MongoClient client;
        string databaseName;
        string connectionString;

        //constructor
        public StaffController()
        {
            connectionString = ConfigurationManager.ConnectionStrings["BeanSceneConn"].ConnectionString;
            client = new MongoClient(connectionString);
            databaseName = MongoUrl.Create(connectionString).DatabaseName;
        }
        
        
        /// <summary>
        /// This method is to get all the staff information from the databasse
        /// </summary>
        /// <returns></returns>
        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            var collection = client.GetDatabase(databaseName).GetCollection<Staff>("Staff").AsQueryable();
            string jsonResult = JsonConvert.SerializeObject(collection);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }

        /// <summary>
        /// This method is to get the username from the database and verify the password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        [Route("api/Staff/{username}/{password}")]
        public HttpResponseMessage Get(string username, string password)
        {
            var collection = client.GetDatabase(databaseName).GetCollection<Staff>("Staff");
            var result = collection.Find(x => x.username == username).FirstOrDefault();

            if (result != null)
            {
                try
                {
                    bool verified = BCrypt.Net.BCrypt.Verify(password, result.password);
                    if (!verified)
                    {
                        result = null;
                    }
                }
                catch (Exception ex)
                {
                    result = null;
                }
            }

            string jsonResult = JsonConvert.SerializeObject(result);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }

        /// <summary>
        /// This method is to search a staff by name in the database 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        // Search Staff
        [Route("api/Staff/{name}")]
        public HttpResponseMessage Get(string name)
        {
            var collection = client.GetDatabase(databaseName).GetCollection<Staff>("Staff");
            var filterdResult = collection.AsQueryable().Where(x => x.firstname.ToLower().Contains(name)).ToList();
            string jsonResult = JsonConvert.SerializeObject(filterdResult);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }

        /// <summary>
        /// This method is to update a staff by passing staff object
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>

        // updating a staff by passing staff object
        [Route("api/Staff/updateStaff")]
        public HttpResponseMessage updateStaff([FromBody] Staff s)
        {
            try
            {
                var filter = Builders<Staff>.Filter.Eq("id", s.id);
                var update = Builders<Staff>.Update.Set("firstname", s.firstname).Set("lastname", s.lastname).Set("email", s.email).Set("mobile", s.mobile).Set("imageURL", s.imageURL);
                client.GetDatabase(databaseName).GetCollection<Staff>("Staff").UpdateOne(filter, update);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
        }

        /// <summary>
        /// This method is to add a staff into the database
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>

        // adding a staff by passing staff object
        [Route("api/Staff/addStaff")]
        public HttpResponseMessage addStaff([FromBody] Staff s)
        {
            try
            {
                // hashing the password entered by the user using BCrypt
                string hashPassword = BCrypt.Net.BCrypt.HashPassword(s.password);
                s.password = hashPassword;

                int lastProductId = client.GetDatabase(databaseName).GetCollection<Staff>("Staff").AsQueryable().Count();
                s.id = (lastProductId + 1).ToString();
                client.GetDatabase(databaseName).GetCollection<Staff>("Staff").InsertOne(s);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);

                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");

                return response;

            }

        }

        /// <summary>
        /// This method is to delete a staff from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        // DELETE api/<controller>/5 -- deleting a staff
        [Route("api/Staff/deleteStaff/{id}")]
        public HttpResponseMessage deleteStaff(int id)
        {
            try
            {
                var collection = client.GetDatabase(databaseName).GetCollection<Staff>("Staff");
                var filter = Builders<Staff>.Filter.Eq("id", id);
                collection.DeleteOne(filter);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                var responsse = Request.CreateResponse(HttpStatusCode.BadRequest);
                var jObject = new JObject();
                responsse.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return responsse;
            }
        }
    }
}