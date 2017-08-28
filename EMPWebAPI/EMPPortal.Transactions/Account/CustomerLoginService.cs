using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMP.Core.Utilities;

using EMPPortal.Transactions.Account.Model;
using EMP.Core.Token;
using EMPPortal.Transactions.CustomerLoginInformation;
using EMPPortal.Core.Utilities;
using EMPPortal.Transactions.DropDowns;
using EMPPortal.Transactions.DropDowns.DTO;
using System.Collections.Generic;
using EMPPortal.Transactions.SubSiteFees;
using EMPPortal.Transactions.Sitemap;

namespace EMPPortal.Transactions.Account
{
    public class CustomerLoginService : ICustomerLoginService
    {
        DatabaseEntities db = new DatabaseEntities();
        TokenService _TokenService = new TokenService();
        // public DatabaseEntities db = new DatabaseEntities();

        //public IQueryable<ChangePasswordModel> GetAll()
        //{
        //    db = new DatabaseEntities();
        //    var data = db.emp_CustomerLoginInformation.Select(o => new ChangePasswordModel
        //    {
        //        Id = o.Id,
        //        EMPPassword = o.EMPPassword,
        //        CustomerOfficeId = o.CustomerOfficeId,
        //        StatusCode = o.StatusCode
        //    }).DefaultIfEmpty();
        //    return data;
        //}

