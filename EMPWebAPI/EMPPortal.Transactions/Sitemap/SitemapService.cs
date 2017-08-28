using System;
using System.Linq;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPPortal.Transactions.Sitemap.DTO;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using EMP.Core.Utilities;
using EMPPortal.Transactions.Configuration.DTO;
using EMPPortal.Transactions.Configuration;
using EMPPortal.Transactions.EnrollmentBankSelectionInfo;
using EMPPortal.Transactions.DropDowns.DTO;
using EMPPortal.Transactions.DropDowns;

namespace EMPPortal.Transactions.Sitemap
{
    public class SitemapService : IDisposable
    {
        DatabaseEntities db;

        public IQueryable<SitemapDTO> GetSitemap(int? entityid, Guid UserId, string ptype, int? mainEntityid, Guid bankid)
        {
            try
            {
                List<int> SitemapTypeIds = new List<int>();
                SitemapTypeIds.Add((int)EMPConstants.SitemapType.ProfileConfiguration);

                if (entityid != 1 && (entityid == 5 || entityid == 9))
                {
                    SitemapTypeIds.Add((int)EMPConstants.SitemapType.MainSiteHome);
                }
                else if (entityid != 1 && entityid != 5 && entityid != 9)
                {
                    SitemapTypeIds.Add((int)EMPConstants.SitemapType.SubSiteHome);
                }

                bool isAdmin = false;
                CustomerConfigStatusService _CustomerConfigStatusService = new CustomerConfigStatusService();
                db = new DatabaseEntities();
                var IsActiveUser = 0;
                var IsVerified = false;
                var EFINStatus = 0;
                var CanEnroll = false;
                var CanEnrollForMain = false;
                bool IspaymentinConfig = false;

                bool IsTaxReturn = false;
                bool IsonHold = false;
                bool AddPaymentOptions = false;
                // bool IsActive = false;

                var User = db.OfficeManagements.Where(a => a.CustomerId == UserId).FirstOrDefault();

                ReportsPermission _rpt = new ReportsPermission();
                Guid feereport = new Guid("af155d4a-c29b-4dfe-9df6-e9765b35ec82");
                Guid noappsubreport = new Guid("abb91b1e-53fd-49fb-bdf7-4f180c0edc65");
                Guid enrstatusreport = new Guid("1c157f32-aa0f-4689-a27b-ca154e9763c4");
                Guid loginreport = new Guid("947b9d9e-22ce-4584-946a-db1ce6f9c7a1");


                if (User != null)
                {
                    IsTaxReturn = User.IsTaxReturn ?? false;
                    IsActiveUser = User.IsActivationCompleted ?? 0;
                    EFINStatus = User.EFINStatus ?? 0;
                    IsVerified = User.IsVerified ?? false;
                    CanEnroll = User.CanEnrollmentAllowed ?? false;
                    CanEnrollForMain = User.CanEnrollmentAllowedForMain ?? false;
                    if (User.IsActivationCompleted == 1 && IsVerified)
                    {
                        SitemapTypeIds.Add((int)EMPConstants.SitemapType.ReportsConfiguration);
                    }
                    IsonHold = User.IsHold ?? false;
                }
                else
                {
                    List<Guid> sitemaps = new List<Guid>();

                    sitemaps.Add(feereport);
                    sitemaps.Add(noappsubreport);
                    sitemaps.Add(enrstatusreport);
                    sitemaps.Add(loginreport);

                    Guid view = new Guid("FCACA5D7-F9F7-4531-8EAD-049B49A8CF89");

                    var user = (from r in db.UserRolesMaps
                                where r.UserId == UserId
                                select r).FirstOrDefault();
                    if (user != null)
                    {
                        var rolepermssions = (from s in db.SiteMapRolePermissions
                                              join p in db.SitemapPermissionMaps on s.PermissionId equals p.Id
                                              //join pm in db.PermissionMasters on p.PermissionId equals pm.Id
                                              where s.RoleId == user.RoleId && sitemaps.Contains(s.SiteMapId)
                                              select new { s.SiteMapId, p.PermissionId }).ToList();

                        _rpt.Enrollstatus = rolepermssions.Where(x => x.PermissionId == view && x.SiteMapId == enrstatusreport).FirstOrDefault() == null ? false : true;
                        _rpt.FeeReport = rolepermssions.Where(x => x.PermissionId == view && x.SiteMapId == feereport).FirstOrDefault() == null ? false : true;
                        _rpt.LoginReport = rolepermssions.Where(x => x.PermissionId == view && x.SiteMapId == loginreport).FirstOrDefault() == null ? false : true;
                        _rpt.NoBankApp = rolepermssions.Where(x => x.PermissionId == view && x.SiteMapId == noappsubreport).FirstOrDefault() == null ? false : true;
                    }
                    else
                    {
                        _rpt.Enrollstatus = true;
                        _rpt.FeeReport = true;
                        _rpt.LoginReport = true;
                        _rpt.NoBankApp = true;
                    }
                }

                if (ptype == "subconfig")
                {
                    return GetSitemap2(entityid, UserId, ptype, IsVerified);
                }

                if (ptype == "config")
                {
                    SitemapTypeIds.Add((int)EMPConstants.SitemapType.EMPConfiguration);
                    SitemapTypeIds.Add((int)EMPConstants.SitemapType.ProfileConfiguration);
                }
                else if (ptype == "enrollment")
                {
                    if (IsonHold && (entityid == (int)EMPConstants.Entity.MO || entityid == (int)EMPConstants.Entity.SVB || entityid == (int)EMPConstants.Entity.SO))
                    {
                        AddPaymentOptions = true;
                    }
                    if (!AddPaymentOptions)
                    {
                        if (entityid == (int)EMPConstants.Entity.SO || entityid == (int)EMPConstants.Entity.SOME || entityid == (int)EMPConstants.Entity.MO || entityid == (int)EMPConstants.Entity.SVB)
                        {
                            if (CanEnroll && IsTaxReturn)
                            {
                                SitemapTypeIds.Add((int)EMPConstants.SitemapType.EnrollmentConfiguration);
                            }
                        }
                        else
                        {
                            if (CanEnrollForMain)
                            {
                                SitemapTypeIds.Add((int)EMPConstants.SitemapType.EnrollmentConfiguration);
                            }
                        }
                    }
                }
                else
                {
                    if (entityid == (int)EMPConstants.Entity.uTax)
                    {
                        IsActiveUser = 1;
                        IsVerified = true;
                        isAdmin = true;
                        SitemapTypeIds.Add((int)EMPConstants.SitemapType.uTaxConfiguration);
                        if (_rpt.Enrollstatus || _rpt.FeeReport || _rpt.LoginReport || _rpt.NoBankApp)
                            SitemapTypeIds.Add((int)EMPConstants.SitemapType.ReportsConfiguration);
                    }
                    else
                    {
                        if (entityid != (int)EMPConstants.Entity.SO && entityid != (int)EMPConstants.Entity.SOME)
                            SitemapTypeIds.Add((int)EMPConstants.SitemapType.EMPConfiguration);
                        SitemapTypeIds.Add((int)EMPConstants.SitemapType.PaymentConfiguration);

                        if (entityid == (int)EMPConstants.Entity.SO || entityid == (int)EMPConstants.Entity.SOME || entityid == (int)EMPConstants.Entity.MO || entityid == (int)EMPConstants.Entity.SVB)
                        {
                            if (CanEnroll && IsTaxReturn)
                            {
                                bool _issharedefin = false;
                                if (entityid == (int)EMPConstants.Entity.SO || entityid == (int)EMPConstants.Entity.SOME)
                                {
                                    var efinino = db.SubSiteOfficeConfigs.Where(x => x.RefId == UserId).FirstOrDefault();
                                    if (efinino != null)
                                    {
                                        if (!string.IsNullOrEmpty(efinino.EFINOwnerSite))
                                            _issharedefin = true;
                                    }
                                }
                                if (!_issharedefin && !IsonHold)
                                    SitemapTypeIds.Add((int)EMPConstants.SitemapType.EnrollmentConfiguration);
                                else
                                {
                                    if (IsonHold)
                                    {
                                        AddPaymentOptions = true;
                                    }
                                    if (!SitemapTypeIds.Contains((int)EMPConstants.SitemapType.SubSiteHome))
                                        SitemapTypeIds.Add((int)EMPConstants.SitemapType.SubSiteHome);
                                    if (SitemapTypeIds.Contains((int)EMPConstants.SitemapType.ReportsConfiguration))
                                        SitemapTypeIds.Remove((int)EMPConstants.SitemapType.ReportsConfiguration);
                                }
                            }
                            else
                            {
                                if (SitemapTypeIds.Contains((int)EMPConstants.SitemapType.ReportsConfiguration))
                                    SitemapTypeIds.Remove((int)EMPConstants.SitemapType.ReportsConfiguration);
                            }
                        }
                        else
                        {
                            if (CanEnrollForMain)
                            {
                                SitemapTypeIds.Add((int)EMPConstants.SitemapType.EnrollmentConfiguration);
                            }

                        }
                    }
                }

                DatabaseArcEntities dbArc = new DatabaseArcEntities();
                var ArchiveExist = dbArc.emp_CustomerInformation.Where(o => o.Id == UserId).FirstOrDefault();
                if (ArchiveExist != null)
                {
                    if (entityid != 1 && (entityid == 5 || entityid == 9))
                    {
                        SitemapTypeIds.Add((int)EMPConstants.SitemapType.ArchiveConfiguration);
                    }
                    else if (entityid != 1)
                    {
                        SitemapTypeIds.Add((int)EMPConstants.SitemapType.ArchiveSubConfiguration);
                    }
                }

                // int SitemapTypeId = (int)EMPConstants.SitemapType.EMPConfiguration;

                Nullable<int> BaseEntityId = 0;
                Nullable<int> EntityId = 0;

                var isownerefin = db.SubSiteOfficeConfigs.Where(x => x.RefId == UserId).FirstOrDefault();

                IQueryable<CustomerConfigStatusDTO> CustomerConfigStatusDTOList = _CustomerConfigStatusService.GetById(UserId);

                List<SitemapDTO> lstSitemap = new List<SitemapDTO>();
                var sitemap = (from sitem in db.SitemapMasters
                               join siteent in db.SitemapEntities on sitem.Id equals siteent.SitemapId
                               join entmst in db.EntityMasters on siteent.EntityId equals entmst.Id
                               where
                               sitem.IsVisible == true
                               && siteent.StatusCode != EMPConstants.InActive
                               && sitem.StatusCode != EMPConstants.InActive
                               && siteent.EntityId == entityid
                               && SitemapTypeIds.Contains(sitem.SitemapTypeID ?? 0)
                               select new { sitem, siteent, EntityId = entmst.Id, BaseEntityId = entmst.BaseEntityId }).Distinct().OrderBy(x => x.sitem.DisplayOrder).ToList();

                if (ptype == "enrollment")
                {
                    var enrollId = new Guid("C333B448-B287-4C08-929B-0797F986CA6F");
                    var enrolls = sitemap.Where(x => x.sitem.Id == enrollId || x.sitem.ParentId == enrollId).ToList();
                    sitemap = enrolls;

                    if (AddPaymentOptions)
                    {
                        List<Guid> payids = new List<Guid>();
                        payids.Add(new Guid("0EDA5D25-591C-4E01-A845-FB580572CFF5"));
                        payids.Add(new Guid("0EDA5D25-591C-4E01-A845-FB580572CFE8"));
                        if (sitemap.Where(x => x.sitem.Id == new Guid("C333B448-B287-4C08-929B-0797F986CA6F")).FirstOrDefault() == null)
                            payids.Add(new Guid("C333B448-B287-4C08-929B-0797F986CA6F"));
                        var sitemap1 = (from sitem in db.SitemapMasters
                                        join siteent in db.SitemapEntities on sitem.Id equals siteent.SitemapId
                                        join entmst in db.EntityMasters on siteent.EntityId equals entmst.Id
                                        where
                                        sitem.IsVisible == true
                                        && siteent.StatusCode != EMPConstants.InActive
                                        && sitem.StatusCode != EMPConstants.InActive
                                        && siteent.EntityId == entityid
                                        && payids.Contains(sitem.Id)
                                        select new { sitem, siteent, EntityId = entmst.Id, BaseEntityId = entmst.BaseEntityId }).Distinct().OrderBy(x => x.sitem.DisplayOrder).ToList();

                        sitemap.AddRange(sitemap1);
                    }
                }
                else if (ptype == "config")
                {
                    var officeId = new Guid("6DF12CEF-8BDB-4D84-9057-095590EA0A79");
                    var mofunc = sitemap.Where(x => x.sitem.Id == officeId || x.sitem.ParentId == officeId).ToList();
                    sitemap = mofunc;

                    if (!IsTaxReturn && User.IsTaxReturn != null && (entityid == (int)EMPConstants.Entity.MO || entityid == (int)EMPConstants.Entity.SVB))
                    {
                        List<Guid> payids = new List<Guid>();
                        payids.Add(new Guid("0EDA5D25-591C-4E01-A845-FB580572CFF5"));
                        payids.Add(new Guid("0EDA5D25-591C-4E01-A845-FB580572CFE8"));
                        //var taxreturn = db.MainOfficeConfigurations.Where(x => x.emp_CustomerInformation_ID == UserId && x.StatusCode == EMPConstants.Active).FirstOrDefaultAsync();
                        var sitemap1 = (from sitem in db.SitemapMasters
                                        join siteent in db.SitemapEntities on sitem.Id equals siteent.SitemapId
                                        join entmst in db.EntityMasters on siteent.EntityId equals entmst.Id
                                        where
                                        sitem.IsVisible == true
                                        && siteent.StatusCode != EMPConstants.InActive
                                        && sitem.StatusCode != EMPConstants.InActive
                                        && siteent.EntityId == entityid
                                        && payids.Contains(sitem.Id)
                                        select new { sitem, siteent, EntityId = entmst.Id, BaseEntityId = entmst.BaseEntityId }).Distinct().OrderBy(x => x.sitem.DisplayOrder).ToList();

                        foreach (var si in sitemap1)
                        {
                            si.sitem.ParentId = sitemap[1].sitem.ParentId;
                        }

                        IspaymentinConfig = true;
                        sitemap.AddRange(sitemap1);
                    }
                }
                else if (ptype == "subconfig")
                {
                    var officeId = new Guid("90348FBC-1255-48E8-8AD2-461301E74AE2");
                    var mofunc = sitemap.Where(x => x.sitem.Id == officeId || x.sitem.ParentId == officeId).ToList();
                    sitemap = mofunc;
                }
                else if (User != null)
                {
                    if (!IsTaxReturn && (entityid == (int)EMPConstants.Entity.MO || entityid == (int)EMPConstants.Entity.SVB))
                    {
                        List<Guid> payids = new List<Guid>();
                        payids.Add(new Guid("0EDA5D25-591C-4E01-A845-FB580572CFF5"));
                        payids.Add(new Guid("0EDA5D25-591C-4E01-A845-FB580572CFE8"));
                        //var taxreturn = db.MainOfficeConfigurations.Where(x => x.emp_CustomerInformation_ID == UserId && x.StatusCode == EMPConstants.Active).FirstOrDefaultAsync();
                        var sitemap1 = (from sitem in db.SitemapMasters
                                        join siteent in db.SitemapEntities on sitem.Id equals siteent.SitemapId
                                        join entmst in db.EntityMasters on siteent.EntityId equals entmst.Id
                                        where
                                        sitem.IsVisible == true
                                        && siteent.StatusCode != EMPConstants.InActive
                                        && sitem.StatusCode != EMPConstants.InActive
                                        && siteent.EntityId == entityid
                                        && payids.Contains(sitem.Id)
                                        select new { sitem, siteent, EntityId = entmst.Id, BaseEntityId = entmst.BaseEntityId }).Distinct().OrderBy(x => x.sitem.DisplayOrder).ToList();

                        foreach (var si in sitemap1)
                        {
                            si.sitem.ParentId = sitemap[1].sitem.ParentId;
                        }

                        IspaymentinConfig = true;
                        sitemap.AddRange(sitemap1);
                    }
                    if (AddPaymentOptions)
                    {
                        List<Guid> payids = new List<Guid>();
                        payids.Add(new Guid("0EDA5D25-591C-4E01-A845-FB580572CFF5"));
                        payids.Add(new Guid("0EDA5D25-591C-4E01-A845-FB580572CFE8"));
                        if (sitemap.Where(x => x.sitem.Id == new Guid("C333B448-B287-4C08-929B-0797F986CA6F")).FirstOrDefault() == null)
                            payids.Add(new Guid("C333B448-B287-4C08-929B-0797F986CA6F"));
                        var sitemap1 = (from sitem in db.SitemapMasters
                                        join siteent in db.SitemapEntities on sitem.Id equals siteent.SitemapId
                                        join entmst in db.EntityMasters on siteent.EntityId equals entmst.Id
                                        where
                                        sitem.IsVisible == true
                                        && siteent.StatusCode != EMPConstants.InActive
                                        && sitem.StatusCode != EMPConstants.InActive
                                        && siteent.EntityId == entityid
                                        && payids.Contains(sitem.Id)
                                        select new { sitem, siteent, EntityId = entmst.Id, BaseEntityId = entmst.BaseEntityId }).Distinct().OrderBy(x => x.sitem.DisplayOrder).ToList();

                        sitemap.AddRange(sitemap1);
                    }
                }

                var custInfo = db.emp_CustomerInformation.Where(x => x.Id == UserId).FirstOrDefault();
                bool _isefilepayment = true, _isbalpayment = true;
                bool _isefinno = true;
                bool _feeaddonno = false;
                bool _submanageenroll = true;


                DropDownService ddService = new DropDownService();
                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                EntityHierarchyDTOs = ddService.GetEntityHierarchies(UserId);

                Guid TopParentId = Guid.Empty;
                Guid FeeSourceParentId = Guid.Empty;

                if (EntityHierarchyDTOs.Count > 0)
                {
                    var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                    TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;
                }

                if (custInfo != null)
                {
                    if (custInfo.ParentId != null && custInfo.ParentId != Guid.Empty)
                    {
                        var efinino = db.SubSiteOfficeConfigs.Where(x => x.RefId == UserId).FirstOrDefault();
                        if (efinino != null)
                        {
                            if (!string.IsNullOrEmpty(efinino.EFINOwnerSite))
                                _isefinno = false;
                        }
                        if (TopParentId != Guid.Empty)
                        {
                            var enrollmanage = db.SubSiteConfigurations.Where(x => x.emp_CustomerInformation_ID == TopParentId).FirstOrDefault();
                            if (enrollmanage != null)
                            {
                                if ((!enrollmanage.IsuTaxManageingEnrolling ?? false) && (!enrollmanage.IsuTaxPortalEnrollment ?? false))
                                    _submanageenroll = false;
                            }
                        }
                    }
                }


                var DefaultBankName = string.Empty;
                var DefaultBankId = Guid.Empty;
                var EditingBankId = bankid;

                int BankSubmissionStatus = 0;
                DefaultBankModel DefaultBank = new DefaultBankModel();

                DefaultBank = (from enrollbank in db.EnrollmentBankSelections
                                   //  join bankmast in db.BankMasters on enrollbank.BankId equals bankmast.Id
                               where enrollbank.CustomerId == UserId && enrollbank.StatusCode == EMPConstants.Active && enrollbank.BankSubmissionStatus == 1
                               orderby enrollbank.BankSubmissionStatus descending, enrollbank.LastUpdatedDate descending
                               select new DefaultBankModel { BankId = enrollbank.BankId, BankSubmissionStatus = enrollbank.BankSubmissionStatus ?? 0, LastUpdatedDate = enrollbank.LastUpdatedDate }).FirstOrDefault();


                if (DefaultBank != null)
                {
                    DefaultBankId = DefaultBank.BankId;
                    BankSubmissionStatus = DefaultBank.BankSubmissionStatus;

                    if (bankid == DefaultBankId || bankid == Guid.Empty)
                    {
                        EditingBankId = DefaultBank.BankId;
                        DefaultBankName = db.BankMasters.Where(o => o.Id == DefaultBankId).Select(o => o.BankName).FirstOrDefault();
                    }
                }
                else
                {
                    DefaultBankModel DefaultBank2 = new DefaultBankModel();
                    if (bankid == Guid.Empty)
                    {
                        DefaultBank2 = (from enrollbank in db.EnrollmentBankSelections
                                        where enrollbank.CustomerId == UserId && enrollbank.StatusCode == EMPConstants.Active
                                        orderby enrollbank.BankSubmissionStatus descending, enrollbank.LastUpdatedDate descending
                                        select new DefaultBankModel { BankId = enrollbank.BankId, BankSubmissionStatus = enrollbank.BankSubmissionStatus ?? 0, LastUpdatedDate = enrollbank.LastUpdatedDate }).FirstOrDefault();

                        if (DefaultBank2 != null)
                        {
                            EditingBankId = DefaultBank2.BankId;
                        }
                    }
                    else
                    {
                        var item = (from enrollbank in db.EnrollmentBankSelections
                                    where enrollbank.CustomerId == UserId && enrollbank.StatusCode == EMPConstants.Active && enrollbank.BankId == bankid
                                    select enrollbank).FirstOrDefault();

                        if (item == null)
                        {
                            EditingBankId = Guid.Empty;
                        }
                    }
                }

                if (entityid != (int)EMPConstants.Entity.SO && entityid != (int)EMPConstants.Entity.SOME && entityid != (int)EMPConstants.Entity.SOME_SS && EditingBankId != Guid.Empty)
                {
                    var SSBConfig = db.SubSiteBankConfigs.Where(o => o.emp_CustomerInformation_ID == TopParentId && o.BankMaster_ID == EditingBankId).ToList();
                    if (SSBConfig.Count() == 0)
                    {
                        EditingBankId = Guid.Empty;
                    }
                }


                bool _isnonebank = false;
                var banksel = db.EnrollmentBankSelections.Where(x => x.CustomerId == UserId && x.StatusCode == EMPConstants.Active && x.BankId == EditingBankId).FirstOrDefault();
                if (banksel != null)
                {
                    _isnonebank = banksel.BankId == Guid.Empty;
                }

                List<Guid> MenuGuids = new List<Guid>();
                MenuGuids.Add(new Guid("067c03a3-34f1-4143-beae-35327a8fca44")); //Bank Selection
                MenuGuids.Add(new Guid("0feeb0fe-d0e7-4370-8733-dd5f7d2041fc")); //Bank Enrollment
                MenuGuids.Add(new Guid("a55334d1-3960-44c4-8cf1-e3ba9901f2be")); //Fee Reimbersment
                MenuGuids.Add(new Guid("0eda5d25-591c-4e01-a845-fb580572cff5")); //e-Filing Payment
                MenuGuids.Add(new Guid("0eda5d25-591c-4e01-a845-fb580572cfe8")); //Outstanding Balance Payment
                MenuGuids.Add(new Guid("98a706d7-031f-4c5d-8cc4-d32cc7658b69")); //Enrollment Summary
                MenuGuids.Add(new Guid("d343b33e-e33b-454a-a7b1-dee6bd0cf86c")); //Bank Selection

                foreach (var itm in sitemap)
                {
                    SitemapDTO omodel = new SitemapDTO();
                    omodel.Id = itm.sitem.Id;
                    omodel.Name = itm.sitem.Name == "Main O\u001fffice Configuration" ? "Main Office Configuration" : itm.sitem.Name;
                    omodel.Description = itm.sitem.Description;
                    omodel.DisplayClass = itm.siteent.DisplayClass;


                    if (itm.sitem.Name == "New Customer Signups")
                    {
                        int Records = db.emp_CustomerInformation.Where(o => o.StatusCode == EMPConstants.NewCustomer).ToList().Count;
                        if (Records > 0)
                        {
                            omodel.Name = "New Customer Signups (" + Records + ")";
                            omodel.Description = "(" + Records.ToString() + ")";
                        }
                        else
                        {
                            omodel.Name = "New Customer Signups";
                            omodel.Description = "";
                        }
                    }

                    //Bank Selection
                    if (itm.sitem.Id.ToString().ToLower() == "067c03a3-34f1-4143-beae-35327a8fca44")
                    {
                        omodel.Name = !string.IsNullOrEmpty(DefaultBankName) ? "(" + DefaultBankName + ") " + itm.sitem.Name : itm.sitem.Name;
                    }

                    if (MenuGuids.Contains(omodel.Id))
                    {
                        var CustomerConfigStatus = CustomerConfigStatusDTOList.Where(o => o.SitemapId.ToLower() == omodel.Id.ToString().ToLower() && o.CustomerId.ToLower() == UserId.ToString().ToLower() && o.bankid.ToLower() == EditingBankId.ToString().ToLower()).FirstOrDefault();
                        if (CustomerConfigStatus != null)
                        {
                            omodel.DisplayClass = CustomerConfigStatus.Status;
                        }
                    }
                    else
                    {
                        var CustomerConfigStatus = CustomerConfigStatusDTOList.Where(o => o.SitemapId.ToLower() == omodel.Id.ToString().ToLower() && o.CustomerId.ToLower() == UserId.ToString().ToLower()).FirstOrDefault();
                        if (CustomerConfigStatus != null)
                        {
                            omodel.DisplayClass = CustomerConfigStatus.Status;
                        }
                    }

                    omodel.ParentId = itm.sitem.ParentId;
                    if (mainEntityid != entityid)
                    {
                        if (itm.sitem.ParentId != null)
                        {
                            omodel.URL = itm.sitem.URL + "?Id=" + UserId.ToString() + "&entitydisplayid=" + User.BaseEntityId + "&entityid=" + User.EntityId + "&ParentId=" + ((User.ParentId != null && User.ParentId != Guid.Empty) ? TopParentId : Guid.Empty).ToString() + "&ptype=" + ptype;
                            if (EditingBankId != Guid.Empty)
                                omodel.URL = omodel.URL + "&bankid=" + EditingBankId;
                        }
                        else
                            omodel.URL = itm.sitem.URL;
                    }
                    else
                    {
                        omodel.URL = itm.sitem.URL;
                        if (EditingBankId != Guid.Empty && itm.sitem.SitemapTypeID != (int)EMPConstants.SitemapType.ReportsConfiguration)
                            omodel.URL = omodel.URL + "?bankid=" + EditingBankId;
                    }
                    omodel.StatusCode = itm.sitem.StatusCode;
                    omodel.DisplayOrder = itm.sitem.DisplayOrder;

                    omodel.DisplayPartial = itm.siteent.DisplayPartial;
                    omodel.DisplayOrderBeforeAct = itm.sitem.DisplayOrderBeforeAct;
                    omodel.DisplayOrderAfterAct = itm.sitem.DisplayOrderAfterAct;
                    omodel.DisplayBeforeVerify = itm.sitem.DisplayBeforeVerify ?? 0;

                    omodel.BaseEntityId = itm.BaseEntityId;
                    omodel.EntityId = itm.EntityId;
                    BaseEntityId = itm.BaseEntityId;
                    EntityId = itm.EntityId;

                    var isshowAfter = itm.sitem.IsDisplayAfterActivation ?? 0;
                    var isshowBefore = itm.sitem.IsDisplayBeforeActvation ?? 0;

                    ////Service Bureau And Transmission Fee Add-On
                    //if (itm.sitem.Id.ToString().ToUpper() == "60025459-7568-4A77-B152-F81904AAAA63")
                    //{

                    //}

                    // Affiliate Configuration
                    if (!IsVerified && itm.sitem.Id.ToString().ToUpper() == "2F7D1B90-78AA-4A93-85EC-81CD8B10A545")
                        continue;

                    // Office Configuration
                    if (!IsVerified && itm.sitem.Id.ToString().ToUpper() == "FC32DB13-6AEC-488E-BAFE-19ACB3399E57")
                        continue;

                    // Bank selection and Bank enrollment
                    if ((itm.sitem.Id.ToString().ToLower() == "067c03a3-34f1-4143-beae-35327a8fca44" || itm.sitem.Id.ToString().ToLower() == "0feeb0fe-d0e7-4370-8733-dd5f7d2041fc"))
                    {
                        if (!IsVerified)
                            continue;
                        if (!_submanageenroll && (mainEntityid != (int)EMPConstants.Entity.uTax) && (mainEntityid != (int)EMPConstants.Entity.MO) && (mainEntityid != (int)EMPConstants.Entity.SVB))
                            continue;
                        if (!_isefinno)
                            continue;
                        if (_isnonebank && itm.sitem.Id.ToString().ToLower() == "0feeb0fe-d0e7-4370-8733-dd5f7d2041fc")
                            continue;
                    }

                    // Skipping Fee-Reiumbersment based on Add-on selection for SO only
                    if (itm.sitem.Id.ToString() == "a55334d1-3960-44c4-8cf1-e3ba9901f2be" &&
                        (EntityId == (int)EMPConstants.Entity.SO || EntityId == (int)EMPConstants.Entity.SOME || EntityId == (int)EMPConstants.Entity.SOME_SS))
                    {
                        if (!IsVerified)
                            continue;

                        bool addon = new EnrollmentBankSelectionService().getAddonSelection(UserId, EditingBankId == Guid.Empty ? DefaultBankId : EditingBankId);

                        bool iprsel = true;
                        // iProtect GUID
                        Guid affGuid = new Guid("25a4379b-4df1-4a65-aec1-30dcd587eeb7");
                        // checking the iprotect affiliate condition
                        if (entityid == (int)EMPConstants.Entity.SO)
                        {
                            var ismapped = db.AffiliationProgramEntityMaps.Where(x => x.AffiliateProgramId == affGuid && x.EntityId == entityid).FirstOrDefault();
                            if (ismapped != null)
                            {
                                var isselected = db.EnrollmentAffiliateConfigurations.Where(x => x.CustomerId == UserId && x.AffiliateProgramId == affGuid && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                                if (isselected == null)
                                    iprsel = false;
                            }
                        }
                        if (!addon)
                            _feeaddonno = true;

                        bool rebate = true;
                        if (EntityId == (int)EMPConstants.Entity.SO && !custInfo.Quote_Rebate__c)
                            rebate = false;

                        if (!rebate && !iprsel && !addon)
                            continue;

                        if (!_isefinno) // efin in subsite config
                            continue;
                    }

                    // Checking/Skipping E-file only view
                    if (itm.sitem.Id.ToString() == "0eda5d25-591c-4e01-a845-fb580572cff5")
                    {
                        if (!IsVerified)
                            continue;
                        if ((custInfo.Federal_EF_Fee_New__c ?? 0) == 0 && (custInfo.State_EF_Fee_New__c ?? 0) == 0)
                        {
                            _isefilepayment = false;
                            continue;
                        }
                    }

                    // Checking/Skipping Out standing balance view
                    if (itm.sitem.Id.ToString() == "0eda5d25-591c-4e01-a845-fb580572cfe8")
                    {
                        if (!IsVerified)
                            continue;
                        bool _canshow = false;
                        if (!string.IsNullOrEmpty(custInfo.Cash_Saver__c) || !string.IsNullOrEmpty(custInfo.LOC_Program_Participant__c) || (custInfo.A_R_Amount_Due_Credit__c ?? 0) != 0)
                        {
                            if (!string.IsNullOrEmpty(custInfo.Cash_Saver__c))
                            {
                                if (custInfo.Cash_Saver__c.ToLower() == "true" && (custInfo.pymt__Balance__c ?? 0) != 0)
                                    _canshow = true;
                            }
                            if (!string.IsNullOrEmpty(custInfo.LOC_Program_Participant__c))
                            {
                                if (custInfo.LOC_Program_Participant__c.ToLower() == "true" && (custInfo.Total_Amount_Loaned__c ?? 0) != 0)
                                    _canshow = true;
                            }
                            if ((custInfo.A_R_Amount_Due_Credit__c ?? 0) != 0)
                                _canshow = true;
                        }
                        if (!_canshow)
                        {
                            _isbalpayment = false;
                            continue;
                        }
                    }

                    //checking for summary
                    if (itm.sitem.Id.ToString() == "98a706d7-031f-4c5d-8cc4-d32cc7658b69")
                    {
                        if (!IsVerified)
                            continue;

                        //bank enrollment
                        var bankenroll = CustomerConfigStatusDTOList.Where(o => o.SitemapId.ToUpper().Contains("0FEEB0FE-D0E7-4370-8733-DD5F7D2041FC") && o.bankid.ToLower() == EditingBankId.ToString().ToLower()).Count();
                        if (bankenroll <= 0 && !_isnonebank)
                            continue;

                        if (!_isnonebank)
                        {
                            // Fee reimb
                            if (EntityId == (int)EMPConstants.Entity.SO || EntityId == (int)EMPConstants.Entity.SOME || EntityId == (int)EMPConstants.Entity.SOME_SS)
                            {
                                var feereium = CustomerConfigStatusDTOList.Where(x => x.SitemapId.ToLower().Contains("a55334d1-3960-44c4-8cf1-e3ba9901f2be") && x.bankid.ToLower() == EditingBankId.ToString().ToLower()).Count();
                                if (feereium <= 0 && !_feeaddonno)
                                    continue;
                            }
                        }

                        //payment options
                        if (custInfo.ParentId == null || custInfo.ParentId == Guid.Empty)
                        {
                            bool _canshow = true;
                            if (_isefilepayment)
                            {
                                var efile = CustomerConfigStatusDTOList.Where(x => x.SitemapId.ToUpper().Contains("0EDA5D25-591C-4E01-A845-FB580572CFF5") && x.bankid.ToLower() == EditingBankId.ToString().ToLower()).Count();
                                if (efile <= 0)
                                    _canshow = false;
                            }
                            if (_isbalpayment)
                            {
                                var outbal = CustomerConfigStatusDTOList.Where(x => x.SitemapId.ToUpper().Contains("0EDA5D25-591C-4E01-A845-FB580572CFE8") && x.bankid.ToLower() == EditingBankId.ToString().ToLower()).Count();
                                if (outbal <= 0)
                                    _canshow = false;
                            }
                            if (!_canshow)
                                continue;
                        }
                    }

                    // Bank status selection
                    if (itm.sitem.Id.ToString() == "d343b33e-e33b-454a-a7b1-dee6bd0cf86c")
                    {
                        List<string> bankstatus = new List<string>();
                        bankstatus.Add(EMPConstants.Submitted);
                        bankstatus.Add(EMPConstants.EnrPending);
                        bankstatus.Add(EMPConstants.Approved);
                        bankstatus.Add(EMPConstants.Rejected);
                        bankstatus.Add(EMPConstants.Denied);

                        var banks = db.BankEnrollments.Where(x => x.CustomerId == UserId && x.IsActive == true && bankstatus.Contains(x.StatusCode) && x.ArchiveStatusCode != EMPConstants.Archive).Count();
                        if (banks < 2)
                            continue;
                    }

                    if (IsActiveUser == 1)
                    {
                        if (isshowAfter == 0 && isshowBefore == 1)
                            continue;
                    }
                    else
                    {
                        if (isshowAfter == 1 && isshowBefore == 0)
                            continue;
                    }

                    if (isAdmin && itm.sitem.Id == enrstatusreport && !_rpt.Enrollstatus)
                    {
                        continue;
                    }
                    if (isAdmin && itm.sitem.Id == feereport && !_rpt.FeeReport)
                    {
                        continue;
                    }
                    if (isAdmin && itm.sitem.Id == loginreport && !_rpt.LoginReport)
                    {
                        continue;
                    }
                    if (isAdmin && itm.sitem.Id == noappsubreport && !_rpt.NoBankApp)
                    {
                        continue;
                    }

                    bool manuFlag = false;
                    if (itm.sitem.Id == new Guid("60025459-7568-4a77-b152-f81904aaaa63")) //This is Service Bureau And Transmission Fee Add-On
                    {
                        if (CustomerConfigStatusDTOList.Any(a => a.SitemapId == "0eda5d25-591c-4e01-a845-fb580572ade8")) //This is Main Office Configuration
                        {
                            manuFlag = true;
                        }

                        if (CustomerConfigStatusDTOList.Any(a => a.SitemapId == "68882c05-5914-4fdb-b284-e33d6c029f5a")) //This is Sub-Site Configuration
                        {
                            manuFlag = true;
                        }

                        bool ipsel = true;
                        // iProtect GUID
                        Guid affGuid = new Guid("25a4379b-4df1-4a65-aec1-30dcd587eeb7");
                        // checking the iprotect affiliate condition
                        if (entityid == (int)EMPConstants.Entity.MO || entityid == (int)EMPConstants.Entity.SVB)
                        {
                            var ismapped = db.AffiliationProgramEntityMaps.Where(x => x.AffiliateProgramId == affGuid && x.EntityId == entityid).FirstOrDefault();
                            if (ismapped != null)
                            {
                                var isselected = db.SubSiteAffiliateProgramConfigs.Where(x => x.emp_CustomerInformation_ID == UserId && x.AffiliateProgramMaster_ID == affGuid).FirstOrDefault();
                                if (isselected == null)
                                    ipsel = false;
                            }
                        }

                        if (manuFlag || ipsel || custInfo.Quote_Rebate__c)
                        {
                            manuFlag = GetServiceTransmeterFeeLink(UserId);
                            if (manuFlag || ipsel || custInfo.Quote_Rebate__c)
                            {
                                lstSitemap.Add(omodel);
                            }
                        }
                    }
                    else
                    {
                        lstSitemap.Add(omodel);
                    }
                }

                if (!IsVerified)
                {
                    return lstSitemap.Where(o => o.DisplayBeforeVerify == 1).OrderBy(o => o.DisplayOrderAfterAct).ThenBy(o => o.DisplayOrder).AsQueryable();
                }
                if (IspaymentinConfig || ptype == null)
                {
                    var efile = lstSitemap.Where(x => x.Id == new Guid("0eda5d25-591c-4e01-a845-fb580572cff5")).FirstOrDefault();
                    if (efile != null && efile.ParentId != new Guid("c333b448-b287-4c08-929b-0797f986ca6f"))
                    {
                        efile.DisplayOrder = 6;
                        efile.DisplayOrderAfterAct = 3;
                    }
                    var outst = lstSitemap.Where(x => x.Id == new Guid("0eda5d25-591c-4e01-a845-fb580572cfe8")).FirstOrDefault();
                    if (outst != null && efile != null && efile.ParentId != new Guid("c333b448-b287-4c08-929b-0797f986ca6f"))
                    {
                        outst.DisplayOrder = 7;
                        outst.DisplayOrderAfterAct = 3;
                    }
                    var act = lstSitemap.Where(x => x.Id == new Guid("4fc65d1b-675f-4985-8022-9e8bd0ed735f")).FirstOrDefault();
                    if (act != null)
                        act.DisplayOrder = 8;
                    if (ptype == null && !(efile != null && efile.ParentId != new Guid("c333b448-b287-4c08-929b-0797f986ca6f")))
                        return lstSitemap.OrderBy(o => o.DisplayOrderAfterAct).ThenBy(o => o.DisplayOrder).AsQueryable();
                    return lstSitemap.OrderBy(o => o.DisplayOrderAfterAct).ThenBy(o => o.DisplayOrder).AsQueryable();
                }
                else
                    return lstSitemap.OrderBy(o => o.DisplayOrder).AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SitemapService/GetSitemap", Guid.Empty);
                return null;
            }
        }

        public IQueryable<SitemapDTO> GetSitemap2(int? entityid, Guid UserId, string ptype, bool IsVerified = false)
        {
            int subsiteid = (int)EMPConstants.SitemapType.SubSiteConfiguration;
            List<SitemapDTO> lstSitemap = new List<SitemapDTO>();
            List<SitemapMaster> sites = new List<SitemapMaster>();
            if (entityid == (int)EMPConstants.Entity.SOME_SS || entityid == (int)EMPConstants.Entity.SOME || entityid == (int)EMPConstants.Entity.SO)
            {
                sites = (from s in db.SitemapEntities
                         join m in db.SitemapMasters on s.SitemapId equals m.Id
                         where m.SitemapTypeID == subsiteid && s.EntityId == entityid && s.StatusCode == EMPConstants.Active && m.StatusCode == EMPConstants.Active
                         select m).Distinct().ToList();
            }
            else
            {
                sites = (from sitem in db.SitemapMasters
                         where sitem.SitemapTypeID == subsiteid

                         select sitem).Distinct().ToList();
            }

            CustomerConfigStatusService _CustomerConfigStatusService = new CustomerConfigStatusService();
            IQueryable<CustomerConfigStatusDTO> CustomerConfigStatusDTOList = _CustomerConfigStatusService.GetById(UserId);

            var User = db.OfficeManagements.Where(a => a.CustomerId == UserId).FirstOrDefault();

            foreach (var itm in sites)
            {
                SitemapDTO omodel = new SitemapDTO();
                omodel.Id = itm.Id;
                omodel.Name = itm.Name;
                omodel.ParentId = itm.ParentId;


                if (itm.ParentId != null)
                {
                    omodel.URL = itm.URL + "?Id=" + UserId.ToString() + "&entitydisplayid=" + User.BaseEntityId + "&entityid=" + User.EntityId + "&ParentId=" + (User.ParentId ?? Guid.Empty).ToString() + "&ptype=" + ptype;

                }
                else
                    omodel.URL = itm.URL;

                omodel.Description = itm.Description;
                omodel.StatusCode = itm.StatusCode;
                omodel.DisplayOrder = itm.DisplayOrder;
                omodel.DisplayClass = itm.ParentId == null ? "mainmenu" : "";
                omodel.DisplayPartial = false;
                omodel.DisplayOrderBeforeAct = itm.DisplayOrderBeforeAct;
                omodel.DisplayOrderAfterAct = itm.DisplayOrderAfterAct;
                omodel.DisplayBeforeVerify = itm.DisplayBeforeVerify ?? 0;

                var CustomerConfigStatus = CustomerConfigStatusDTOList.Where(o => o.SitemapId.ToLower() == omodel.Id.ToString().ToLower() && o.CustomerId.ToLower() == UserId.ToString().ToLower()).FirstOrDefault();
                if (CustomerConfigStatus != null)
                {
                    omodel.DisplayClass = CustomerConfigStatus.Status;
                }

                lstSitemap.Add(omodel);

            }

            if (!IsVerified)
            {
                return lstSitemap.Where(o => o.DisplayBeforeVerify == 1).OrderBy(o => o.DisplayOrderAfterAct).ThenBy(o => o.DisplayOrder).AsQueryable();
            }


            return lstSitemap.OrderBy(o => o.DisplayOrderAfterAct).ThenBy(o => o.DisplayOrder).AsQueryable();
        }

        public bool GetServiceTransmeterFeeLink(Guid UserID)
        {
            try
            {
                db = new DatabaseEntities();
                var CustomInfo = db.emp_CustomerInformation.Where(a => a.Id == UserID).FirstOrDefault();
                if (CustomInfo.uTaxNotCollectingSBFee == null || CustomInfo.uTaxNotCollectingSBFee == false)
                {
                    var subsiteFee = db.SubSiteFeeConfigs.Where(a => a.emp_CustomerInformation_ID == UserID);
                    bool iretval = false;
                    foreach (var itm in subsiteFee)
                    {
                        if (itm.IsAddOnFeeCharge == true)
                        {
                            iretval = true;
                            return true;
                        }

                        if (!iretval)
                        {
                            if (itm.IsSubSiteAddonFee == true)
                            {
                                iretval = true;
                                return true;
                            }
                        }
                    }

                    return iretval;
                }
                else
                {
                    var subsiteFee = db.SubSiteFeeConfigs.Where(a => a.emp_CustomerInformation_ID == UserID && a.ServiceorTransmission == 2);
                    bool iretval = false;
                    foreach (var itm in subsiteFee)
                    {
                        if (itm.ServiceorTransmission == 2)
                        {
                            if (itm.IsAddOnFeeCharge == true)
                            {
                                iretval = true;
                                // return true;
                            }
                            else
                            {
                                return false;
                            }

                            if (iretval)
                            {
                                if (itm.IsSubSiteAddonFee == true)
                                {
                                    iretval = true;
                                    return true;
                                }
                                else
                                {
                                    var subSideBankFee = db.SubSiteBankFeesConfigs.Any(a => a.emp_CustomerInformation_ID == UserID && a.SubSiteFeeConfig_ID == itm.ID && itm.ServiceorTransmission == 2 && (a.BankMaxFees > 0 || a.BankMaxFees_MSO > 0));
                                    return subSideBankFee;
                                }
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SitemapService/GetServiceTransmeterFeeLink", UserID);
                return false;
            }
        }

        public IQueryable<SitemapDTO> GetSubSitemap(int entityid, Guid CustomerId)
        {
            try
            {
                List<int> SitemapTypeIds = new List<int>();
                SitemapTypeIds.Add((int)EMPConstants.SitemapType.SubSiteConfiguration);
                SitemapTypeIds.Add((int)EMPConstants.SitemapType.EnrollmentConfiguration);
                //  int SitemapTypeId = (int)EMPConstants.SitemapType.SubSiteConfiguration;
                db = new DatabaseEntities();
                var sitemap = (from sitemapMaster in db.SitemapMasters
                               join siteent in db.SitemapEntities on sitemapMaster.Id equals siteent.SitemapId
                               where SitemapTypeIds.Contains(sitemapMaster.SitemapTypeID ?? 0) && sitemapMaster.IsVisible == true // == SitemapTypeId
                                 && siteent.EntityId == entityid
                                 && siteent.StatusCode != EMPConstants.InActive

                               select new SitemapDTO
                               {
                                   Id = sitemapMaster.Id,
                                   Name = sitemapMaster.Name,
                                   URL = sitemapMaster.URL,
                                   Description = sitemapMaster.Description,
                                   StatusCode = sitemapMaster.StatusCode,
                                   DisplayOrder = sitemapMaster.DisplayOrder,
                                   DisplayClass = siteent.DisplayClass,
                                   DisplayPartial = siteent.DisplayPartial
                               }).DefaultIfEmpty().Distinct().OrderBy(o => o.DisplayOrder);
                return sitemap;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SitemapService/GetSubSitemap", CustomerId);
                return new List<SitemapDTO>().AsQueryable();
            }
        }

        #region IDisposable Support
        public void Dispose()
        {
            db.Dispose();
            throw new NotImplementedException();
        }
        #endregion

        public UserRolePermission GetUserRolePermissions(Guid UserId)
        {
            try
            {
                UserRolePermission permission = new UserRolePermission();
                List<Guid> sitemaps = new List<Guid>();
                Guid officemgmt = new Guid("B303A3E8-1A4F-4638-B31C-50D1B4B8DB34");
                Guid newcustomer = new Guid("6ED791BD-1909-4E7A-B6B1-B76983FAEF30");
                Guid feereport = new Guid("af155d4a-c29b-4dfe-9df6-e9765b35ec82");
                Guid noappsubreport = new Guid("abb91b1e-53fd-49fb-bdf7-4f180c0edc65");
                Guid enrstatusreport = new Guid("1c157f32-aa0f-4689-a27b-ca154e9763c4");
                Guid loginreport = new Guid("947b9d9e-22ce-4584-946a-db1ce6f9c7a1");
                sitemaps.Add(newcustomer);
                sitemaps.Add(officemgmt);
                sitemaps.Add(noappsubreport);
                sitemaps.Add(feereport);
                sitemaps.Add(enrstatusreport);
                sitemaps.Add(loginreport);

                Guid view = new Guid("FCACA5D7-F9F7-4531-8EAD-049B49A8CF89");
                Guid viewcustinfo = new Guid("39B6AD6F-EFDA-47B4-BE91-8EC361AD9CEA");
                Guid updatesw = new Guid("B8FE8E3C-D547-4FC8-86C6-EF5236B1E6CC");
                Guid resetpswd = new Guid("0E5906C0-4B88-4C17-A45A-7EF913A62EF7");
                Guid AddSubOff = new Guid("FF635BB0-C551-45FB-BC28-D7698FABBDFE");
                Guid OffceMgmt = new Guid("FBCD19BD-A802-465B-98D7-7478B9F97504");
                Guid Enrollment = new Guid("9EEE75CD-BE57-49D1-A3F3-B2770DEE17CB");
                Guid UnlockEnrollment = new Guid("d25470c1-c428-49d6-8172-2b7b8f53cf34");
                Guid archiveinfo = new Guid("a8806d62-9525-49b2-ba96-2e92cf92cdfe");
                Guid showpassword = new Guid("a7f082fe-ace8-4df5-9c53-c50bee0d7de6");

                db = new DatabaseEntities();
                var user = (from r in db.UserRolesMaps
                            where r.UserId == UserId
                            select r).FirstOrDefault();
                if (user != null)
                {
                    var rolepermssions = (from s in db.SiteMapRolePermissions
                                          join p in db.SitemapPermissionMaps on s.PermissionId equals p.Id
                                          //join pm in db.PermissionMasters on p.PermissionId equals pm.Id
                                          where s.RoleId == user.RoleId && sitemaps.Contains(s.SiteMapId)
                                          select new { s.SiteMapId, p.PermissionId }).ToList();

                    NewCustomerSignupPermissions _newcust = new NewCustomerSignupPermissions();
                    _newcust.Updatesw = rolepermssions.Where(x => x.PermissionId == updatesw && x.SiteMapId == newcustomer).FirstOrDefault() == null ? false : true;
                    _newcust.ViewCustomerInfo = rolepermssions.Where(x => x.PermissionId == viewcustinfo && x.SiteMapId == newcustomer).FirstOrDefault() == null ? false : true;
                    _newcust.ViewPermission = rolepermssions.Where(x => x.PermissionId == view && x.SiteMapId == newcustomer).FirstOrDefault() == null ? false : true;

                    OfficeMgmtPermissions _off = new OfficeMgmtPermissions();
                    _off.AddSubOffice = rolepermssions.Where(x => x.PermissionId == AddSubOff && x.SiteMapId == officemgmt).FirstOrDefault() == null ? false : true;
                    _off.ResetPassword = rolepermssions.Where(x => x.PermissionId == resetpswd && x.SiteMapId == officemgmt).FirstOrDefault() == null ? false : true;
                    _off.ViewPermission = rolepermssions.Where(x => x.PermissionId == view && x.SiteMapId == officemgmt).FirstOrDefault() == null ? false : true;
                    _off.OfficeMgmt = rolepermssions.Where(x => x.PermissionId == OffceMgmt && x.SiteMapId == officemgmt).FirstOrDefault() == null ? false : true;
                    _off.EnrollmentMgmt = rolepermssions.Where(x => x.PermissionId == Enrollment && x.SiteMapId == officemgmt).FirstOrDefault() == null ? false : true;
                    _off.UnlockEnrollment = rolepermssions.Where(x => x.PermissionId == UnlockEnrollment && x.SiteMapId == officemgmt).FirstOrDefault() == null ? false : true;
                    _off.ArchiveInfo = rolepermssions.Where(x => x.PermissionId == archiveinfo && x.SiteMapId == officemgmt).FirstOrDefault() == null ? false : true;
                    _off.ShowPassword = rolepermssions.Where(x => x.PermissionId == showpassword && x.SiteMapId == officemgmt).FirstOrDefault() == null ? false : true;

                    ReportsPermission _rpt = new ReportsPermission();
                    _rpt.Enrollstatus = rolepermssions.Where(x => x.PermissionId == view && x.SiteMapId == enrstatusreport).FirstOrDefault() == null ? false : true;
                    _rpt.FeeReport = rolepermssions.Where(x => x.PermissionId == view && x.SiteMapId == feereport).FirstOrDefault() == null ? false : true;
                    _rpt.LoginReport = rolepermssions.Where(x => x.PermissionId == view && x.SiteMapId == loginreport).FirstOrDefault() == null ? false : true;
                    _rpt.NoBankApp = rolepermssions.Where(x => x.PermissionId == view && x.SiteMapId == noappsubreport).FirstOrDefault() == null ? false : true;

                    permission.NewCustomer = _newcust;
                    permission.OfficeManamgement = _off;
                    permission.ReportPermissions = _rpt;
                    return permission;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SitemapService/GetUserRolePermissions", UserId);
                return null;
            }
        }

        public UserRolePermission GetPartnerPermissions()
        {
            try
            {
                UserRolePermission permission = new UserRolePermission();
                List<Guid> sitemaps = new List<Guid>();
                Guid officemgmt = new Guid("F73EA95F-13C1-4CC7-8217-6E0AA171B68F");
                sitemaps.Add(officemgmt);

                Guid showpassword = new Guid("52678eb1-8e6e-4574-8ebc-df030e50a4bf");

                db = new DatabaseEntities();
                
                var rolepermssions = (from s in db.SiteMapRolePermissions
                                      join p in db.SitemapPermissionMaps on s.PermissionId equals p.Id
                                      where sitemaps.Contains(s.SiteMapId)
                                      select new { s.SiteMapId, p.PermissionId }).ToList();

                OfficeMgmtPermissions _off = new OfficeMgmtPermissions();
                _off.ShowPassword = rolepermssions.Where(x => x.PermissionId == showpassword && x.SiteMapId == officemgmt).FirstOrDefault() == null ? false : true;

                permission.OfficeManamgement = _off;
                return permission;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SitemapService/GetUserRolePermissions", Guid.Empty);
                return null;
            }
        }
    }

    public class DefaultBankModel
    {
        public Guid BankId { get; set; }
        public int BankSubmissionStatus { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
