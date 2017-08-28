using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.PhoneType.DTO;
using EMP.Core.Utilities;

namespace EMPAdmin.Transactions.PhoneType
{
    public class PhoneTypeService : IPhoneTypeService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<PhoneTypeDTO> GetAll()
        {
            db = new DatabaseEntities();
            var data = db.PhoneTypeMasters.Select(o => new PhoneTypeDTO
            {
                Id = o.Id,
                PhoneType = o.PhoneType,
                Description = o.Description,
                ActivatedDate = o.ActivatedDate,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();
            return data.OrderBy(o => o.PhoneType);
        }

        public async Task<PhoneTypeDTO> GetById(Guid Id)
        {
            var data = await db.PhoneTypeMasters.Where(o => o.Id == Id).Select(o => new PhoneTypeDTO
            {
                Id = o.Id,
                PhoneType = o.PhoneType,
                Description = o.Description,
                ActivatedDate = o.ActivatedDate,
                StatusCode = o.StatusCode
            }).FirstOrDefaultAsync();

            return data;
        }

        public async Task<int> Save(PhoneTypeDTO _Dto, Guid Id, int EntityState)
        {
            PhoneTypeMaster _phoneTypeMaster = new PhoneTypeMaster();

            if (_Dto != null)
            {
                _phoneTypeMaster.Id = Id;
                _phoneTypeMaster.PhoneType = _Dto.PhoneType;
                _phoneTypeMaster.Description = _Dto.Description;
                _phoneTypeMaster.ActivatedDate = DateTime.Now;
                _phoneTypeMaster.StatusCode = EMPConstants.Active;
            }

            if (_Dto.Id != null)
            {
                _phoneTypeMaster.CreatedBy = _Dto.UserId;
                _phoneTypeMaster.CreatedDate = DateTime.Now;
                _phoneTypeMaster.LastUpdatedBy = _Dto.UserId;
                _phoneTypeMaster.LastUpdatedDate = DateTime.Now;
            }
            else
            {
                _phoneTypeMaster.LastUpdatedBy = _Dto.UserId;
                _phoneTypeMaster.LastUpdatedDate = DateTime.Now;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                var ExistPhone = db.PhoneTypeMasters.Where(o => o.Id != Id && o.PhoneType == _Dto.PhoneType).Any();
                if (ExistPhone)
                    return -1;

                db.Entry(_phoneTypeMaster).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                var ExistPhone = db.PhoneTypeMasters.Where(o => o.PhoneType == _Dto.PhoneType).Any();
                if (ExistPhone)
                    return -1;
                db.PhoneTypeMasters.Add(_phoneTypeMaster);
            }

            try
            {
                await db.SaveChangesAsync();
                return 1;
            }

            catch (DbUpdateConcurrencyException)
            {
                return 0;
            }
        }

        public async Task<Guid> SaveStatus(PhoneTypeDTO _Dto, Guid Id, int EntityState)
        {
            PhoneTypeMaster PhoneType = new PhoneTypeMaster();
            PhoneType = await db.PhoneTypeMasters.Where(o => o.Id == Id).FirstOrDefaultAsync();
            PhoneType.ActivatedDate = DateTime.Now;

            if (PhoneType.StatusCode == EMPConstants.InActive)
            {
                PhoneType.StatusCode = EMPConstants.Active;
            }
            else if (PhoneType.StatusCode == EMPConstants.Active)
            {
                PhoneType.StatusCode = EMPConstants.InActive;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                PhoneType.LastUpdatedDate = DateTime.Now;
                PhoneType.LastUpdatedBy = _Dto.UserId;
                db.Entry(PhoneType).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                await db.SaveChangesAsync();
                db.Dispose();
                return PhoneType.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(PhoneType.Id))
                {
                    return Guid.Empty;
                }
                else
                {
                    throw;
                }
            }
        }

        private bool IsExists(Guid id)
        {
            return db.PhoneTypeMasters.Count(e => e.Id == id) > 0;
        }

        public async Task<bool> Delete(Guid Id)
        {
            try
            {
                var deletedata = db.PhoneTypeMasters.Where(o => o.Id == Id).FirstOrDefault();
                if (deletedata != null)
                {
                    db.PhoneTypeMasters.Remove(deletedata);
                    await db.SaveChangesAsync();
                    db.Dispose();
                }
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

    }
}
