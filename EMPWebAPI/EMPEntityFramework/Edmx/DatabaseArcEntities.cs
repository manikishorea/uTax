using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data.Entity.Infrastructure.Interception;

namespace EMPEntityFramework.Edmx
{
    public class DatabaseArcEntities : EMPDB_ARCEntities
    {
        public DatabaseArcEntities()
        {
            this.Configuration.ProxyCreationEnabled = false;
            DbInterception.Add(new DatabaseLogger());
            // rest of you code goes here
        }
     
        public DbSet<AuditLog> Audit { get; set; }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {

                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                this.Database.Log = message => Trace.Write(exceptionMessage);

                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (DbUpdateException)
            {
                //var errorMessages = ex.Entries
                //         .SelectMany(x => x.ValidationErrors)
                //         .Select(x => x.ErrorMessage);

                // // Join the list to a single string.
                // var fullErrorMessage = string.Join("; ", errorMessages);

                // // Combine the original exception message with the new one.
                // var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // // Throw a new DbEntityValidationException with the improved exception message.
                // throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);

            }

            return 1;
        }
    }
}