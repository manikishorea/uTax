using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.Utilities;
using EMPEntityFramework.Edmx;
using EMPPortal.Transactions.DropDowns;

namespace EMPPortal.Transactions.CustomerLoginInformation
{
    public class CustomerLoginInformationService
    {
        private DatabaseEntities db = new DatabaseEntities();

        public List<CustomerLoginInformationModel> GetAllCustomerLoginInformation()
        {
            try
            {
                List<CustomerLoginInformationModel> CustomerLoginInfoList = new List<CustomerLoginInformationModel>();
                var emp = (from cl in db.emp_CustomerLoginInformation
                           join ci in db.emp_CustomerInformation on cl.CustomerOfficeId equals ci.Id
                           select new { cl, ci.CompanyName, ci.EFINStatus, ci.EFIN }).Distinct().ToList();


                //var emp1 = await db.emp_CustomerLoginInformation.Select(e => new CustomerLoginInformationModel
                //{
                //    Id = e.Id.ToString(),
                //    EFIN = e.EFIN,
                //    MasterIdentifier = e.MasterIdentifier.ToString(),
                //    CrossLinkUserId = e.CrossLinkUserId.ToString(),
                //    CrossLinkPassword = e.CrossLinkPassword.ToString(),
                //    OfficePortalUrl = e.OfficePortalUrl.ToString(),
                //    TaxOfficeUsername = e.TaxOfficeUsername.ToString(),
                //    TaxOfficePassword = e.TaxOfficePassword.ToString(),
                //    CustomerOfficeId = e.CustomerOfficeId,
                //    CompanyName = db.emp_CustomerInformation.Where(a => a.Id == e.CustomerOfficeId).Select(a => a.CompanyName).FirstOrDefault()
                //}).Distinct().ToListAsync();

                foreach (var item in emp)
                {
                    CustomerLoginInformationModel CustomerLoginInfo = new CustomerLoginInformationModel();
                    CustomerLoginInfo.Id = item.cl.Id.ToString();

                    CustomerLoginInfo.MasterIdentifier = item.cl.MasterIdentifier.ToString();
                    CustomerLoginInfo.CrossLinkUserId = item.cl.CrossLinkUserId.ToString();
                    CustomerLoginInfo.CrossLinkPassword = item.cl.CrossLinkPassword.ToString();
                    CustomerLoginInfo.OfficePortalUrl = item.cl.OfficePortalUrl.ToString();
                    CustomerLoginInfo.TaxOfficeUsername = item.cl.TaxOfficeUsername.ToString();
                    CustomerLoginInfo.TaxOfficePassword = item.cl.TaxOfficePassword.ToString();
                    CustomerLoginInfo.CustomerOfficeId = item.cl.CustomerOfficeId;

                    CustomerLoginInfo.CompanyName = item.CompanyName;
                    CustomerLoginInfo.EFIN = item.EFIN;
                    CustomerLoginInfo.EFINStatus = item.EFINStatus;

                    CustomerLoginInfo.CLAccountId = item.cl.CLAccountId;
                    CustomerLoginInfo.CLLogin = item.cl.CLLogin;
                    CustomerLoginInfo.CLAccountPassword = item.cl.CLAccountPassword;
                }
                return CustomerLoginInfoList;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerLoginInformationService/GetAllCustomerLoginInformation", Guid.Empty);
                throw;
            }
        }

