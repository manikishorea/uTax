using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.IO;
using System.Configuration;


using EMPAdmin.Transactions.AffiliateProgram.DTO;
using EMPAdmin.Transactions.AffiliateProgram;
using EMPAdminWebAPI.Filters;
using System.Web.Http.Cors;

namespace EMPAdminWebAPI.APIControllers
{
    
    [TokenAuthorization]
    public class AffiliateProgramMasterController : ApiController
    {
        //private readonly IUserService _UserService;
        //private readonly uTaxDBEntities _db;
        //private readonly UserDetailDTO _user;

        //public UserMastersController(
        //   IUserService UserService, uTaxDBEntities db, UserDetailDTO user)
        //{
        //    _UserService = UserService;
        //    _db = db;
        //    _user = user;
        //}

        public AffiliateProgramService _AffiliateProgramService = new AffiliateProgramService();

        // GET: api/UserMasters
        // [Route("GetAllUser")]
        [ResponseType(typeof(AffiliateProgramDTO))]
        public IHttpActionResult GetAffiliateProgramMaster()
        {
            var user = _AffiliateProgramService.GetAll();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
            // return user;
        }

        // GET: api/UserMasters/5
        
        [ResponseType(typeof(AffiliateProgramDTO))]
        public async Task<IHttpActionResult> GetAffiliateProgramMaster(Guid id)
        {
            var data = await _AffiliateProgramService.GetById(id);

            if (data.Id == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        // POST: api/UserMasters
        [ResponseType(typeof(AffiliateProgramDTO))]
        public IHttpActionResult PostAffiliateProgramMaster(AffiliateProgramDTO data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
            string filePath = SaveDoc(hfc);
            if (!string.IsNullOrEmpty(filePath))
            {
                data.DocumentPath = filePath;
            }

            Guid id = data.Id;
            int EntityStateId = 0;
            if (id == Guid.Empty)
            {
                id = Guid.NewGuid();
                //user.Id = id;
                EntityStateId = (int)EntityState.Added;
            }
            else
            {
                EntityStateId = (int)EntityState.Modified;
            }

            int result = _AffiliateProgramService.Save(data, id, EntityStateId);
            data.Id = id;
            if (result == -1)
            {
                return StatusCode(HttpStatusCode.NotModified);
            }
            else if (result == 0)
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }
            return Ok(id);
        }

        // PUT: api/AffiliateProgramMaster/id
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUserMaster(Guid id, AffiliateProgramDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            int EntityStateId = (int)EntityState.Modified;

            Guid result = _AffiliateProgramService.SaveStatus(user, id, EntityStateId);

            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/UserMasters/5
        [ResponseType(typeof(AffiliateProgramDTO))]
        public IHttpActionResult DeleteAffiliateProgramMaster(Guid id)
        {
            bool result = _AffiliateProgramService.Delete(id);
            if (result == false)
            {
                return NotFound();
            }

            return Ok(result);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //  db.Dispose();
            }
            base.Dispose(disposing);
        }


        private string UploadFiles()
        {
            int iUploadedCnt = 0;

            // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
            string sPath = "";
            string appPath = ConfigurationManager.AppSettings["AppPath"];
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/ProductDocuments/AffiliateProgram/");

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

            // CHECK THE FILE COUNT.
            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];

                if (hpf.ContentLength > 0)
                {

                    DirectoryInfo folder = new DirectoryInfo(sPath);
                    if (!folder.Exists)
                    {
                        folder.Create();
                    }

                    // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                    if (!File.Exists(sPath + Path.GetFileName(hpf.FileName)))
                    {
                        // SAVE THE FILES IN THE FOLDER.
                        hpf.SaveAs(sPath + Path.GetFileName(hpf.FileName));
                        iUploadedCnt = iUploadedCnt + 1;
                    }
                }
            }

            // RETURN A MESSAGE (OPTIONAL).
            if (iUploadedCnt > 0)
            {
                return iUploadedCnt + " Files Uploaded Successfully";
            }
            else
            {
                return "Upload Failed";
            }
        }

        private string SaveDoc(System.Web.HttpFileCollection attachments)
        {
            int iUploadedCnt = 0;

            // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
            string sPath = "";
            string filePath = "";
            string appPath = ConfigurationManager.AppSettings["AppPath"];
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/ProductDocuments/AffiliateProgram/");

            System.Web.HttpFileCollection hfc = attachments;

            // CHECK THE FILE COUNT.
            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];

                if (hpf.ContentLength > 0)
                {
                    DirectoryInfo folder = new DirectoryInfo(sPath);
                    if (!folder.Exists)
                    {
                        folder.Create();
                    }

                    // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                    if (!File.Exists(sPath + Path.GetFileName(hpf.FileName)))
                    {
                        // SAVE THE FILES IN THE FOLDER.
                        hpf.SaveAs(sPath + Path.GetFileName(hpf.FileName));
                        filePath = sPath + Path.GetFileName(hpf.FileName);
                        iUploadedCnt = iUploadedCnt + 1;
                    }
                }
            }

            // RETURN A MESSAGE (OPTIONAL).
            if (iUploadedCnt > 0)
            {
                return filePath;
            }
            else
            {
                return "";
            }
        }
    }
}