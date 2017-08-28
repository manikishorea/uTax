using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.Documents.DTO;
using System.IO;

namespace EMPPortal.Core.Documents
{
    public interface IDocumentMasterService
    {
        DocumentMasterDTO SaveDocuments(DocumentMasterDTO _Dto);
        Task<DocumentMasterDTO> GetById(Guid id);
       // Stream GetBinaryData(Guid id);
    }
}
