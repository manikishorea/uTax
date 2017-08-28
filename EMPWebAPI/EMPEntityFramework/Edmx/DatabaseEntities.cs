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
    public class DatabaseEntities : EMPDBEntities
    {
        public DatabaseEntities()
        {
            this.Configuration.ProxyCreationEnabled = false;
            //this.Database.Log = message => Trace.Write(message);
            DbInterception.Add(new DatabaseLogger());
            // rest of you code goes here
        }

        private AuditTrailFactory auditFactory;
        private List<AuditLog> auditList = new List<AuditLog>();
        private List<DbEntityEntry> objectList = new List<DbEntityEntry>();

        //public StdContext() : base("stdConnection")
        //{
        //    Database.SetInitializer<StdContext>(new CreateDatabaseIfNotExists<StdContext>());
        //}

        //public DbSet<Student> Student { get; set; }
        // public DbSet<Audit> Audit { get; set; }
        public DbSet<AuditLog> Audit { get; set; }

        public override int SaveChanges()
        {
            try
            {
                var appRead = new AppSettingsReader();
                var AuditLogRequired = appRead.GetValue("AuditLogRequired", typeof(string));

                if (Convert.ToString("1") != AuditLogRequired.ToString())
                {
                    var retVal2 = base.SaveChanges();
                    return retVal2;
                }

                auditList.Clear();
                objectList.Clear();
                auditFactory = new AuditTrailFactory(this);

                var entityList = ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified);
                foreach (var entity in entityList)
                {
                    if (entity.Entity.GetType().Name.ToLower() != "tokenmaster")
                    {
                        AuditLog audit = auditFactory.GetAudit(entity);
                        bool isValid = true;
                        if (entity.State == EntityState.Modified && string.IsNullOrWhiteSpace(audit.NewValue) && string.IsNullOrWhiteSpace(audit.OldValue))
                        {
                            isValid = false;
                        }
                        if (isValid)
                        {
                            auditList.Add(audit);
                            objectList.Add(entity);
                        }
                    }
                }

                var retVal = base.SaveChanges();
                if (auditList.Count > 0)
                {
                    int i = 0;
                    foreach (var audit in auditList)
                    {
                        if (audit.AuditStatus == AuditActions.I.ToString())
                            audit.PrimaryKey = auditFactory.GetKeyValue(objectList[i]);
                        this.Audit.Add(audit);

                        i++;
                    }

                    base.SaveChanges();
                }

                return retVal;
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

        public override Task<int> SaveChangesAsync()
        {
            var appRead = new AppSettingsReader();
            var AuditLogRequired = appRead.GetValue("AuditLogRequired", typeof(string));

            if (Convert.ToString("1") != AuditLogRequired.ToString())
            {
                var retVal2 = base.SaveChangesAsync();
                return retVal2;
            }

            auditList.Clear();
            objectList.Clear();
            auditFactory = new AuditTrailFactory(this);

            var entityList = ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified);
            foreach (var entity in entityList)
            {
                if (entity.Entity.GetType().Name.ToLower() != "tokenmaster")
                {
                    AuditLog audit = auditFactory.GetAudit(entity);
                    bool isValid = true;
                    if (entity.State == EntityState.Modified && string.IsNullOrWhiteSpace(audit.NewValue) && string.IsNullOrWhiteSpace(audit.OldValue))
                    {
                        isValid = false;
                    }
                    if (isValid)
                    {
                        auditList.Add(audit);
                        objectList.Add(entity);
                    }
                }
            }

            var retVal = base.SaveChangesAsync();
            if (auditList.Count > 0)
            {
                int i = 0;
                foreach (var audit in auditList)
                {
                    if (audit.AuditStatus == AuditActions.I.ToString())
                        audit.PrimaryKey = auditFactory.GetKeyValue(objectList[i]);
                    this.Audit.Add(audit);

                    i++;
                }

                base.SaveChangesAsync();
            }

            return retVal;

        }
    }
}