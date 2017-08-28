namespace EMPAdmin.Transactions.User.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using EMP.Core.DTO;
    using EMPAdmin.Transactions.Group.DTO;
    using EMPAdmin.Transactions.Role.DTO;

    public class UserDetailDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public Nullable<int> EntityId { get; set; }
        public Nullable<System.Guid> CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string EmailConfirmationCode { get; set; }
        public string PasswordResetCode { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> IsActiveDate { get; set; }
        public string EntityName { get; set; }
        public string CustomerName { get; set; }

        public List<RoleDTO> Roles { get; set; }
        public GroupDTO Groups { get; set; }
    }

    public class xlinkModel
    {
        public string MasterId { get; set; }
        public string Password { get; set; }
        public Guid UserId { get; set; }
        public Int64 Id { get; set; }
        public string status { get; set; }

        public string CLAccountId { get; set; }
        public string CLLogin { get; set; }
        public string CLAccountPassword { get; set; }
    }


}
