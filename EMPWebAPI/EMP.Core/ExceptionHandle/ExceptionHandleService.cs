using System;
using System.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Data;

using System.Threading.Tasks;

using System.Data.Entity;

using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using EMP.Core.Utilities;

using EMP.Core.ExceptionHandle.DTO;
using EMPEntityFramework.Edmx;
using EMPPortal.Core.Utilities;

namespace EMP.Core.ExceptionHandle
{
    public class ExceptionHandleService : IExceptionHandleService, IDisposable
    {
        public DatabaseEntities db = new DatabaseEntities();

        public bool LogException(ExceptionModel dto)
        {
            ExceptionLogger.LogException(dto.Message, dto.MethodName, dto.UserId);
            return true;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}