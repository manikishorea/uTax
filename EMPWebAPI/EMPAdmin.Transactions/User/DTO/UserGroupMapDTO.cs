using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPAdmin.Transactions.User.DTO
{
    public class UserGroupMapDTO
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> UserId { get; set; }
        public Nullable<System.Guid> GroupId { get; set; }

        public string UserName { get; set; }
        public string GroupName { get; set; }
        public string StatusCode { get; set; }

    }
}
