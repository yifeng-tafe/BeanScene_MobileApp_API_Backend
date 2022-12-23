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
    public class OrderController : ApiController
    {
        MongoClient client;
        string databaseName;
        string connectionString;

        //constructor
        public OrderController()
        {
            connectionString = ConfigurationManager.ConnectionStrings["BeanSceneConn"].ConnectionString;
            client = new MongoClient(connectionString);
            databaseName = MongoUrl.Create(connectionString).DatabaseName;
        }

        /// <summary>
        /// This method is to get all order information from the database
        /// </summary>
        /// <returns></returns>

        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            var collection = client.GetDatabase(databaseName).GetCollection<Order>("Order").AsQueryable();
            string jsonResult = JsonConvert.SerializeObject(collection);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }

        /// <summary>
        /// This method is to an order into the database by passing order object
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>

        // Add a order
        [Route("api/Order/addOrder")]
        public HttpResponseMessage addFood([FromBody] Order o)
        {
            try
            {
                int lastOrderId = client.GetDatabase(databaseName).GetCollection<Order>("Order").AsQueryable().Count();
                o.Id = (lastOrderId + 1).ToString();
                client.GetDatabase(databaseName).GetCollection<Order>("Order").InsertOne(o);
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
        /// This method is to get a order detail information from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        // Get a Order detail by id
        [Route("api/Order/{id}")]
        public HttpResponseMessage Get(string id)
        {
            var collection = client.GetDatabase(databaseName).GetCollection<Order>("Order");
            var filterdResult = collection.AsQueryable().Where(x => x.Id.ToLower().Contains(id)).ToList();
            string jsonResult = JsonConvert.SerializeObject(filterdResult);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }

        // Get a Order detail by status
        [Route("api/Order/{status}/{id}")]
        public HttpResponseMessage Get(string status, string id)
        {
            var collection = client.GetDatabase(databaseName).GetCollection<Order>("Order");
            var filterdResult = collection.AsQueryable().Where(x => x.OrderStatus.ToLower().Contains(id)).ToList();
            string jsonResult = JsonConvert.SerializeObject(filterdResult);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }

        // updating a order by passing food object
        [Route("api/Order/updateOrder")]
        public HttpResponseMessage updateOrder([FromBody] Order f)
        {
            try
            {
                var filter = Builders<Order>.Filter.Eq("Id", f.Id);
                var update = Builders<Order>.Update.Set("OrderTime", f.OrderTime).Set("OrderNumber", f.OrderNumber).Set("OrderDetailNumber", f.OrderDetailNumber).Set("GuestTitle", f.GuestTitle).Set("GuestName", f.GuestName).Set("AreaName", f.AreaName).Set("TableNumber", f.TableNumber).Set("NumberOfGuest", f.NumberOfGuest).Set("Requirement", f.Requirement).Set("OrderStatus", f.OrderStatus);

                client.GetDatabase(databaseName).GetCollection<Order>("Order").UpdateOne(filter, update);

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
    }
}