        public CustomerLoginModel Get(CustomerLoginModel _Dto, string userip)
        {
            try
            {
                DropDownService ddService = new DropDownService();
                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();

                db = new DatabaseEntities();
                //var data2 = db.emp_CustomerLoginInformation.ToList();
                string EMPPassword = PasswordManager.CryptText(_Dto.EMPPassword); // _Dto.EMPPassword
                var data = (from ci in db.emp_CustomerInformation
                            join cli in db.emp_CustomerLoginInformation on ci.Id equals cli.CustomerOfficeId
                            join entMas in db.EntityMasters on ci.EntityId equals entMas.Id
                            where cli.EMPUserId == _Dto.EMPUserId && cli.EMPPassword == EMPPassword && (ci.StatusCode == EMPConstants.Active || ci.StatusCode == EMPConstants.Pending)
                            //&& ci.IsHold != true//ci.StatusCode == EMPConstants.Created ||
                            select new { ci, cli, EntityId = entMas.Id, entMas.BaseEntityId }).FirstOrDefault();

                if (data != null)
                {
                    if ((data.ci.IsHold ?? false) && data.ci.EntityId != (int)EMPConstants.Entity.SO && data.ci.EntityId != (int)EMPConstants.Entity.MO && data.ci.EntityId != (int)EMPConstants.Entity.SVB)
                    {
                        _Dto.Id = Guid.Empty;
                        _Dto.Message = "Your site has been placed on hold, please contact your Software Provider";
                        return _Dto;
                    }
                    _Dto.Id = data.cli.Id;
                    _Dto.CustomerOfficeId = data.cli.CustomerOfficeId;

                    //11212016
                    _Dto.EFIN = data.ci.EFIN;
                    _Dto.EFINStatus = data.ci.EFINStatus;

                    _Dto.TaxOfficeUsername = data.cli.TaxOfficeUsername;
                    _Dto.CrossLinkUserId = data.cli.CrossLinkUserId;

                    _Dto.IsChangedPassword = (data.cli.CrossLinkPassword == data.cli.EMPPassword) ? true : false;
                    _Dto.IsSetSecurityQuestion = db.SecurityAnswerUserMaps.Where(o => o.UserId == data.cli.CustomerOfficeId).Any();
                    _Dto.Token = _TokenService.GenerateToken(data.ci.Id, userip);
                    _Dto.ParentID = data.ci.ParentId.ToString();
                    _Dto.SalesYearID = data.ci.SalesYearID.ToString();
                    //_Dto.EntityDisplayID = data.DisplayId;
                    _Dto.BaseEntityId = data.BaseEntityId;
                    // _Dto.EntityID = data.EntityId;
                    _Dto.EntityID = data.ci.EntityId;//.ToString();

                    _Dto.IsMSOUser = data.ci.IsMSOUser ?? false;
                    _Dto.IsActivationCompleted = data.ci.IsActivationCompleted ?? 0;
                    _Dto.IsEnrollmentSubmit = new SubSiteFeeService().IsEnrollmentSubmit(data.ci.Id);
                    _Dto.IsVerified = data.ci.IsVerified ?? false;

                    _Dto.uTaxNotCollectingSBFee = data.ci.uTaxNotCollectingSBFee ?? false;

                    _Dto.IsTaxReturn = true;
                    if (data.ci.ParentId == Guid.Empty || data.ci.ParentId == null)
                    {
                        var taxreturn = db.MainOfficeConfigurations.Where(x => x.emp_CustomerInformation_ID == data.ci.Id && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                        if (taxreturn != null)
                        {
                            _Dto.IsTaxReturn = taxreturn.IsSiteTransmitTaxReturns;
                        }
                    }
                    _Dto.IsHold = data.ci.IsHold ?? false;

                    Guid TopParentId = Guid.Empty;
                    EntityHierarchyDTOs = ddService.GetEntityHierarchies(data.ci.Id);
                    var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                    TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;

                    DefaultBankModel DefaultBank = new DefaultBankModel();

                    DefaultBank = (from enrollbank in db.EnrollmentBankSelections
                                   where enrollbank.CustomerId == data.cli.CustomerOfficeId && enrollbank.StatusCode == EMPConstants.Active && enrollbank.BankSubmissionStatus == 1
                                   orderby enrollbank.BankSubmissionStatus descending, enrollbank.LastUpdatedDate descending
                                   select new DefaultBankModel { BankId = enrollbank.BankId, BankSubmissionStatus = enrollbank.BankSubmissionStatus ?? 0, LastUpdatedDate = enrollbank.LastUpdatedDate }).FirstOrDefault();

                    if (DefaultBank != null)
                    {
                        _Dto.BankId = DefaultBank.BankId;
                    }
                    else
                    {
                        DefaultBankModel DefaultBank2 = (from enrollbank in db.EnrollmentBankSelections
                                                         where enrollbank.CustomerId == data.cli.CustomerOfficeId && enrollbank.StatusCode == EMPConstants.Active
                                                         orderby enrollbank.BankSubmissionStatus descending, enrollbank.LastUpdatedDate descending
                                                         select new DefaultBankModel { BankId = enrollbank.BankId, BankSubmissionStatus = enrollbank.BankSubmissionStatus ?? 0, LastUpdatedDate = enrollbank.LastUpdatedDate }).FirstOrDefault();

                        if (DefaultBank2 != null)
                        {
                            _Dto.BankId = DefaultBank2.BankId;
                        }
                    }


                    if (_Dto.EntityID != (int)EMPConstants.Entity.SO && _Dto.EntityID != (int)EMPConstants.Entity.SOME && _Dto.EntityID != (int)EMPConstants.Entity.SOME_SS && _Dto.BankId != Guid.Empty)
                    {
                        var SSBConfig = db.SubSiteBankConfigs.Where(o => o.emp_CustomerInformation_ID == TopParentId && o.BankMaster_ID == _Dto.BankId).ToList();
                        if (SSBConfig.Count() == 0)
                        {
                            _Dto.BankId = Guid.Empty;
                        }
                    }
                   
                    if (TopParentId != Guid.Empty)
                    {
                        if (TopParentId == data.ci.Id)
                            _Dto.CanSubSiteLoginToEmp = true;
                        else
                        {
                            if (_Dto.BaseEntityId == (int)EMPConstants.BaseEntities.AE_SS)
                            {
                                // _Dto.CanSubSiteLoginToEmp = db.SubSiteOfficeConfigs.Where(a => a.RefId.ToString() == _Dto.ParentID).Select(a => a.CanSubSiteLoginToEmp ?? false).FirstOrDefault();
                                //var SupParentData = db.emp_CustomerInformation.Where(o => o.Id == data.ci.ParentId).FirstOrDefault();
                                //if (SupParentData != null)
                                //{
                                _Dto.SupParentID = TopParentId.ToString();

                                var sscExist = (from ssc in db.SubSiteConfigurations where ssc.emp_CustomerInformation_ID == TopParentId select ssc).FirstOrDefault();
                                if (sscExist != null)
                                {
                                    if (sscExist.IsuTaxManageingEnrolling == true)
                                        _Dto.CanSubSiteLoginToEmp = true;
                                    else
                                    {
                                        if (sscExist.IsuTaxPortalEnrollment == true)
                                            _Dto.CanSubSiteLoginToEmp = true;
                                        else
                                            _Dto.CanSubSiteLoginToEmp = false;
                                    }
                                }



                                var subSiteOfficeCo = (from ssc in db.SubSiteOfficeConfigs where ssc.RefId == _Dto.CustomerOfficeId select ssc).FirstOrDefault();
                                if (subSiteOfficeCo != null)
                                {
                                    if (subSiteOfficeCo.EFINListedOtherOffice == false)
                                        _Dto.EFINOwnerUserId = false;
                                    else
                                    {
                                        if (subSiteOfficeCo.SiteOwnthisEFIN == false)
                                            _Dto.EFINOwnerUserId = true;
                                        else
                                            _Dto.EFINOwnerUserId = false;
                                    }
                                }
                                //}
                            }
                            else
                            {
                                // _Dto.CanSubSiteLoginToEmp = db.SubSiteConfigurations.Where(a => a.emp_CustomerInformation_ID.ToString() == _Dto.ParentID).Select(a => a.CanSubSiteLoginToEmp).FirstOrDefault();

                                var sscExist = (from ssc in db.SubSiteConfigurations where ssc.emp_CustomerInformation_ID == TopParentId select ssc).FirstOrDefault();
                                if (sscExist != null)
                                {
                                    if (sscExist.IsuTaxManageingEnrolling == true)
                                        _Dto.CanSubSiteLoginToEmp = true;
                                    else
                                    {
                                        if (sscExist.IsuTaxPortalEnrollment == true)
                                            _Dto.CanSubSiteLoginToEmp = true;
                                        else
                                            _Dto.CanSubSiteLoginToEmp = false;
                                    }
                                }


                                var subSiteOfficeCo = (from ssc in db.SubSiteOfficeConfigs where ssc.RefId == _Dto.CustomerOfficeId select ssc).FirstOrDefault();
                                if (subSiteOfficeCo != null)
                                {
                                    if (subSiteOfficeCo.EFINListedOtherOffice == false)
                                        _Dto.EFINOwnerUserId = false;
                                    else
                                    {
                                        if (subSiteOfficeCo.SiteOwnthisEFIN == false)
                                            _Dto.EFINOwnerUserId = true;
                                        else
                                            _Dto.EFINOwnerUserId = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        _Dto.CanSubSiteLoginToEmp = true;
                    }


                    if (_Dto.EntityID == (int)EMPConstants.Entity.SOME_SS)
                    {
                        _Dto.CanSubSiteLoginToEmp = true;
                    }

                    EntityHierarchyDTOs = ddService.GetEntityHierarchies(_Dto.Id);

                    Guid ParentId = Guid.Empty;
                    int Level = EntityHierarchyDTOs.Count;
                    if (EntityHierarchyDTOs.Count > 0)
                    {
                        var LevelOne = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                        if (LevelOne != null)
                        {
                            ParentId = LevelOne.CustomerId ?? Guid.Empty;
                        }

                        if (EntityHierarchyDTOs.Count > 1)
                        {
                            _Dto.IsMSOUser = db.emp_CustomerInformation.Where(o => o.Id == ParentId).Select(o => o.IsMSOUser).FirstOrDefault() ?? false;
                        }
                    }

                }
                else
                {
                    return null;
                }
                return _Dto;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerLogin/Get", Guid.Empty);
                return null;
            }
        }

        private bool IsExists(Guid id)
        {
            using (DatabaseEntities db = new DatabaseEntities())
            {
                return db.SecurityQuestionMasters.Count(e => e.Id == id) > 0;
            }
        }

        public bool IsEnrollmentSubmit(Guid Id)
        {
            try
            {
                var BankEnroll = db.BankEnrollments.Any(a => a.CustomerId == Id && a.IsActive == true);
                if (BankEnroll)
                {
                    return true;
                }
                else
                {
                    var custominfo = db.emp_CustomerInformation.Where(a => a.ParentId == Id);
                    if (custominfo != null)
                    {
                        foreach (var cu in custominfo)
                        {
                            var BankEnrolls = db.BankEnrollments.Any(a => a.CustomerId == cu.Id && a.IsActive == true);
                            if (BankEnrolls)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerLogin/IsEnrollmentSubmit", Id);
                return false;
            }
        }

        public CustomerLoginModel IsUserExist(CustomerLoginModel _Dto, string userip)
        {
            try
            {
                //using (DatabaseEntities DatabaseEntities = new DatabaseEntities())
                //{
                db = new DatabaseEntities();

                //string dd = PasswordManager.DecryptText("aWIbcZIuvxlFajh8TMADjrxSKlmndmUM");

                string EMPPassword = PasswordManager.CryptText(_Dto.EMPPassword);
                //string EMPPasswordTest = PasswordManager.DecryptText(_Dto.EMPPassword);

                var data = (from user in db.UserMasters
                            join entMas in db.EntityMasters on user.EntityId equals entMas.Id
                            where user.UserName == _Dto.EMPUserId && user.Password == EMPPassword // _Dto.EMPPassword
                            select new { user, entMas }).FirstOrDefault();

                if (data != null)
                {
                    _Dto.Id = data.user.Id;
                    _Dto.CustomerOfficeId = data.user.Id;
                    //_Dto.EFIN = data.Id;
                    _Dto.TaxOfficeUsername = data.user.UserName;
                    _Dto.CrossLinkUserId = data.user.UserName;
                    _Dto.EntityID = data.user.EntityId;//.ToString();
                    _Dto.IsChangedPassword = true;
                    _Dto.IsSetSecurityQuestion = true;
                    //_Dto.EntityDisplayID = data.entMas.Id;
                    _Dto.BaseEntityId = data.entMas.BaseEntityId;
                    _Dto.IsActivationCompleted = 1;
                    _Dto.IsVerified = true;
                    _Dto.uTaxNotCollectingSBFee = true;

                    var permissions = new Sitemap.SitemapService().GetUserRolePermissions(data.user.Id);
                    _Dto.IsnewCustomers = permissions.NewCustomer.ViewPermission;
                    _Dto.IsOfficeMgmt = permissions.OfficeManamgement.ViewPermission;
                    _Dto.FeeReport = permissions.ReportPermissions.FeeReport;
                    _Dto.NoBankApp = permissions.ReportPermissions.NoBankApp;
                    _Dto.Enrollstatus = permissions.ReportPermissions.Enrollstatus;
                    _Dto.LoginReport = permissions.ReportPermissions.LoginReport;

                }

                if (data != null)
                {
                    _Dto.Token = _TokenService.GenerateToken(data.user.Id, userip);
                }
                else
                {
                    _Dto = Get(_Dto, userip);
                }
                return _Dto;
                // }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerLogin/IsUserExist", Guid.Empty);
                return null;
            }
        }

        public CustomerLoginModel getCustomerInfoById(string Id)
        {
            CustomerLoginModel _Dto = new CustomerLoginModel();
            Guid UserId;
            if (!Guid.TryParse(Id, out UserId))
            {
                return null;
            }

            try
            {
                db = new DatabaseEntities();
                //var data2 = db.emp_CustomerLoginInformation.ToList();
                var data = (from ci in db.emp_CustomerInformation
                            join cli in db.emp_CustomerLoginInformation on ci.Id equals cli.CustomerOfficeId
                            join entMas in db.EntityMasters on ci.EntityId equals entMas.Id
                            where ci.Id == UserId && ci.StatusCode == EMPConstants.Created
                            select new { ci, cli, entMas.Id, entMas.BaseEntityId }).FirstOrDefault();

                if (data != null)
                {
                    _Dto.Id = data.cli.Id;
                    _Dto.CustomerOfficeId = data.cli.CustomerOfficeId;
                    //11212016
                    _Dto.EFIN = data.ci.EFIN;
                    _Dto.EFINStatus = data.ci.EFINStatus;

                    _Dto.TaxOfficeUsername = data.cli.TaxOfficeUsername;
                    _Dto.CrossLinkUserId = data.cli.CrossLinkUserId;
                    _Dto.EntityID = data.ci.EntityId;//.ToString();
                    _Dto.IsChangedPassword = (data.cli.CrossLinkPassword == data.cli.EMPPassword) ? true : false;
                    _Dto.IsSetSecurityQuestion = db.SecurityAnswerUserMaps.Where(o => o.UserId == data.cli.CustomerOfficeId).Any();
                    _Dto.Token = _TokenService.GenerateToken(data.cli.Id);
                    _Dto.ParentID = data.ci.ParentId.ToString();
                    _Dto.SalesYearID = data.ci.SalesYearID.ToString();
                    //_Dto.EntityDisplayID = data.DisplayId;
                    _Dto.BaseEntityId = data.BaseEntityId;
                    _Dto.IsMSOUser = data.ci.IsMSOUser ?? false;
                    _Dto.IsActivationCompleted = data.ci.IsActivationCompleted ?? 0;
                    _Dto.IsEnrollmentSubmit = new SubSiteFeeService().IsEnrollmentSubmit(data.ci.Id);


                    //if (!string.IsNullOrEmpty(_Dto.ParentID))
                    //{
                    //    if (_Dto.BaseEntityId == (int)EMPConstants.BaseEntities.SOME)
                    //    {
                    //        // _Dto.CanSubSiteLoginToEmp = db.SubSiteOfficeConfigs.Where(a => a.RefId.ToString() == _Dto.ParentID).Select(a => a.CanSubSiteLoginToEmp ?? false).FirstOrDefault();
                    //        var SupParentData = db.emp_CustomerInformation.Where(o => o.Id == data.ci.ParentId).FirstOrDefault();
                    //        if (SupParentData != null)
                    //        {
                    //            _Dto.SupParentID = SupParentData.ParentId.ToString();

                    //            var sscExist = (from ssc in db.SubSiteConfigurations where ssc.emp_CustomerInformation_ID.ToString() == _Dto.SupParentID select ssc).FirstOrDefault();
                    //            if (sscExist != null)
                    //            {
                    //                if (sscExist.IsuTaxManageingEnrolling == true)
                    //                    _Dto.CanSubSiteLoginToEmp = true;
                    //                else
                    //                {
                    //                    if (sscExist.IsuTaxPortalEnrollment == true)
                    //                        _Dto.CanSubSiteLoginToEmp = true;
                    //                    else
                    //                        _Dto.CanSubSiteLoginToEmp = false;
                    //                }
                    //            }
                    //        }


                    //    }
                    //    else
                    //    {
                    //        // _Dto.CanSubSiteLoginToEmp = db.SubSiteConfigurations.Where(a => a.emp_CustomerInformation_ID.ToString() == _Dto.ParentID).Select(a => a.CanSubSiteLoginToEmp).FirstOrDefault();

                    //        var sscExist = (from ssc in db.SubSiteConfigurations where ssc.emp_CustomerInformation_ID.ToString() == _Dto.ParentID select ssc).FirstOrDefault();
                    //        if (sscExist != null)
                    //        {
                    //            if (sscExist.IsuTaxManageingEnrolling == true)
                    //                _Dto.CanSubSiteLoginToEmp = true;
                    //            else
                    //            {
                    //                if (sscExist.IsuTaxPortalEnrollment == true)
                    //                    _Dto.CanSubSiteLoginToEmp = true;
                    //                else
                    //                    _Dto.CanSubSiteLoginToEmp = false;
                    //            }
                    //        }
                    //    }

                    //}
                    //else
                    //{
                    //    _Dto.CanSubSiteLoginToEmp = true;
                    //}

                }
                else
                {
                    return null;
                }

                return _Dto;


            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerLogin/GetCustomer", UserId);
                return null;
            }
        }

        public CustomerLoginInformationModel GetParentCustomerInformation(Guid id)
        {
            CustomerLoginInformationModel model = new CustomerLoginInformationModel();
            try
            {
                Guid ParentId = id;
                var Customer = db.emp_CustomerInformation.Where(a => a.Id == id).Select(a => new { Id = a.Id, ParentId = a.ParentId, EntityId = a.EntityId }).FirstOrDefault();


                if (Customer.EntityId == (int)EMPConstants.Entity.SOME)
                {
                    id = Customer.Id;
                }
                else if ((Customer.ParentId ?? Guid.Empty) != Guid.Empty)
                {
                    id = Customer.ParentId ?? Guid.Empty;
                    //ParentId = id_Customer.Id;
                }

                var itm = (from e in db.emp_CustomerLoginInformation
                           join c in db.emp_CustomerInformation on e.CustomerOfficeId equals c.Id
                           where c.Id == id
                           select new { e, c }).FirstOrDefault();

                if (itm != null)
                {
                    model.SalesforceParentID = itm.c.SalesforceParentID;
                    model.Id = itm.e.Id.ToString();

                    model.EFIN = itm.c.EFIN;
                    model.EFINStatus = itm.c.EFINStatus;

                    model.ParentId = (itm.c.ParentId ?? Guid.Empty).ToString();
                    model.MasterIdentifier = itm.e.MasterIdentifier;
                    model.CrossLinkUserId = itm.e.CrossLinkUserId;
                    model.CrossLinkPassword = itm.e.CrossLinkPassword;
                    model.OfficePortalUrl = itm.e.OfficePortalUrl;
                    model.TaxOfficeUsername = itm.e.TaxOfficeUsername;
                    model.TaxOfficePassword = itm.e.TaxOfficePassword;
                    model.CustomerOfficeId = itm.e.CustomerOfficeId;
                    model.EMPPassword = itm.e.EMPPassword;
                    model.EMPUserId = itm.e.EMPUserId;
                    model.CompanyName = itm.c.CompanyName;
                    model.BusinessOwnerFirstName = itm.c.BusinessOwnerFirstName;
                    model.PhysicalAddress1 = itm.c.PhysicalAddress1;
                    model.IsMSOUser = itm.c.IsMSOUser ?? false;
                    model.CityStateZip = itm.c.PhysicalCity + ',' + itm.c.PhysicalState + ',' + itm.c.PhysicalZipCode;
                    model.SalesforceParentID = itm.c.SalesforceParentID;
                    model.TransmitType = "";
                    model.IsMSOUser = itm.c.IsMSOUser ?? false;
                    //  model.IsAdditionalEFINAllowed = itm.c.IsAdditionalEFINAllowed ?? false;

                    if ((itm.c.ParentId ?? Guid.Empty) == Guid.Empty)
                    {
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
                    }


                    if (Customer.EntityId == (int)EMPConstants.Entity.SOME)
                    {
                        model.IsAdditionalEFINAllowed = true;
                    }
                    else if ((Customer.ParentId ?? Guid.Empty) != Guid.Empty)
                    {
                        if (ParentId != id)
                        {
                            var dbssb = db.SubSiteOfficeConfigs.Where(a => a.RefId == ParentId).FirstOrDefault();
                            if (dbssb != null)
                            {
                                //if (dbssb.SubSiteSendTaxReturn == true)
                                //    model.TransmitType = "All Sub-sites will transmit to the IRS (Transmitter) ";
                                //else if (dbssb.SubSiteSendTaxReturn == false)
                                //    model.TransmitType = "All Sub-sites will transmit to the Main Office (Feeder) ";
                                //else
                                //    model.TransmitType = "Mixed – Some Sub-sites will transmit to the IRS and some will transmit to the Main Office ";

                                if (dbssb.SOorSSorEFIN == 3)
                                {
                                    model.IsAdditionalEFINAllowed = true;
                                }

                            }
                        }
                    }

                    model.MSO = "";
                    string strBank = "";

                    if (Customer.EntityId != (int)EMPConstants.Entity.SOME)
                    {
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
                    }

                    model.Bank = strBank;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerLogin/ParentCustomerInfo", id);
            }
            return model;
        }

        public CustomerLoginModel IsUserExist(CustomerLoginModel _Dto)
        {
            throw new NotImplementedException();
        }
    }
}