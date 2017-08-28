namespace EMPAdmin.Transactions.Group.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using EMP.Core.DTO;

    public class GroupDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
