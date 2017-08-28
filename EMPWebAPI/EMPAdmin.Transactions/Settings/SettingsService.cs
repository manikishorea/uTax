using EMP.Core.Utilities;
using EMPAdmin.Transactions.Settings.DTO;
using EMPEntityFramework.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPAdmin.Transactions.Settings
{
    public class SettingsService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public SettingsDTO GetSettings()
        {
            SettingsDTO res = new SettingsDTO();

            try
            {
                var data = db.uTaxSettings.Where(x => x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (data != null)
                {
                    res.IsAccountCreation = data.AccountCreation ?? false;
                }
            }
            catch (Exception ex)
            {
            }
            return res;
        }

        public int Save(SettingsDTO req)
        {
            try
            {
                var data = db.uTaxSettings.Where(x => x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (data != null)
                {
                    data.AccountCreation = req.IsAccountCreation;
                    data.LastUpdatedBy = req.UserId;
                    data.LastUpdatedDate = DateTime.Now;
                }
                else
                {
                    uTaxSetting _setg = new uTaxSetting();
                    _setg.AccountCreation = req.IsAccountCreation;
                    _setg.CreatedBy = req.UserId;
                    _setg.CreatedDate = DateTime.Now;
                    _setg.StatusCode = EMPConstants.Active;
                    db.uTaxSettings.Add(_setg);
                }
                db.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }            
        }
    }
}
