using EMPEntityFramework.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Core.Utilities
{
    public static class AuditLogger
    {
        /// <summary>
        /// This method is used to Save the any changes while handling exceptions
        /// Audit Log daase le wise 
        /// </summary>
        /// <param name="newlyAddedEntities"></param>
        /// <param name="oldAddedEntities"></param>
        /// <param name="Action"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static string AudittrailLog(object newlyAddedEntities, object oldAddedEntities, string Action, Guid UserID)
        {
            using (var DBEntity = new DatabaseEntities())
            {
                DatabaseEntities oldDBEntity = new DatabaseEntities();
                try
                {
                    Type objectType = newlyAddedEntities.GetType();
                    PropertyInfo[] objectPropertyList = objectType.GetProperties();
                    int i = 0; long PrimaryKey = 0;
                    StringBuilder newValues = new StringBuilder();
                    StringBuilder oldValues = new StringBuilder();
                    if (Action == "Add")
                    {
                        foreach (PropertyInfo property in objectPropertyList)
                        {
                            i++;
                            object sval = property.GetValue(newlyAddedEntities, null).ToString();
                            AuditLog audModel = new AuditLog();
                            audModel.AuditLogID = 0;
                            audModel.UserId = UserID;
                            audModel.TableName = objectType.Name;
                            if (i == 1)
                                PrimaryKey = Convert.ToInt64(sval);
                            audModel.PrimaryKey = "";// PrimaryKey;
                            audModel.AuditStatus = Action;
                            audModel.DateStamp = System.DateTime.Now;
                            audModel.FieldName = property.Name;
                            audModel.NewValue = sval.ToString();
                            audModel.OldValue = string.Empty;
                            oldDBEntity.AuditLogs.Add(audModel);
                            oldDBEntity.SaveChanges();
                        }
                    }
                    else
                    {
                        string sFPName = objectPropertyList[0].Name;
                        long lPrimaryKey = Convert.ToInt64(objectPropertyList[0].GetValue(newlyAddedEntities));
                        foreach (PropertyInfo property in objectPropertyList)
                        {
                            PropertyInfo property1 = oldAddedEntities.GetType().GetProperty(property.Name);
                            object originalValue = property1.GetValue(oldAddedEntities, null);
                            object newValue = property1.GetValue(newlyAddedEntities, null);
                            if (originalValue == (object)DateTime.MinValue)
                            {
                                originalValue = string.Empty;
                            }
                            else if (originalValue == (object)0)
                            {
                                originalValue = string.Empty;
                            }
                            if (newValue == (object)DateTime.MinValue)
                            {
                                newValue = string.Empty;
                            }
                            else if (newValue == (object)0)
                            {
                                newValue = string.Empty;
                            }
                            if (!object.Equals(originalValue, newValue))
                            {
                                AuditLog audModel = new AuditLog();
                                audModel.AuditLogID = 0;
                                audModel.UserId = UserID;
                                audModel.TableName = objectType.Name;
                                audModel.PrimaryKey = "";//lPrimaryKey.;
                                audModel.AuditStatus = Action;
                                audModel.DateStamp = System.DateTime.Now;
                                audModel.FieldName = property.Name;
                                audModel.NewValue = newValue.ToString();
                                audModel.OldValue = originalValue == null ? "" : originalValue.ToString();
                                oldDBEntity.AuditLogs.Add(audModel);
                                oldDBEntity.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string Message = ex.Message;
                    return ex.Message;
                }
            }
            return "";
        }
    }
}
