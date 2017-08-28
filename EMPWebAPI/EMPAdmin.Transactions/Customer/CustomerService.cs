using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using System.Threading.Tasks;
using EMPEntityFramework.Edmx;

using System.Data.Entity;
using EMPAdmin.Transactions.Customer.DTO;

namespace EMPAdmin.Transactions.Customer
{
    public class CustomerService : ICustomerService
    {
        // private readonly DatabaseEntities _db = new DatabaseEntities();
        // private readonly CustomerDTO _entity = new CustomerDTO();

        //public CustomerService(DatabaseEntities db, CustomerDTO entity)
        //{
        //    _db = db;
        //    _entity = entity;
        //}

        public DatabaseEntities _db = new DatabaseEntities();
        public CustomerDTO _customer = new CustomerDTO();

        public IQueryable<CustomerDTO> GetAllCustomer()
        {
            var customer = _db.CustomerMasters.Select(o => new CustomerDTO
            {
                Id = o.Id,
                Name = o.Name,
                EntityId = o.EntityId,
              //  EntityName = o.EntityMaster.Name,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();

            return customer;
        }

        public async Task<CustomerDTO> GetCustomer(Guid Id)
        {
            var customer = _db.CustomerMasters.Select(o => new CustomerDTO
            {
                Id = o.Id,
                Name = o.Name,
                EntityId = o.EntityId,
              //  EntityName = o.EntityMaster.Name,
                StatusCode = o.StatusCode
            }).SingleOrDefaultAsync(o => o.Id == Id);

            return await customer;
        }

        public async Task<CustomerDetailDTO> GetCustomerDetail(Guid Id)
        {
            var customer = _db.CustomerMasters.Select(o => new CustomerDetailDTO
            {
                Id = o.Id,
                Name = o.Name,
                EntityId = o.EntityId,
              //  EntityName = o.EntityMaster.Name,
                StatusCode = o.StatusCode
            }).SingleOrDefaultAsync(o => o.Id == Id);

            return await customer;
        }
    }
}