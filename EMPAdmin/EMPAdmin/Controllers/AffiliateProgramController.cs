using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EMPAdmin.Filters;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EMPAdmin.Models;
using System.IO;

namespace EMPAdmin.Controllers
{
    [SessionCheckAttribute]
    public class AffiliateProgramController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                Id = Guid.Empty.ToString();

            ViewBag.Id = Id;
            return View();
        }

        public async Task<JsonResult> SaveAffiliateProgramDoc()
        {
            string Token = Session["Token"].ToString();
            var fileToUpload = Request.Files["fileToUpload"];
            DocumentModel omodel = new DocumentModel();
            //string uplopath = ConfigurationManager.AppSettings["UploadDocumentPath"].ToString();
            //string fileName = "";
            //if (fileToUpload != null && fileToUpload.ContentLength > 0)
            //{
            //    fileName = System.IO.Path.GetFileName(fileToUpload.FileName);
            //    if (fileName != null)
            //        fileToUpload.SaveAs(uplopath + "/" + fileName);
            //}
            //return Json(uplopath + "/ " + fileName, JsonRequestBehavior.AllowGet);
            using (var client = new HttpClient())
            {
                omodel.File = fileToUpload;

                byte[] data;
                using (Stream inputStream = omodel.File.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    data = memoryStream.ToArray();
                }

                // New code:
                string APIUrl = ConfigurationManager.AppSettings["EMPAdminWebAPI"].ToString();

                client.BaseAddress = new Uri(APIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Token", Token);

                var gizmo = new DocumentModel { Id = Guid.NewGuid(), FileName = "Test", FileType = ".pdf", FileData = data, UserID = new Guid(Session["UserId"].ToString()), CreatedDate = System.DateTime.Now };
                var response = client.PostAsJsonAsync("api/Document/PostDocumentMaster", gizmo).Result;

                if (response.IsSuccessStatusCode)
                {
                    //var json = new JavaScriptSerializer();
                    //string message = await response.Content.ReadAsStringAsync();
                    //_LoginModel = json.Deserialize<LoginModel>(message);
                    var json = new JavaScriptSerializer();
                    string message = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(message);// json.Deserialize(message);
                    string retval = jsonResponse.IsSubSiteEFINAllow;
                }
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public void DownloadFile(string FilePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(Server.MapPath(FilePath)))
                {
                    if (System.IO.File.Exists(Server.MapPath(FilePath)))
                    {
                        string FileName = System.IO.Path.GetFileName(Server.MapPath(FilePath));
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName);
                        Response.ContentType = "application/octet-stream";
                        Response.WriteFile(FilePath);
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public ActionResult GetFileStream()
        {
            string Token = Session["Token"].ToString();
            using (var client = new HttpClient())
            {
                string APIUrl = ConfigurationManager.AppSettings["EMPAdminWebAPI"].ToString();

                client.BaseAddress = new Uri(APIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Token", Token);

                var response = client.GetAsync("api/Document/Download?Id=DDBEB274-04DF-4B7F-89B7-881A50183EF7").Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = new JavaScriptSerializer();
                    byte[] bytesInStream;
                    bytesInStream = response.Content.ReadAsAsync<byte[]>().Result;

                    // 
                    // using (Stream inputStream = message)
                    // {
                    //     MemoryStream memoryStream = inputStream as MemoryStream;
                    //     if (memoryStream == null)
                    //     {
                    //         memoryStream = new MemoryStream();
                    //         inputStream.CopyTo(memoryStream);
                    //     }
                    //     bytesInStream = memoryStream.ToArray();
                    // }
                    //// byte[] bytesInStream = message.ToArray(); // simpler way of converting to array
                    // message.Close();

                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=ABCD.pdf");
                    Response.BinaryWrite(bytesInStream);
                    Response.Buffer = true;
                    Response.BufferOutput = true;
                    Response.Flush();

                    // System.IO.File.WriteAllBytes(@"E:\testpdf.pdf", bytesInStream);

                    //Response.Clear();
                    //MemoryStream ms = new MemoryStream(bytesInStream);
                    //Response.ContentType = "application/pdf";
                    //Response.AddHeader("content-disposition", "attachment;filename=labtest.pdf");
                    //Response.Buffer = true;
                    //Response.BufferOutput = true;
                    //ms.WriteTo(Response.OutputStream);
                    // Response.End();
                }
                return View();
            }
        }
    }
}
