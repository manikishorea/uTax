using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMPPortal.Utilities
{
    public class EMPPortalConstants
    {
        //public enum Entities
        //{
        //    uTax = 1,
        //    SO = 2,
        //    SOME = 3,
        //    MO = 4,
        //    MOSubSite = 5,
        //    SVB = 6,
        //    SVBSubSite = 7,
        //    SOME_SubSite = 8,
        //}

        public enum Entity
        {
            uTax = 1,
            SO = 2,
            SOME = 3,
            SOME_SS = 4,
            MO = 5,
            MO_SO = 6,
            MO_AE = 7,
            MO_AE_SS = 8,
            SVB = 9,
            SVB_SO = 10,
            SVB_MO = 11,
            SVB_MO_SO = 12,
            SVB_MO_AE = 13,
            SVB_MO_AE_SS = 14,
            SVB_AE = 15,
            SVB_AE_SS = 16
        }

        public enum BaseEntities
        {
            uTax = 0,
            SVB = 1,
            MO = 1,
            SO = 1,
            SO_ME = 1,
            AE = 2,
            AE_SS = 3,
        }
    }
}