namespace EMPAdmin.Transactions.User.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EMPAdmin.Transactions.Group.DTO;
    using EMP.Core.DTO;

    public class UserDTO : CoreModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string IsActive { get; set; }
        public string GroupName { get; set; }
        public string AccessType { get; set; }
    }
}
