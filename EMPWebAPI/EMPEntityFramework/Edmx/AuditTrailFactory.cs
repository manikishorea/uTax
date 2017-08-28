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
using EMPEntityFramework.Edmx;
using System.Reflection;

namespace EMPEntityFramework.Edmx
{
    public class AuditTrailFactory : IDisposable
    {
        public DatabaseEntities context;

        // public DatabaseEntities db=new DatabaseEntities();

        public AuditLog AuditLog = new AuditLog();

        public AuditTrailFactory(DatabaseEntities context)
        {
            this.context = context;
        }

        public AuditLog GetAudit(DbEntityEntry entry)
        {
            AuditLog audit = new AuditLog();
            // var user = (User)HttpContext.Current.Session[":user"];
            audit.TableName = GetTableName(entry);
            audit.DateStamp = DateTime.Now;
            audit.PrimaryKey = GetKeyValue(entry);
            // HttpActionContext filterContext
            //entry is Added 
            if (entry.State == EntityState.Added)
            {
                var newValues = new StringBuilder();
                var TokenId = new StringBuilder();
                var UserId = new StringBuilder();

                SetAddedProperties(entry, newValues, TokenId, UserId);
                audit.NewValue = newValues.ToString();
                audit.AuditStatus = AuditActions.I.ToString();
                if (TokenId.Length > 0)
                    audit.TokenId = new Guid(TokenId.ToString());
                if (UserId.Length > 0)
                    audit.UserId = new Guid(UserId.ToString());
            }
            //entry in deleted
            else if (entry.State == EntityState.Deleted)
            {
                var TokenId = new StringBuilder();
                var UserId = new StringBuilder();
                var oldValues = new StringBuilder();
                SetDeletedProperties(entry, oldValues, TokenId, UserId);
                audit.OldValue = oldValues.ToString();
                audit.AuditStatus = AuditActions.D.ToString();
                if (TokenId.Length > 0)
                    audit.TokenId = new Guid(TokenId.ToString());
                if (UserId.Length > 0)
                    audit.UserId = new Guid(UserId.ToString());
            }
            //entry is modified
            else if (entry.State == EntityState.Modified)
            {
                var TokenId = new StringBuilder();
                var UserId = new StringBuilder();
                var oldValues = new StringBuilder();
                var newValues = new StringBuilder();
                SetModifiedProperties(entry, oldValues, newValues, TokenId, UserId);
                audit.OldValue = oldValues.ToString();
                audit.NewValue = newValues.ToString();
                audit.AuditStatus = AuditActions.U.ToString();
                if (TokenId.Length > 0)
                    audit.TokenId = new Guid(TokenId.ToString());
                if (UserId.Length > 0)
                    audit.UserId = new Guid(UserId.ToString());
            }

            return audit;
        }

        private void SetAddedProperties(DbEntityEntry entry, StringBuilder newData, StringBuilder TokenId, StringBuilder UserId)
        {
            foreach (var propertyName in entry.CurrentValues.PropertyNames)
            {
                var newVal = entry.CurrentValues[propertyName];

                newData.AppendFormat("{0}={1} || ", propertyName, newVal ?? "");

                if (newVal != null)
                {
                    if (propertyName == "LastUpdatedBy" || propertyName == "UpdatedBy")
                    {
                        Guid gUserId = Guid.Empty;
                        if (Guid.TryParse(newVal.ToString(), out gUserId))
                        {
                            //var Token = context.TokenMasters.Where(o => o.UserId == gUserId).FirstOrDefault();
                            //if (Token != null)
                            //{
                            //    TokenId.Append(Token.AuthToken);
                            //}
                            UserId.Append(gUserId.ToString());
                        }

                    }
                }

            }
            if (newData.Length > 0)
                newData = newData.Remove(newData.Length - 3, 3);
        }

        private void SetDeletedProperties(DbEntityEntry entry, StringBuilder oldData, StringBuilder TokenId, StringBuilder UserId)
        {
            DbPropertyValues dbValues = entry.GetDatabaseValues();
            foreach (var propertyName in dbValues.PropertyNames)
            {
                var oldVal = dbValues[propertyName];

                oldData.AppendFormat("{0}={1} || ", propertyName, oldVal ?? "");

                if (oldVal != null)
                {
                    if (propertyName == "LastUpdatedBy" || propertyName == "UpdatedBy")
                    {
                        Guid gUserId = Guid.Empty;
                        if (Guid.TryParse(oldVal.ToString(), out gUserId))
                        {
                            //var Token = context.TokenMasters.Where(o => o.UserId == gUserId).FirstOrDefault();
                            //if (Token != null)
                            //{
                            //    TokenId.Append(Token.AuthToken);
                            //}
                            UserId.Append(gUserId.ToString());
                        }

                    }
                }
            }
            if (oldData.Length > 0)
                oldData = oldData.Remove(oldData.Length - 3, 3);
        }

        private void SetModifiedProperties(DbEntityEntry entry, StringBuilder oldData, StringBuilder newData, StringBuilder TokenId, StringBuilder UserId)
        {
            DbPropertyValues dbValues = entry.GetDatabaseValues();
            foreach (var propertyName in entry.OriginalValues.PropertyNames)
            {
                var oldVal = dbValues[propertyName];
                var newVal = entry.CurrentValues[propertyName];
                if (!Equals(oldVal, newVal))
                {
                    newData.AppendFormat("{0}={1} || ", propertyName, newVal);
                    oldData.AppendFormat("{0}={1} || ", propertyName, oldVal);
                }

                if (newVal != null)
                {
                    if (propertyName == "LastUpdatedBy" || propertyName == "UpdatedBy")
                    {
                        Guid gUserId = Guid.Empty;
                        if (Guid.TryParse(newVal.ToString(), out gUserId))
                        {
                            //var Token = context.TokenMasters.Where(o => o.UserId == gUserId).FirstOrDefault();
                            //if (Token != null)
                            //{
                            //    TokenId.Append(Token.AuthToken);
                            //}

                            UserId.Append(gUserId.ToString());
                        }

                    }
                }
            }

            if (oldData.Length > 0)
                oldData = oldData.Remove(oldData.Length - 3, 3);
            if (newData.Length > 0)
                newData = newData.Remove(newData.Length - 3, 3);
        }

        public string GetKeyValue(DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            string id = "";
            if (objectStateEntry.EntityKey.EntityKeyValues != null)
                id = objectStateEntry.EntityKey.EntityKeyValues[0].Value.ToString();

            return id;
        }

        private string GetTableName(DbEntityEntry dbEntry)
        {
            Type objectType = dbEntry.GetType();
            // PropertyInfo[] objectPropertyList = objectType.GetProperties();

            //TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
            // string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;
            string tableName = dbEntry.Entity.GetType().Name;// objectType.Name;
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

    public class AuditLogDTO
    {
        public long AuditLogID { get; set; }
        public System.Guid UserID { get; set; }
        public string TableName { get; set; }
        public long PrimaryKey { get; set; }
        public string AuditStatus { get; set; }
        public System.DateTime DateStamp { get; set; }
        public string FieldName { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
    }
}
