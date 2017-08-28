using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.ContactPersonTitle.DTO;
using EMP.Core.Utilities;
namespace EMPAdmin.Transactions.ContactPersonTitle
{
    public class ContactPersonTitleService : IContactPersonTitleService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<ContactPersonTitleDTO> GetAll()
        {
            db = new DatabaseEntities();
            var data = db.ContactPersonTitleMasters.Select(o => new ContactPersonTitleDTO
            {
                Id = o.Id,
                ContactPersonTitle = o.ContactPersonTitle,
                Description = o.Description,
                ActivatedDate = o.ActivatedDate,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();
            return data.OrderBy(o => o.ContactPersonTitle);
        }

        public async Task<ContactPersonTitleDTO> GetById(Guid Id)
        {
            var data = await db.ContactPersonTitleMasters.Where(o => o.Id == Id).Select(o => new ContactPersonTitleDTO
            {
                Id = o.Id,
                ContactPersonTitle = o.ContactPersonTitle,
                Description = o.Description,
                ActivatedDate = o.ActivatedDate,
                StatusCode = o.StatusCode
            }).FirstOrDefaultAsync();

            return data;
        }

        public async Task<int> Save(ContactPersonTitleDTO _Dto, Guid Id, int EntityState)
        {
            ContactPersonTitleMaster _contactPersonTitle = new ContactPersonTitleMaster();

            if (_Dto != null)
            {
                _contactPersonTitle.Id = Id;
                _contactPersonTitle.ContactPersonTitle = _Dto.ContactPersonTitle;
                _contactPersonTitle.Description = _Dto.Description;
                _contactPersonTitle.ActivatedDate = DateTime.Now;
                _contactPersonTitle.StatusCode = EMPConstants.Active;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                var ExistContact = db.ContactPersonTitleMasters.Where(o => o.Id != Id && o.ContactPersonTitle == _Dto.ContactPersonTitle).Any();
                if (ExistContact)
                    return -1;
                _contactPersonTitle.CreatedBy = _Dto.UserId;
                _contactPersonTitle.CreatedDate = DateTime.Now;
                _contactPersonTitle.LastUpdatedBy = _Dto.UserId;
                _contactPersonTitle.LastUpdatedDate = DateTime.Now;

                db.Entry(_contactPersonTitle).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                var ExistContact = db.ContactPersonTitleMasters.Where(o => o.ContactPersonTitle == _Dto.ContactPersonTitle).Any();
                if (ExistContact)
                    return -1;
                _contactPersonTitle.LastUpdatedBy = _Dto.UserId;
                _contactPersonTitle.LastUpdatedDate = DateTime.Now;
                db.ContactPersonTitleMasters.Add(_contactPersonTitle);
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

        public async Task<Guid> SaveStatus(ContactPersonTitleDTO _Dto, Guid Id, int EntityState)
        {
            ContactPersonTitleMaster ContactPersonTitle = new ContactPersonTitleMaster();
            ContactPersonTitle = await db.ContactPersonTitleMasters.Where(o => o.Id == Id).FirstOrDefaultAsync();
            ContactPersonTitle.ActivatedDate = DateTime.Now;

            if (ContactPersonTitle.StatusCode == EMPConstants.InActive)
            {
                ContactPersonTitle.StatusCode = EMPConstants.Active;
            }
            else if (ContactPersonTitle.StatusCode == EMPConstants.Active)
            {
                ContactPersonTitle.StatusCode = EMPConstants.InActive;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                ContactPersonTitle.LastUpdatedDate = DateTime.Now;
                ContactPersonTitle.LastUpdatedBy = _Dto.UserId;
                db.Entry(ContactPersonTitle).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                await db.SaveChangesAsync();
                db.Dispose();
                return ContactPersonTitle.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(ContactPersonTitle.Id))
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
            return db.ContactPersonTitleMasters.Count(e => e.Id == id) > 0;
        }

        public async Task<bool> Delete(Guid Id)
        {
            try
            {
                var deletedata = db.ContactPersonTitleMasters.Where(o => o.Id == Id).FirstOrDefault();
                if (deletedata != null)
                {
                    db.ContactPersonTitleMasters.Remove(deletedata);
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
