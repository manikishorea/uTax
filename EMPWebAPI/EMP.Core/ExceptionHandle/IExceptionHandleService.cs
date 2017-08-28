using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.ExceptionHandle.DTO;

namespace EMP.Core.ExceptionHandle
{
    public interface IExceptionHandleService
    {
        bool LogException(ExceptionModel dto);
    }
}
