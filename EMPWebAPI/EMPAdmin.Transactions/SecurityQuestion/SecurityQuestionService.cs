using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.SecurityQuestion.DTO;
using EMP.Core.Utilities;

namespace EMPAdmin.Transactions.SecurityQuestion
{
    public class SecurityQuestionService : ISecurityQuestionService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<SecurityQuestionDTO> GetAll()
        {
            db = new DatabaseEntities();
            var data = db.SecurityQuestionMasters.Select(o => new SecurityQuestionDTO
            {
                Id = o.Id,
                Question = o.Question,
                Description = o.Description,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();
            return data;
        }

        public async Task<SecurityQuestionDTO> GetById(Guid Id)
        {
            var data = await db.SecurityQuestionMasters.Where(o => o.Id == Id).Select(o => new SecurityQuestionDTO
            {
                Id = o.Id,
                Question = o.Question,
                Description = o.Description,
                StatusCode = o.StatusCode
            }).FirstOrDefaultAsync();

            return data;
        }

        public async Task<Guid> Save(SecurityQuestionDTO _Dto, Guid Id, int EntityState)
        {
            SecurityQuestionMaster _SecurityQuestionMaster = new SecurityQuestionMaster();

            if (_Dto != null)
            {
                _SecurityQuestionMaster.Id = Id;
                _SecurityQuestionMaster.Question = _Dto.Question;
                _SecurityQuestionMaster.Description = _Dto.Description;
                _SecurityQuestionMaster.StatusCode = EMPConstants.Active;
            }

            if (_Dto.Id != null)
            {
                _SecurityQuestionMaster.CreatedBy = _Dto.UserId;
                _SecurityQuestionMaster.CreatedDate = DateTime.Now;
                _SecurityQuestionMaster.LastUpdatedBy = _Dto.UserId;
                _SecurityQuestionMaster.LastUpdatedDate = DateTime.Now;
            }
            else
            {
                _SecurityQuestionMaster.LastUpdatedBy = _Dto.UserId;
                _SecurityQuestionMaster.LastUpdatedDate = DateTime.Now;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                db.Entry(_SecurityQuestionMaster).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.SecurityQuestionMasters.Add(_SecurityQuestionMaster);
            }

            try
            {
                await db.SaveChangesAsync();
                return _SecurityQuestionMaster.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(_SecurityQuestionMaster.Id))
                {
                    return _SecurityQuestionMaster.Id;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Guid> SaveStatus(SecurityQuestionDTO _Dto, Guid Id, int EntityState)
        {
            SecurityQuestionMaster SecurityQuestion = new SecurityQuestionMaster();
            SecurityQuestion = await db.SecurityQuestionMasters.Where(o => o.Id == Id).FirstOrDefaultAsync();

            if (SecurityQuestion.StatusCode == EMPConstants.InActive)
            {
                SecurityQuestion.StatusCode = EMPConstants.Active;
            }
            else if (SecurityQuestion.StatusCode == EMPConstants.Active)
            {
                SecurityQuestion.StatusCode = EMPConstants.InActive;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                SecurityQuestion.LastUpdatedDate = DateTime.Now;
                SecurityQuestion.LastUpdatedBy = _Dto.UserId;
                db.Entry(SecurityQuestion).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                await db.SaveChangesAsync();
                db.Dispose();
                return SecurityQuestion.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(SecurityQuestion.Id))
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
            return db.SecurityQuestionMasters.Count(e => e.Id == id) > 0;
        }

        public async Task<bool> Delete(Guid Id)
        {
            try
            {
                var deletedata = db.SecurityQuestionMasters.Where(o => o.Id == Id).FirstOrDefault();
                if (deletedata != null)
                {
                    db.SecurityQuestionMasters.Remove(deletedata);
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
