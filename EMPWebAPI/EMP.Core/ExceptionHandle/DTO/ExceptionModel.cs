using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Core.ExceptionHandle.DTO
{
    public class ExceptionModel
    {
        public string Message { get; set; }
        public string MethodName { get; set; }
        public Guid UserId { get; set; }
    }
}
