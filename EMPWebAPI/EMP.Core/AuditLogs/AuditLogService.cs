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

using EMP.Core.AuditLogs.DTO;
using EMPEntityFramework.Edmx;
using System.Reflection;

namespace EMP.Core.AuditLogs
{
    public class AuditLogService : IAuditLogService, IDisposable
    {
        public DbContext context;
        public AuditLogDTO AuditLog = new AuditLogDTO();

        public AuditLogService(DatabaseEntities context)
        {
            this.context = context;
        }

        public AuditLogDTO GetAudit(DbEntityEntry entry)
        {
            AuditLogDTO audit = new AuditLogDTO();
            // var user = (User)HttpContext.Current.Session[":user"];
            audit.UserID = Guid.NewGuid();// "swapnil";// user.UserName;
            audit.TableName = GetTableName(entry);
            audit.DateStamp = DateTime.Now;
            audit.PrimaryKey = GetKeyValue(entry) ?? 0;

            //entry is Added 
            if (entry.State == EntityState.Added)
            {
                var newValues = new StringBuilder();
                SetAddedProperties(entry, newValues);
                audit.NewValue = newValues.ToString();
                audit.AuditStatus = AuditActions.I.ToString();
            }
            //entry in deleted
            else if (entry.State == EntityState.Deleted)
            {
                var oldValues = new StringBuilder();
                SetDeletedProperties(entry, oldValues);
                audit.OldValue = oldValues.ToString();
                audit.AuditStatus = AuditActions.D.ToString();
            }
            //entry is modified
            else if (entry.State == EntityState.Modified)
            {
                var oldValues = new StringBuilder();
                var newValues = new StringBuilder();
                SetModifiedProperties(entry, oldValues, newValues);
                audit.OldValue = oldValues.ToString();
                audit.NewValue = newValues.ToString();
                audit.AuditStatus = AuditActions.U.ToString();
            }

            return audit;
        }

        private void SetAddedProperties(DbEntityEntry entry, StringBuilder newData)
        {
            foreach (var propertyName in entry.CurrentValues.PropertyNames)
            {
                var newVal = entry.CurrentValues[propertyName];
                if (newVal != null)
                {
                    newData.AppendFormat("{0}={1} || ", propertyName, newVal);
                }
            }
            if (newData.Length > 0)
                newData = newData.Remove(newData.Length - 3, 3);
        }

        private void SetDeletedProperties(DbEntityEntry entry, StringBuilder oldData)
        {
            DbPropertyValues dbValues = entry.GetDatabaseValues();
            foreach (var propertyName in dbValues.PropertyNames)
            {
                var oldVal = dbValues[propertyName];
                if (oldVal != null)
                {
                    oldData.AppendFormat("{0}={1} || ", propertyName, oldVal);
                }
            }
            if (oldData.Length > 0)
                oldData = oldData.Remove(oldData.Length - 3, 3);
        }

        private void SetModifiedProperties(DbEntityEntry entry, StringBuilder oldData, StringBuilder newData)
        {
            DbPropertyValues dbValues = entry.GetDatabaseValues();
            foreach (var propertyName in entry.OriginalValues.PropertyNames)
            {
                var oldVal = dbValues[propertyName];
                var newVal = entry.CurrentValues[propertyName];
                if (oldVal != null && newVal != null && !Equals(oldVal, newVal))
                {
                    newData.AppendFormat("{0}={1} || ", propertyName, newVal);
                    oldData.AppendFormat("{0}={1} || ", propertyName, oldVal);
                }
            }
            if (oldData.Length > 0)
                oldData = oldData.Remove(oldData.Length - 3, 3);
            if (newData.Length > 0)
                newData = newData.Remove(newData.Length - 3, 3);
        }

        public long? GetKeyValue(DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            long id = 0;
            if (objectStateEntry.EntityKey.EntityKeyValues != null)
                id = Convert.ToInt64(objectStateEntry.EntityKey.EntityKeyValues[0].Value);

            return id;
        }

        private string GetTableName(DbEntityEntry dbEntry)
        {
            Type objectType = dbEntry.GetType();
           // PropertyInfo[] objectPropertyList = objectType.GetProperties();

            //TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
            // string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;
            string tableName = objectType.Name;
            return tableName;
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }

    public enum AuditActions
    {
        I,
        U,
        D
    }
}
