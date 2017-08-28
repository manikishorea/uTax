using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace EMPAdmin.Utilities
{
    public class WebCall
    {
        public static async Task<HttpResponseMessage> PostBson<T>(string url, T data)
        {
            using (var client = new HttpClient())
            {
                //Specifiy 'Accept' header As BSON: to ask server to return data as BSON format
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                //Specify 'Content-Type' header: to tell server which format of the data will be posted
                //Post data will be as Bson format                
                var bSonData = SerializeBson<T>(data);
                var byteArrayContent = new ByteArrayContent(bSonData);
                byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/bson");

                var response = await client.PostAsync(url, byteArrayContent);

                response.EnsureSuccessStatusCode();

                return response;
            }
        }

        public static byte[] SerializeBson<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BsonWriter writer = new BsonWriter(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, obj);
                }

                return ms.ToArray();
            }
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:9000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP GET
                HttpResponseMessage response = await client.GetAsync("api/products/1");
                if (response.IsSuccessStatusCode)
                {
                    testModel product = await response.Content.ReadAsAsync<testModel>();
                    Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                }

                // HTTP POST
                var gizmo = new testModel() { Name = "Gizmo", Price = 100, Category = "Widget" };
                response = await client.PostAsJsonAsync("api/products", gizmo);
                if (response.IsSuccessStatusCode)
                {
                    Uri gizmoUrl = response.Headers.Location;

                    // HTTP PUT
                    gizmo.Price = 80;   // Update price
                    response = await client.PutAsJsonAsync(gizmoUrl, gizmo);

                    // HTTP DELETE
                    response = await client.DeleteAsync(gizmoUrl);
                }
            }
        }
    }

    public class testModel {
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
    }
}