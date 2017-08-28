using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPAdmin.Transactions.Settings.DTO
{
    public class SettingsDTO
    {
        public bool IsAccountCreation { get; set; }
        public Guid UserId { get; set; }
    }
}
