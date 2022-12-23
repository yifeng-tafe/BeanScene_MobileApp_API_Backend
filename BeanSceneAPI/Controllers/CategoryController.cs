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
    public class CategoryController : ApiController
    {
        MongoClient client;
        string databaseName;
        string connectionString;

        //constructor
        public CategoryController()
        {
            connectionString = ConfigurationManager.ConnectionStrings["BeanSceneConn"].ConnectionString;
            client = new MongoClient(connectionString);
            databaseName = MongoUrl.Create(connectionString).DatabaseName;
        }

        /// <summary>
        /// This method is to get all the categories information from the database
        /// </summary>
        /// <returns></returns>

        // GET api/<controller>
        public HttpResponseMessage Get()
        
        {
            var collection = client.GetDatabase(databaseName).GetCollection<Category>("Category").AsQueryable();
            string jsonResult = JsonConvert.SerializeObject(collection);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }

        // Search Category by Id
        [Route("api/Category/{id}")]
        public HttpResponseMessage Get(string id)
        {
            var collection = client.GetDatabase(databaseName).GetCollection<Category>("Category");
            var filterdResult = collection.AsQueryable().Where(x => x.id.ToLower().Contains(id)).ToList();
            string jsonResult = JsonConvert.SerializeObject(filterdResult);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }

        // Add a category
        [Route("api/Category/addCategory")]
        public HttpResponseMessage addCategory([FromBody] Category c)
        {
            try
            {
                int lastCategoryId = client.GetDatabase(databaseName).GetCollection<Category>("Category").AsQueryable().Count();
                c.id = (lastCategoryId + 1).ToString();
                client.GetDatabase(databaseName).GetCollection<Category>("Category").InsertOne(c);
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

        // Delete a category
        [Route("api/Category/deleteCategory/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var collection = client.GetDatabase(databaseName).GetCollection<Category>("Category");

                var filter = Builders<Category>.Filter.Eq("id", id);

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