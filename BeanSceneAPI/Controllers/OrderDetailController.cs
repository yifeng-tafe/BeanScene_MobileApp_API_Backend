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
    public class OrderDetailController : ApiController
    {
        MongoClient client;
        string databaseName;
        string connectionString;

        //constructor
        public OrderDetailController()
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
            var collection = client.GetDatabase(databaseName).GetCollection<OrderDetail>("OrderDetail").AsQueryable();
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
        [Route("api/OrderDetail/addOrderDetail")]
        public HttpResponseMessage addOrderDetail([FromBody] OrderDetail o)
        {
            try
            {
                int lastOrderDetailId = client.GetDatabase(databaseName).GetCollection<OrderDetail>("OrderDetail").AsQueryable().Count();
                o.Id = (lastOrderDetailId + 1).ToString();
                client.GetDatabase(databaseName).GetCollection<OrderDetail>("OrderDetail").InsertOne(o);
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

        // Get a Order detail
        [Route("api/OrderDetail/{id}")]
        public HttpResponseMessage Get(string id)
        {
            var collection = client.GetDatabase(databaseName).GetCollection<OrderDetail>("OrderDetail");
            var filterdResult = collection.AsQueryable().Where(x => x.OrderNumber.ToLower().Contains(id)).ToList();
            string jsonResult = JsonConvert.SerializeObject(filterdResult);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }

        // DELETE api/<controller>/5 -- deleting a detail order
        [Route("api/OrderDetail/deleteOrderDetail/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var collection = client.GetDatabase(databaseName).GetCollection<OrderDetail>("OrderDetail");

                var filter = Builders<OrderDetail>.Filter.Eq("OrderNumber", id);

                collection.DeleteMany(filter);
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

        // updating a order detail by passing order object
        [Route("api/OrderDetail/updateOrderDetail")]
        public HttpResponseMessage updateOrderDetail([FromBody] OrderDetail f)
        {
            try
            {
                var filter = Builders<OrderDetail>.Filter.Eq("Id", f.Id);
                var update = Builders<OrderDetail>.Update.Set("OrderNumber", f.OrderNumber).Set("FoodName", f.FoodName).Set("Quantity", f.Quantity).Set("ImageURL", f.ImageURL).Set("TotalPrice", f.TotalPrice).Set("FoodStatus", f.FoodStatus);

                client.GetDatabase(databaseName).GetCollection<OrderDetail>("OrderDetail").UpdateOne(filter, update);

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