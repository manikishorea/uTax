using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPPortal.Transactions.ContactPersonTitle.DTO;
using EMP.Core.Utilities;
using EMPPortal.Core.Utilities;

namespace EMPPortal.Transactions.ContactPersonTitle
{
    public class ContactPersonTitleService : IContactPersonTitleService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<ContactPersonTitleDTO> GetAll()
        {
            try
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
                return data;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ContactPersonTitleMaster/GetContactPersonTitleMaster", Guid.Empty);
                return new List<ContactPersonTitleDTO>().AsQueryable();
            }
        }

        public async Task<ContactPersonTitleDTO> GetById(Guid Id)
        {
            try
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
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ContactPersonTitleMaster/GetContactPersonTitleMaster", Id);
                return null;
            }
        }

        public async Task<Guid> Save(ContactPersonTitleDTO _Dto, Guid Id, int EntityState)
        {
            try
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
                    _contactPersonTitle.CreatedBy = _Dto.UserId;
                    _contactPersonTitle.CreatedDate = DateTime.Now;
                    _contactPersonTitle.LastUpdatedBy = _Dto.UserId;
                    _contactPersonTitle.LastUpdatedDate = DateTime.Now;

                    db.Entry(_contactPersonTitle).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    _contactPersonTitle.LastUpdatedBy = _Dto.UserId;
                    _contactPersonTitle.LastUpdatedDate = DateTime.Now;
                    db.ContactPersonTitleMasters.Add(_contactPersonTitle);
                }

                try
                {
                    await db.SaveChangesAsync();
                    return _contactPersonTitle.Id;
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!IsExists(_contactPersonTitle.Id))
                    {
                        return _contactPersonTitle.Id;
                    }
                    else
                    {
                        return Guid.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ContactPersonTitleMaster/PostContactPersonTitleMaster", Id);
                return Guid.Empty;
            }
        }

        public async Task<Guid> SaveStatus(ContactPersonTitleDTO _Dto, Guid Id, int EntityState)
        {
            try
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
                        return Guid.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ContactPersonTitleMaster/PutUserMaster", Id);
                return Guid.Empty;
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
            catch (DbUpdateConcurrencyException ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ContactPersonTitleMaster/DeleteContactPersonTitleMaster", Id);
                return false;
            }
            catch(Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ContactPersonTitleMaster/DeleteContactPersonTitleMaster", Id);
                return false;
            }
        }
    }
}
