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

namespace BeanSceneAPI.Controllers
{
    [BasicAuthentication]
    public class FoodController : ApiController
    {
        MongoClient client;
        string databaseName;
        string connectionString;

        //constructor
        public FoodController()
        {
            connectionString = ConfigurationManager.ConnectionStrings["BeanSceneConn"].ConnectionString;
            client = new MongoClient(connectionString);
            databaseName = MongoUrl.Create(connectionString).DatabaseName;
        }

        /// <summary>
        /// This method is to get all food information from the database
        /// </summary>
        /// <returns></returns>

        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            var collection = client.GetDatabase(databaseName).GetCollection<Food>("Food").AsQueryable();
            string jsonResult = JsonConvert.SerializeObject(collection);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }

        /// <summary>
        /// This method is to search a food detail information from the database by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        // Search Food
        [Route("api/Food/{name}")]
        public HttpResponseMessage Get(string name)
        {
            var collection = client.GetDatabase(databaseName).GetCollection<Food>("Food");
            var filterdResult = collection.AsQueryable().Where(x => x.Name.ToLower().Contains(name)).ToList();
            string jsonResult = JsonConvert.SerializeObject(filterdResult);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }

        /// <summary>
        /// This method is to get all food in a category from the database 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <returns></returns>

        // Get all food from a Category
        [Route("api/Food/{category}/{name}")]
        public HttpResponseMessage Get(string category, string name)
        {
            var collection = client.GetDatabase(databaseName).GetCollection<Food>("Food");
            var filterdResult = collection.AsQueryable().Where(x => x.Category_Id == name).ToList();
            string jsonResult = JsonConvert.SerializeObject(filterdResult);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }

        /// <summary>
        /// This method is to update a food into the database
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>

        // updating a food by passing food object
        [Route("api/Food/updateFood")]
        public HttpResponseMessage updateFood([FromBody] Food f)
        {
            try
            {
                var filter = Builders<Food>.Filter.Eq("Id", f.Id);
                var update = Builders<Food>.Update.Set("Name", f.Name).Set("Description", f.Description).Set("Price", f.Price).Set("ImageURL", f.ImageURL).Set("Category_Id", f.Category_Id);

                client.GetDatabase(databaseName).GetCollection<Food>("Food").UpdateOne(filter, update);

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
        /// This method is to add a food into the database
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>

        // Add a food
        [Route("api/Food/addFood")]
        public HttpResponseMessage addFood([FromBody] Food f)
        {
            try
            {
                int lastFoodId = client.GetDatabase(databaseName).GetCollection<Food>("Food").AsQueryable().Count();
                f.Id = (lastFoodId + 1).ToString();
                client.GetDatabase(databaseName).GetCollection<Food>("Food").InsertOne(f);
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
        /// This method is to delete a food from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        // DELETE api/<controller>/5 -- deleting a food
        [Route("api/Food/deleteFood/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var collection = client.GetDatabase(databaseName).GetCollection<Food>("Food");

                var filter = Builders<Food>.Filter.Eq("Id", id);

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