using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Core.Documents.DTO
{
    public class DocumentMasterDTO
    {
        public System.Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public Byte[] FileData { get; set; }
        public System.Guid UserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FilePath { get; set; }
    }
}
