using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using System.Threading.Tasks;
using EMPEntityFramework.Edmx;

using System.Data.Entity;
using EMPAdmin.Transactions.Entity.DTO;

namespace EMPAdmin.Transactions.Entity
{
    public class EntityService : IEntityService
    {
        // private readonly DatabaseEntities _db = new DatabaseEntities();
        //  private readonly EntityDTO _entity = new EntityDTO();

        //public EntityService(DatabaseEntities db, EntityDTO entity)
        //{
        //    _db = db;
        //    _entity = entity;
        //}

        public DatabaseEntities _db = new DatabaseEntities();
        public EntityDTO _entity = new EntityDTO();

        public IQueryable<EntityDTO> GetAllEntity()
        {
            var entity = _db.EntityMasters.Where(a => a.Name != "uTax").OrderBy(o => o.DisplayOrder).Select(o => new EntityDTO
            {
                Id = o.Id,
                Name = o.Name,
                Description = o.Description,
                ParentId = o.ParentId ?? 0,
                ParentName = o.ParentId != null ? _db.EntityMasters.Where(s => s.ParentId == o.ParentId).FirstOrDefault().Name : ""
            }).DefaultIfEmpty();

            return entity;
        }

        public async Task<EntityDTO> GetEntity(int Id)
        {
            var Entity = _db.EntityMasters.Select(o => new EntityDTO
            {
                Id = o.Id,
                Name = o.Name,
                Description = o.Description,
                ParentId = o.ParentId ?? 0,
                ParentName = o.ParentId != null ? _db.EntityMasters.Where(s => s.ParentId == o.ParentId).FirstOrDefault().Name : ""
            }).SingleOrDefaultAsync(o => o.Id == Id);

            return await Entity;
        }

        public async Task<EntityDetailDTO> GetEntityDetail(int Id)
        {
            var Entity = _db.EntityMasters.Select(o => new EntityDetailDTO
            {
                Id = o.Id,
                Name = o.Name,
                Description = o.Description,
                ParentId = o.ParentId ??0,
                ParentName = o.ParentId != null ? _db.EntityMasters.Where(s => s.ParentId == o.ParentId).FirstOrDefault().Name : ""
            }).SingleOrDefaultAsync(o => o.Id == Id);

            return await Entity;
        }
    }
}