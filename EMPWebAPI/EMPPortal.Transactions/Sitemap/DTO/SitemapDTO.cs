using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;

namespace EMPPortal.Transactions.Sitemap.DTO
{
    public class SitemapDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public string DisplayClass { get; set; }
        public Nullable<bool> DisplayPartial { get; set; }
        public int? BaseEntityId { get; set; }
        public int? EntityId { get; set; }

        public Nullable<int> DisplayOrderBeforeAct { get; set; }
        public Nullable<int> DisplayOrderAfterAct { get; set; }
        public Nullable<int> DisplayBeforeVerify { get; set; }
        public int SitemapTypeId { get; set; }
    }

    public class UserRolePermission
    {
        public NewCustomerSignupPermissions NewCustomer { get; set; }
        public OfficeMgmtPermissions OfficeManamgement { get; set; }
        public ReportsPermission ReportPermissions { get; set; }
    }

    public class NewCustomerSignupPermissions
    {
        public bool ViewPermission { get; set; }
        public bool ViewCustomerInfo { get; set; }
        public bool Updatesw { get; set; }
    }

    public class OfficeMgmtPermissions
    {
        public bool ViewPermission { get; set; }
        public bool ResetPassword { get; set; }
        public bool AddSubOffice { get; set; }
        public bool OfficeMgmt { get; set; }
        public bool EnrollmentMgmt { get; set; }
        public bool UnlockEnrollment { get; set; }
        public bool ArchiveInfo { get; set; }
        public bool ShowPassword { get; set; }
    }

    public class ReportsPermission
    {
        public bool FeeReport { get; set; }
        public bool NoBankApp { get; set; }
        public bool Enrollstatus { get; set; }
        public bool LoginReport { get; set; }
    }
}