        public CustomerLoginInformationModel GetCustomerLoginInformation(Guid id)
        {
            try
            {
                CustomerLoginInformationModel model = new CustomerLoginInformationModel();
                var itm = (from e in db.emp_CustomerLoginInformation
                           join c in db.emp_CustomerInformation on e.CustomerOfficeId equals c.Id
                           where c.Id == id
                           select new { e, c }).FirstOrDefault();
                if (itm != null)
                {

                    model.CLAccountId = itm.e.CLAccountId;
                    model.CLLogin = itm.e.CLLogin;
                    model.CLAccountPassword = itm.e.CLAccountPassword;

                    model.SalesforceParentID = itm.c.SalesforceParentID != null ? itm.c.SalesforceParentID.ToString() : "";
                    model.Id = itm.e.Id.ToString();
                    model.EFIN = itm.c.EFIN;
                    model.EntityId = itm.c.EntityId;
                    model.EFINStatus = itm.c.EFINStatus;

                    model.MasterIdentifier = !string.IsNullOrEmpty(itm.e.MasterIdentifier) ? itm.e.MasterIdentifier.ToString() : "";
                    model.CrossLinkUserId = !string.IsNullOrEmpty(itm.e.CrossLinkUserId) ? itm.e.CrossLinkUserId.ToString() : "";
                    model.CrossLinkPassword = !string.IsNullOrEmpty(itm.e.CrossLinkPassword) ? PasswordManager.DecryptText(itm.e.CrossLinkPassword.ToString()) : "";
                    model.OfficePortalUrl = !string.IsNullOrEmpty(itm.e.OfficePortalUrl) ? itm.e.OfficePortalUrl.ToString() : "";
                    model.TaxOfficeUsername = !string.IsNullOrEmpty(itm.e.TaxOfficeUsername) ? itm.e.TaxOfficeUsername.ToString() : "";
                    model.TaxOfficePassword = !string.IsNullOrEmpty(itm.e.TaxOfficePassword) ? PasswordManager.DecryptText(itm.e.TaxOfficePassword.ToString()) : "";
                    model.CustomerOfficeId = itm.e.CustomerOfficeId;
                    model.EMPPassword = !string.IsNullOrEmpty(itm.e.EMPPassword) ? PasswordManager.DecryptText(itm.e.EMPPassword.ToString()) : "";
                    model.EMPUserId = !string.IsNullOrEmpty(itm.e.EMPUserId) ? itm.e.EMPUserId.ToString() : "";
                    model.CompanyName = !string.IsNullOrEmpty(itm.c.CompanyName) ? itm.c.CompanyName.ToString() : "";
                    model.BusinessOwnerFirstName = !string.IsNullOrEmpty(itm.c.BusinessOwnerFirstName) ? itm.c.BusinessOwnerFirstName.ToString() : "";
                    model.BusinessOwnerLastName = !string.IsNullOrEmpty(itm.c.BusinesOwnerLastName) ? itm.c.BusinesOwnerLastName.ToString() : "";
                    model.PhysicalAddress1 = !string.IsNullOrEmpty(itm.c.PhysicalAddress1) ? itm.c.PhysicalAddress1.ToString() : "";
                    model.IsMSOUser = itm.c.IsMSOUser ?? false;
                    model.CityStateZip = ((itm.c.PhysicalCity != null) ? itm.c.PhysicalCity.ToString() : "") + ' ' + ((itm.c.PhysicalState != null) ? itm.c.PhysicalState.ToString() : "") + ", " + ((itm.c.PhysicalZipCode != null) ? itm.c.PhysicalZipCode.ToString() : "");
                    model.TransmitType = "";
                    model.IsMSOUser = itm.c.IsMSOUser ?? false;
                    var dbssb = db.SubSiteConfigurations.Where(a => a.emp_CustomerInformation_ID == id).FirstOrDefault();
                    if (dbssb != null)
                    {
                        if (dbssb.SubSiteTaxReturn == 1)
                            model.TransmitType = "All Sub-sites will transmit to the IRS (Transmitter) ";
                        else if (dbssb.SubSiteTaxReturn == 2)
                            model.TransmitType = "All Sub-sites will transmit to the Main Office (Feeder) ";
                        else
                            model.TransmitType = "Mixed – Some Sub-sites will transmit to the IRS and some will transmit to the Main Office ";
                    }

                    model.MSO = "";
                    string strBank = "";

                    var dbBank = (from ssb in db.SubSiteBankConfigs
                                  join bm in db.BankMasters on ssb.BankMaster_ID equals bm.Id
                                  where ssb.emp_CustomerInformation_ID == id
                                  select new { bm, ssb });


                    foreach (var bn in dbBank)
                    {
                        if (bn.bm.BankName == "TPG")
                        {
                            strBank += bn.bm.BankName + ": ";
                            var bankqu = db.BankSubQuestions.Where(a => a.Id == bn.ssb.SubQuestion_ID).Select(a => a.Questions).FirstOrDefault() + "<br/>";
                            strBank += bankqu;
                        }
                        else
                        {
                            strBank += bn.bm.BankName + "<br/>";
                        }
                    }


                    if (!string.IsNullOrEmpty(model.MasterIdentifier))
                    {
                        model.MasterIdentifierPassword = "Not Available";
                        var UtaxCrosslinkDetail = db.UtaxCrosslinkDetails.Where(o => o.Username == model.MasterIdentifier).FirstOrDefault();
                        if (UtaxCrosslinkDetail != null)
                        {
                            model.MasterIdentifierPassword = !string.IsNullOrEmpty(UtaxCrosslinkDetail.Password) ? PasswordManager.DecryptText(UtaxCrosslinkDetail.Password) : "";
                        }
                    }

                    model.Bank = strBank;
                    int EFIN = model.EFIN ?? 0;
                    string EFINText = EFIN.ToString().PadLeft(6, '0');
                    if (model.EFINStatus == 16 || model.EFINStatus == 19)
                    {
                        model.EFINStatusText = EFINText;
                    }
                    else if (model.EFINStatus == 21)
                    {

                        model.EFINStatusText = (EFIN > 0) ? EFINText + "<u><b>S</b></u>".ToString() : "Sharing";
                    }
                    else if (model.EFINStatus == 17 || model.EFINStatus == 20)
                    {
                        model.EFINStatusText = "Applied";
                    }
                    else if (model.EFINStatus == 18)
                    {
                        model.EFINStatusText = "Not Required";
                    }
                    else
                    {
                        model.EFINStatusText = EFINText;
                    }

                }
                return model;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerLoginInformationService/GetCustomerLoginInformation", id);
                throw;
            }
        }

