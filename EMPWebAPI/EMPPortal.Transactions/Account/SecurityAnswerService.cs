using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.Account.Model;
using EMP.Core.Utilities;
using EMPEntityFramework.Edmx;
using EMPPortal.Core.Utilities;

namespace EMPPortal.Transactions.Account
{
    public class SecurityAnswerService : ISecurityAnswerService
    {
        public DatabaseEntities db = new DatabaseEntities();

        //public IQueryable<SecurityQuestionAnswerModel> GetAll()
        //{
        //    db = new DatabaseEntities();
        //    var data = db.SecurityAnswerUserMaps.Select(o => new SecurityQuestionAnswerModel
        //    {
        //        Id = o.Id,
        //        QuestionId = o.QuestionId,
        //        Answer = o.Answer,
        //        StatusCode = o.StatusCode
        //    }).DefaultIfEmpty();
        //    return data;
        //}

        public IQueryable<SecurityQuestionAnswerModel> GetAll()
        {
            try
            {
                db = new DatabaseEntities();
                var data = db.SecurityQuestionMasters.OrderBy(o => o.DisplayOrder).Select(o => new SecurityQuestionAnswerModel
                {
                    Id = o.Id,
                    Question = o.Question,
                }).DefaultIfEmpty();
                return data;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "SecurityAnswer/GetAllSecurityAnswer", Guid.Empty);
                return null;
            }
        }

        public IQueryable<SecurityQuestionAnswerModel> GetByUser(Guid UserId)
        {
            try
            {
                var data = db.SecurityAnswerUserMaps.Where(o => o.UserId == UserId).Select(o => new SecurityQuestionAnswerModel
                {
                    Id = o.Id,
                    QuestionId = o.QuestionId,
                    Answer = o.Answer,
                    StatusCode = o.StatusCode,
                    DisplayOrder = o.DisplayOrder ?? 0,
                }).DefaultIfEmpty();

                return data;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "SecurityAnswer/GetSecurityAnswers", Guid.Empty);
                return null;
            }
        }

        public bool Save(userSecurityAnswerModel _Dto)
        {
            try
            {
                List<SecurityAnswerUserMap> _SecurityAnswers = new List<SecurityAnswerUserMap>();

                List<SecurityQuestionAnswerModel> _userSecurityAnswerModel = new List<SecurityQuestionAnswerModel>();

                if (_Dto.Status == "2")
                {
                    var result = db.SecurityAnswerUserMaps.Where(o => o.UserId == _Dto.Id).ToList();
                    if (result.Count() > 0)
                        db.SecurityAnswerUserMaps.RemoveRange(result);
                }

                foreach (var item in _Dto.QuestionsLst)
                {
                    SecurityAnswerUserMap _SecurityAnswer = new SecurityAnswerUserMap();
                    _SecurityAnswer.Id = Guid.NewGuid();
                    _SecurityAnswer.QuestionId = item.QuestionId;
                    _SecurityAnswer.Answer = item.Answer;
                    _SecurityAnswer.DisplayOrder = item.DisplayOrder;
                    _SecurityAnswer.UserId = _Dto.Id;
                    _SecurityAnswer.CreatedBy = _Dto.Id;
                    _SecurityAnswer.LastUpdatedBy = _Dto.Id;
                    _SecurityAnswer.LastUpdatedDate = DateTime.Now;
                    _SecurityAnswer.CreatedDate = DateTime.Now;
                    _SecurityAnswer.StatusCode = EMPConstants.Active;
                    _SecurityAnswers.Add(_SecurityAnswer);
                }

                db.SecurityAnswerUserMaps.AddRange(_SecurityAnswers);

                try
                {
                    db.SaveChanges();
                    db.Dispose();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ExceptionLogger.LogException(ex.ToString(), "SecurityAnswer/PostSecurityQuestion", Guid.Empty);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "SecurityAnswer/PostSecurityQuestion", Guid.Empty);
                return false;
            }
        }

        //public async Task<Guid> SaveStatus(SecurityQuestionAnswerModel _Dto, Guid Id, int EntityState)
        //{
        //    SecurityAnswerUserMap SecurityAnswer = new SecurityAnswerUserMap();
        //    SecurityAnswer = await db.SecurityAnswerUserMaps.Where(o => o.Id == Id).FirstOrDefaultAsync();
        //    //securityQuestionMaster.ActivatedDate = DateTime.Now;

        //    if (SecurityAnswer.StatusCode == EMPConstants.InActive)
        //    {
        //        SecurityAnswer.StatusCode = EMPConstants.Active;
        //    }
        //    else if (SecurityAnswer.StatusCode == EMPConstants.Active)
        //    {
        //        SecurityAnswer.StatusCode = EMPConstants.InActive;
        //    }

        //    if (EntityState == (int)System.Data.Entity.EntityState.Modified)
        //    {
        //        SecurityAnswer.LastUpdatedDate = DateTime.Now;
        //        SecurityAnswer.LastUpdatedBy = _Dto.UserId;
        //        db.Entry(SecurityAnswer).State = System.Data.Entity.EntityState.Modified;
        //    }

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //        db.Dispose();
        //        return SecurityAnswer.Id;
        //    }

        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!IsExists(SecurityAnswer.Id))
        //        {
        //            return Guid.Empty;
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        private bool IsExists(Guid id)
        {
            return db.SecurityQuestionMasters.Count(e => e.Id == id) > 0;
        }
    }
}
