using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMPAdmin.Models
{
    public class DocumentModel
    {
        public System.Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public Byte[] FileData { get; set; }
        public System.Guid UserID { get; set; }
        public DateTime CreatedDate { get; set; }

        public HttpPostedFileBase File { get; set; }
    }
}