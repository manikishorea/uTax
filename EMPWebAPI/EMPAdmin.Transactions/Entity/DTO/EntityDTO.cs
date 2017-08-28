﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPAdmin.Transactions.Entity.DTO
{
    public class EntityDTO
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StatusCode { get; set; }
        public string ParentName { get; set; }
        public int EntityCount { get; set; }
    }
}
