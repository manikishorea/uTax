using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;
using EMPPortal.Transactions.DropDowns.DTO;

namespace EMPPortal.Transactions.SubSite.DTO
{
    public class SubSiteOnBoardingDTO : CoreModel
    {
        public string Id { get; set; }
        public string refId { get; set; }
        public Nullable<bool> IsuTaxManageOnboarding { get; set; }
        public string formtype { get; set; }
    }
}
