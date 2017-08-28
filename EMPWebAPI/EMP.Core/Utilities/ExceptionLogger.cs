using EMPEntityFramework.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Core.Utilities
{
    public static class ExceptionLogger
    {
        static EMPDBEntities db = new EMPDBEntities();

        public static void LogException(string Message, string Method, Guid? UserId)
        {
            try
            {

                ExceptionLog log = new ExceptionLog();
                log.ExceptionMessage = Message;
                log.MethodName = Method;
                log.UserId = UserId;
                log.CreatedDateTime = DateTime.Now;
                db.ExceptionLogs.Add(log);
                db.SaveChanges();

            }
            catch (Exception)
            {

            }
        }
    }
}
