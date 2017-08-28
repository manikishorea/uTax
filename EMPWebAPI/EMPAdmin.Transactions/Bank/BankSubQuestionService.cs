using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.Bank.DTO;
using EMP.Core.Utilities;

namespace EMPAdmin.Transactions.Bank
{
    public class BankSubQuestionService : IBankSubQuestionService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<BankSubQuestionDTO> GetAll()
        {
            db = new DatabaseEntities();
            var data = db.BankSubQuestions.Select(o => new BankSubQuestionDTO
            {
                Id = o.Id,
                BankId = o.BankId,
                BankName = o.BankMaster.BankName,
                Questions = o.Questions,
                Description = o.Description,
                ActivatedDate = o.ActivatedDate,
                DeActivatedDate = o.DeActivatedDate,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();
            return data;
        }

        public async Task<BankSubQuestionDTO> GetById(Guid Id)
        {
            var data = await db.BankSubQuestions.Where(o => o.Id == Id).Select(o => new BankSubQuestionDTO
            {
                Id = o.Id,
                BankId = o.BankId,
                BankName=o.BankMaster.BankName,
                Questions = o.Questions,
                Description = o.Description,
                ActivatedDate = o.ActivatedDate,
                DeActivatedDate = o.DeActivatedDate,
                StatusCode = o.StatusCode
            }).FirstOrDefaultAsync();

            return data;
        }

        public async Task<Guid> Save(BankSubQuestionDTO _Dto, Guid Id, int EntityState)
        {
            BankSubQuestion _BankSubQuestion = new BankSubQuestion();

            if (_Dto != null)
            {
                _BankSubQuestion.Id = Id;
                _BankSubQuestion.BankId = _Dto.BankId;
                _BankSubQuestion.Questions = _Dto.Questions;
                _BankSubQuestion.Description = _Dto.Description;
                _BankSubQuestion.ActivatedDate = DateTime.Now;
                _BankSubQuestion.DeActivatedDate = DateTime.Now;
                _BankSubQuestion.StatusCode = EMPConstants.Active;
            }

            if (_Dto.Id != null)
            {
                _BankSubQuestion.CreatedBy = _Dto.UserId;
                _BankSubQuestion.CreatedDate = DateTime.Now;
                _BankSubQuestion.LastUpdatedBy = _Dto.UserId;
                _BankSubQuestion.LastUpdatedDate = DateTime.Now;
            }
            else
            {
                _BankSubQuestion.LastUpdatedBy = _Dto.UserId;
                _BankSubQuestion.LastUpdatedDate = DateTime.Now;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                db.Entry(_BankSubQuestion).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.BankSubQuestions.Add(_BankSubQuestion);
            }

            try
            {
                await db.SaveChangesAsync();
                return _BankSubQuestion.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(_BankSubQuestion.Id))
                {
                    return _BankSubQuestion.Id;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Guid> SaveStatus(BankSubQuestionDTO _Dto, Guid Id, int EntityState)
        {
            BankSubQuestion bankSubQuestion = new BankSubQuestion();
            bankSubQuestion = await db.BankSubQuestions.Where(o => o.Id == Id).FirstOrDefaultAsync();

            if (bankSubQuestion.StatusCode == EMPConstants.InActive)
            {
                bankSubQuestion.StatusCode = EMPConstants.Active;
            }
            else if (bankSubQuestion.StatusCode == EMPConstants.Active)
            {
                bankSubQuestion.StatusCode = EMPConstants.InActive;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                bankSubQuestion.LastUpdatedDate = DateTime.Now;
                bankSubQuestion.LastUpdatedBy = _Dto.UserId;
                db.Entry(bankSubQuestion).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                await db.SaveChangesAsync();
                db.Dispose();
                return bankSubQuestion.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(bankSubQuestion.Id))
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
            return db.APIIntegrations.Count(e => e.Id == id) > 0;
        }

        public async Task<bool> Delete(Guid Id)
        {
            try
            {
                var deletedata = db.APIIntegrations.Where(o => o.Id == Id).FirstOrDefault();
                if (deletedata != null)
                {
                    db.APIIntegrations.Remove(deletedata);
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
