using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPEntityFramework.Edmx
{
    //this file is used to log the database operations it checks
    //the query execution time and than insert if it takes more than one second.
    //to disable it remove its registry from TeamPassDbContext default constructor
    public class DatabaseLogger : IDbCommandInterceptor
    {
        static readonly ConcurrentDictionary<DbCommand,DateTime> MStartTime = new ConcurrentDictionary<DbCommand, DateTime>();

        public void NonQueryExecuted(DbCommand command,
        DbCommandInterceptionContext<int> interceptionContext)
        {
            //executed state
            Log(command, interceptionContext);
        }

        public void NonQueryExecuting(DbCommand command,
        DbCommandInterceptionContext<int> interceptionContext)
        {
            //executing state
            OnStart(command);
        }

        public void ReaderExecuted(DbCommand command,
        DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            //reader executed state
            Log(command, interceptionContext);
        }

        public void ReaderExecuting(DbCommand command,
        DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            //reader executing state
            OnStart(command);
        }

        private static void Log<T>(DbCommand command,
        DbCommandInterceptionContext<T> interceptionContext)
        {
            DateTime startTime;
            TimeSpan duration;
            //Removing from dictionary and calculating time
            MStartTime.TryRemove(command, out startTime);
            if (startTime != default(DateTime))
            {
                duration = DateTime.Now - startTime;
            }
            else
                duration = TimeSpan.Zero;

            const int requestId = -1;

            var parameters = new StringBuilder();
            foreach (DbParameter param in command.Parameters)
            {
                parameters.AppendLine(param.ParameterName + " " +
                param.DbType + " = " + param.Value);
            }

            var message = interceptionContext.Exception == null ?
            $"Database call took {duration.TotalSeconds.ToString("N3")} sec.RequestId { requestId} \r\nCommand:\r\n { parameters + command.CommandText}" :
            $"EF Database call failed after {duration.TotalSeconds.ToString("N3")} sec.RequestId { requestId} \r\nCommand:\r\n{parameters.ToString() + command.CommandText}\r\nError: { interceptionContext.Exception}";

            //Ignoring some queries which runs perfectly
            if (duration.TotalSeconds > 1 || message.Contains("EF Database call failed after "))
            {
                //The time taken is more or it contains error so adding that to database
                using (DatabaseEntities dbContext = new DatabaseEntities())
                {
                    //using error model class
                    Error error = new Error
                    {
                        TotalSeconds = (decimal)duration.TotalSeconds,
                        Active = true,
                        CommandType = Convert.ToString(command.CommandType),
                        CreateDate = DateTime.Now,
                        Exception = Convert.ToString(interceptionContext.Exception),
                        FileName = "",
                        InnerException = interceptionContext.Exception == null ?
                        "" : Convert.ToString(interceptionContext.Exception.InnerException),
                        Parameter = parameters.ToString(),
                        Query = command.CommandText,
                        RequestId = 0
                    };
                    //Adding to database
                    dbContext.Errors.Add(error);
                    dbContext.SaveChanges();
                }

                //var errorFileUrl = ;
                //File.WriteAllLines(, message);
            }
        }

        public void ScalarExecuted
        (DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            //Log and calculate after executed
            Log(command, interceptionContext);
        }

        public void ScalarExecuting
        (DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            //adding to dictionary when executing
            OnStart(command);
        }
        private static void OnStart(DbCommand command)
        {
            //adding to dictionary when executing
            MStartTime.TryAdd(command, DateTime.Now);
        }
    }

    //public class Error
    //{
    //    [Key]
    //    [Required]
    //    public int ErrorId { get; set; }

    //    public string Query { get; set; }

    //    public string Parameters { get; set; }

    //    public string CommandType { get; set; }

    //    public decimal TotalSeconds { get; set; }

    //    public string Exception { get; set; }

    //    public string InnerException { get; set; }
    //    public int RequestId { get; set; }
    //    public string FileName { get; set; }
    //    public DateTime CreateDate { get; set; }

    //    public bool Active { get; set; }
    //}
}