        public int Save(CustomerLoginInformationModel model)
        {
            int entityState = 0;
            emp_CustomerLoginInformation customerLoginInformation = new emp_CustomerLoginInformation();

            if (model != null)
            {
                Guid newguid1;
                Guid CustomerOfficeId = model.CustomerOfficeId ?? Guid.Empty;
                if (Guid.TryParse(model.Id, out newguid1))
                {
                    //var ExistCust = db.emp_CustomerLoginInformation.Any(a => a.EFIN == model.EFIN && a.Id != newguid1);
                    //if (ExistCust)
                    //    return -1;

                    //11212016 - If

                    int EFIN = model.EFIN ?? 0;
                    if (EFIN > 0 && (model.EFINStatus == 16 || model.EFINStatus == 19))
                    {
                        //if (CustomerOfficeId != Guid.Empty)
                        //{
                        var ExistCust = db.emp_CustomerInformation.Any(a => a.EFIN == model.EFIN && a.Id != CustomerOfficeId);
                        if (ExistCust)
                            return -1;
                        //}
                    }

                    var ExistCrossCust = db.emp_CustomerLoginInformation.Any(a => a.CrossLinkUserId == model.CrossLinkUserId && a.Id != newguid1);
                    if (ExistCrossCust)
                        return -2;
                }

                if (string.IsNullOrEmpty(model.Id))
                {
                    customerLoginInformation.Id = Guid.NewGuid();
                }
                else
                {
                    Guid newguid;
                    if (Guid.TryParse(model.Id, out newguid))
                    {
                        customerLoginInformation.Id = newguid;
                        entityState = (int)System.Data.Entity.EntityState.Modified;
                    }
                }

                // customerLoginInformation.EFIN = model.EFIN;
                customerLoginInformation.MasterIdentifier = model.MasterIdentifier;
                customerLoginInformation.CrossLinkUserId = model.CrossLinkUserId;
                customerLoginInformation.CrossLinkPassword = PasswordManager.CryptText(model.CrossLinkPassword);// model.CrossLinkPassword;
                customerLoginInformation.OfficePortalUrl = model.OfficePortalUrl;
                customerLoginInformation.TaxOfficeUsername = model.TaxOfficeUsername;
                customerLoginInformation.TaxOfficePassword = PasswordManager.CryptText(model.TaxOfficePassword);// model.TaxOfficePassword;
                customerLoginInformation.CustomerOfficeId = model.CustomerOfficeId;
                customerLoginInformation.EMPPassword = PasswordManager.CryptText(model.EMPPassword); //model.EMPPassword;
                customerLoginInformation.EMPUserId = model.EMPUserId;
                customerLoginInformation.StatusCode = EMPConstants.Active;

                customerLoginInformation.CLAccountId = model.CLAccountId;
                customerLoginInformation.CLLogin = model.CLLogin;
                customerLoginInformation.CLAccountPassword = string.IsNullOrEmpty(model.CLAccountPassword) ? "" : PasswordManager.CryptText(model.CLAccountPassword);

                if (entityState == (int)System.Data.Entity.EntityState.Modified)
                {
                    customerLoginInformation.CreatedDate = DateTime.Now;
                    customerLoginInformation.LastUpdatedDate = DateTime.Now;
                    customerLoginInformation.LastUpdatedBy = model.UserId;
                    customerLoginInformation.CreatedBy = model.UserId;
                    db.Entry(customerLoginInformation).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    db.Entry(customerLoginInformation).State = System.Data.Entity.EntityState.Added;
                    customerLoginInformation.LastUpdatedBy = model.UserId;
                    customerLoginInformation.LastUpdatedDate = DateTime.Now;
                    db.emp_CustomerLoginInformation.Add(customerLoginInformation);
                }

                emp_CustomerInformation empCustInfo = new emp_CustomerInformation();
                empCustInfo = db.emp_CustomerInformation.Where(o => o.Id == model.CustomerOfficeId).FirstOrDefault();
                if (empCustInfo != null)
                {
                    //11212016 - 2
                    empCustInfo.EFIN = model.EFIN;
                    empCustInfo.EFINStatus = model.EFINStatus;
                    empCustInfo.StatusCode = empCustInfo.IsActivationCompleted == 1 ? EMPConstants.Active : EMPConstants.Created;
                    empCustInfo.CreatedBy = model.UserId;
                    empCustInfo.LastUpdatedBy = model.UserId;

                    if (empCustInfo.EntityId == (int)EMPConstants.Entity.SO || empCustInfo.EntityId == (int)EMPConstants.Entity.SOME)
                    {
                        if (empCustInfo.StatusCode == EMPConstants.Active || empCustInfo.IsActivationCompleted == 1)
                        {
                            empCustInfo.StatusCode = EMPConstants.Active;
                            empCustInfo.IsActivationCompleted = 1;
                            empCustInfo.AccountStatus = "Active";
                            if (empCustInfo.EntityId == (int)EMPConstants.Entity.SO)
                            {
                                empCustInfo.EROType = "Single Office";
                            }
                            else if (empCustInfo.EntityId == (int)EMPConstants.Entity.SOME)
                            {
                                empCustInfo.EROType = "SOME";
                            }
                        }
                    }

                    empCustInfo.CreatedDate = System.DateTime.Now;
                    empCustInfo.LastUpdatedDate = System.DateTime.Now;

                    db.Entry(empCustInfo).State = System.Data.Entity.EntityState.Modified;


                }
            }
            try
            {
                db.SaveChanges();
                db.Dispose();

                if (model != null)
                {
                    if (model.CustomerOfficeId != Guid.Empty)
                    {
                        DropDownService ddService = new DropDownService();
                        var items = ddService.GetBottomToTopHierarchy(model.CustomerOfficeId ?? Guid.Empty);
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerLoginInformationService/Save", Guid.Empty);
                return 0;
                throw;
            }
        }

        public bool CustomerLoginInformationExists(Guid id)
        {
            return db.emp_CustomerLoginInformation.Count(e => e.Id == id) > 0;
        }
    }
}
