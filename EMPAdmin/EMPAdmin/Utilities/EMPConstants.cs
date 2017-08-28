using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPAdmin.Utilities
{
    public static class EMPConstants
    {
        public static string Active = StatusCode.ACT.ToString();
        public static string InActive = StatusCode.INA.ToString();
        public static string Deleted = StatusCode.DEL.ToString();

        public enum StatusCode
        {
            ACT = 1,// Active status
            INA = 2, // In Active status
            DEL = 3,
        }

        public enum FeeType
        {
            ACT = 1,// Active status
            INA = 2, // In Active status
            DEL = 3,
        }

        public enum FeeNature
        {
            ACT = 1,// Active status
            INA = 2, // In Active status
            DEL = 3,
        }
    }
}
