using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.Documents.DTO;
using EMPEntityFramework.Edmx;
using System.IO;
using System.Data.Entity;
using EMPPortal.Core.Documents;

namespace EMP.Core.Documents
{
    public class DocumentMasterService : IDocumentMasterService
    {
        public DatabaseEntities db = new DatabaseEntities();
        
        public DocumentMasterDTO oDocsDto = new DocumentMasterDTO();

        public DocumentMasterDTO SaveDocuments(DocumentMasterDTO _Dto)
        {
            int entityState = 0;
            DocumentMaster odocumentmaster = new DocumentMaster();
            if (_Dto.FileData.Length > 0)
            {
                odocumentmaster = db.DocumentMasters.Where(a => a.Id == _Dto.Id).FirstOrDefault();
                if (odocumentmaster != null)
                {
                    entityState = (int)System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    odocumentmaster = new DocumentMaster();
                    odocumentmaster.Id = Guid.NewGuid();
                    entityState = (int)System.Data.Entity.EntityState.Added;
                }
                odocumentmaster.FileName = _Dto.FileName.Replace("-", "").Replace(" ", "");
                odocumentmaster.FileType = _Dto.FileType;
                odocumentmaster.FileData = _Dto.FileData;
                odocumentmaster.UserID = _Dto.UserID;
                odocumentmaster.CreatedDate = System.DateTime.Now;
                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    db.DocumentMasters.Add(odocumentmaster);
                }
                else
                {
                    db.Entry(odocumentmaster).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                _Dto.Id = odocumentmaster.Id;
            }
            return _Dto;
        }

        public async Task<DocumentMasterDTO> GetById(Guid id)
        {
            db = new DatabaseEntities();
            var data = await db.DocumentMasters.Where(o => o.Id == id).Select(o => new DocumentMasterDTO
            {
                Id = o.Id,
                FileName = o.FileName,
                FileType = o.FileType,
                FileData = o.FileData,
                UserID = o.UserID,
                CreatedDate = o.CreatedDate
            }).FirstOrDefaultAsync();
            return data;
        }

        //public Stream GetBinaryData(Guid id)
        //{
        //    var r = (from a in db.DocumentMasters where a.Id == id select a).First();
        //    return new MemoryStream(r.FileData.ToArray());
        //}

        //public byte[] GetByteData(Guid id)
        //{
        //    var r = (from a in db.DocumentMasters where a.Id == id select a).First();
        //    return (r.FileData);
        //}

        //public async Task<string> UploadFile()
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
        //    return result;
        //}
    }
}
