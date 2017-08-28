using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EMP.Core.Documents.DTO;
using EMP.Core.Documents;
using System.Web;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using EMPAdmin.Transactions.User.DTO;
using EMPAdmin.Transactions.User;

namespace EMPAdminWebAPI.APIControllers
{
    public class DocumentController : ApiController
    {
        DocumentMasterService _documentmasterService = new DocumentMasterService();

        [ActionName("Download")]
        public async Task<IHttpActionResult> GetDocument(string Id)
        {
            Guid GId;
            if (!Guid.TryParse(Id, out GId))
            {
                return Ok(Id);
            }

            string filename = "";
            DocumentMasterDTO omodel = await _documentmasterService.GetById(GId);
            if (omodel == null)
            {
                return NotFound();
            }
            else
            {
                filename = GId.ToString().Replace("-", "").Replace(" ", "") + "_" + omodel.FileName.Replace("-", "").Replace(" ", "");
                if (!File.Exists(ConfigurationManager.AppSettings["DownloadPath"].ToString() + filename))
                {
                    System.IO.File.WriteAllBytes(ConfigurationManager.AppSettings["DownloadPath"].ToString() + filename, omodel.FileData);
                }
            }
            filename = "Download\\" + filename;
            return Ok(filename);
        }

        [HttpPost]
        [ActionName("SaveAjaxDocuments")]
        [ResponseType(typeof(DocumentMasterDTO))]
        public IHttpActionResult PostDocumentMaster()
        {
            DocumentMasterDTO _dto = new DocumentMasterDTO();
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                // Get the uploaded image from the Files collection  
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];
                if (httpPostedFile != null)
                {
                    FileUpload imgupload = new FileUpload();
                    int length = httpPostedFile.ContentLength;
                    _dto.FileType = Path.GetExtension(httpPostedFile.FileName);
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(httpPostedFile.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(httpPostedFile.ContentLength);
                    }

                    if (HttpContext.Current.Request["Id"] != "")
                    {
                        Guid Id;
                        if (!Guid.TryParse(HttpContext.Current.Request["Id"], out Id))
                        {
                            _dto.Id = Guid.Empty;
                        }
                        else
                        {
                            _dto.Id = new Guid(HttpContext.Current.Request["Id"]);
                        }
                    }
                    else
                    {
                        _dto.Id = Guid.Empty;
                    }

                    _dto.FileName = Path.GetFileName(httpPostedFile.FileName);
                    _dto.FileData = fileData;// new byte[length];
                    _dto.UserID = new Guid(HttpContext.Current.Request["UserID"]);
                    var user = _documentmasterService.SaveDocuments(_dto);



                    // Here you can use EF with an entity with a byte[] property, or
                    // an stored procedure with a varbinary parameter to insert the
                    // data into the DB

                    //var result
                    //    = string.Format("Received '{0}' with length: {1}", fileName, file.Length);
                    //return result;

                    //imgupload.imagedata = new byte[length]; //get imagedata  
                    //  httpPostedFile.InputStream.Read(imgupload.imagedata, 0, length);
                    //  imgupload.imagename = Path.GetFileName(httpPostedFile.FileName);
                    //   db.fileUpload.Add(imgupload);
                    //  db.SaveChanges();
                    //  var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedFiles"), httpPostedFile.FileName);
                    // Save the uploaded file to "UploadedFiles" folder  
                    //   httpPostedFile.SaveAs(fileSavePath);
                    return Ok(user.Id);
                }
            }
            return Ok("NoFile");
        }
        
        //[HttpPost]
        //[ActionName("Upload")]
        //[ResponseType(typeof(string))]
        //public async Task<IHttpActionResult> PostUploadFile()
        //{
        //    var provider = new MultipartMemoryStreamProvider();
        //    await Request.Content.ReadAsMultipartAsync(provider);

        //    // extract file name and file contents
        //    var fileNameParam = provider.Contents[0].Headers.ContentDisposition.Parameters
        //        .FirstOrDefault(p => p.Name.ToLower() == "filename");
        //    string fileName = (fileNameParam == null) ? "" : fileNameParam.Value.Trim('"');
        //    byte[] file = await provider.Contents[0].ReadAsByteArrayAsync();

        //    // Here you can use EF with an entity with a byte[] property, or
        //    // an stored procedure with a varbinary parameter to insert the
        //    // data into the DB

        //    var result
        //        = string.Format("Received '{0}' with length: {1}", fileName, file.Length);
        //    //return result;
        //    return Ok(result);
        //}
    }
}
