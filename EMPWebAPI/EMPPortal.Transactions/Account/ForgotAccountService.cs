using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using EMPPortal.Transactions.Account.Model;
using EMPEntityFramework.Edmx;
using EMP.Core.Utilities;
using System.Collections.Generic;
using EMPPortal.Core.Utilities;

namespace EMPPortal.Transactions.Account
{
    public class ForgotAccountService : IForgotPasswordService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<ForgotPasswordModel> GetAll()
        {
            db = new DatabaseEntities();
            var data = db.emp_CustomerLoginInformation.Select(o => new ForgotPasswordModel
            {
                Id = o.Id,
                EMPPassword = o.EMPPassword,
                CustomerOfficeId = o.CustomerOfficeId,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();
            return data;
        }

        public async Task<ForgotPasswordModel> GetById(Guid Id)
        {
            var data = await db.emp_CustomerLoginInformation.Where(o => o.Id == Id).Select(o => new ForgotPasswordModel
            {
                Id = o.Id,
                EMPPassword = o.EMPPassword,
                CustomerOfficeId = o.CustomerOfficeId,
                StatusCode = o.StatusCode
            }).FirstOrDefaultAsync();

            return data;
        }

        public async Task<Guid> PostForgotPassword(ForgotPasswordModel _Dto)
        {
            emp_CustomerLoginInformation _CustomerLoginInformation = new emp_CustomerLoginInformation();
            SecurityAnswerUserMap _SecurityAnswerUserMap = new SecurityAnswerUserMap();

            var dbresult = await db.emp_CustomerLoginInformation.Where(o => o.CrossLinkUserId == _Dto.CrossLinkUserId).Select(o => new CustomerLoginModel
            {
                CrossLinkUserId = o.CrossLinkUserId,
            }).FirstOrDefaultAsync();
            if (dbresult != null)
            {
                var result = await db.SecurityAnswerUserMaps.Where(o => (o.QuestionId == _SecurityAnswerUserMap.QuestionId) && (o.Answer == _SecurityAnswerUserMap.Answer)).Select(o => new SecurityAnswerUserMap
                {
                    UserId = o.UserId,
                }).FirstOrDefaultAsync();
            }
            return dbresult.UserId ?? Guid.Empty;
        }

        public async Task<Guid> SaveStatus(ForgotPasswordModel _Dto, Guid Id, int EntityState)
        {
            emp_CustomerLoginInformation CustomerLoginInformation = new emp_CustomerLoginInformation();
            CustomerLoginInformation = await db.emp_CustomerLoginInformation.Where(o => o.Id == Id).FirstOrDefaultAsync();
            //securityQuestionMaster.ActivatedDate = DateTime.Now;

            if (CustomerLoginInformation.StatusCode == EMPConstants.InActive)
            {
                CustomerLoginInformation.StatusCode = EMPConstants.Active;
            }
            else if (CustomerLoginInformation.StatusCode == EMPConstants.Active)
            {
                CustomerLoginInformation.StatusCode = EMPConstants.InActive;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                CustomerLoginInformation.LastUpdatedDate = DateTime.Now;
                CustomerLoginInformation.LastUpdatedBy = _Dto.UserId;
                db.Entry(CustomerLoginInformation).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                await db.SaveChangesAsync();
                db.Dispose();
                return CustomerLoginInformation.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(CustomerLoginInformation.Id))
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

        public ForgotAccountModel GetByUserId(string UserId)
        {
            var IsCorrect = false;
            ForgotAccountModel ForgotAccountModel = new ForgotAccountModel();
            try
            {
                var data = db.emp_CustomerLoginInformation.Where(o => o.EMPUserId == UserId).FirstOrDefault();

                if (data != null)
                {
                    ForgotAccountModel.Id = data.Id;
                    ForgotAccountModel.SecurityQuestions = db.SecurityAnswerUserMaps.Where(s => s.UserId == data.CustomerOfficeId).OrderBy(a => a.DisplayOrder).Select(s => new SecurityQuestionModel() { Id = s.QuestionId, Question = db.SecurityQuestionMasters.Where(a => a.Id == s.QuestionId).FirstOrDefault().Question }).ToList();
                    ForgotAccountModel.Status = (ForgotAccountModel.SecurityQuestions.ToList().Count > 0) ? 1 : 0;
                    IsCorrect = true;
                }
                else
                {
                    var User = db.UserMasters.Where(o => o.UserName == UserId).FirstOrDefault();
                    if (User != null)
                    {
                        ForgotAccountModel.Id = User.Id;
                        ForgotAccountModel.SecurityQuestions = db.SecurityAnswerUserMaps.Where(s => s.UserId == User.Id).OrderBy(a => a.DisplayOrder).Select(s => new SecurityQuestionModel() { Id = s.QuestionId, Question = db.SecurityQuestionMasters.Where(a => a.Id == s.QuestionId).FirstOrDefault().Question }).ToList();
                        ForgotAccountModel.Status = (ForgotAccountModel.SecurityQuestions.ToList().Count > 0) ? 1 : -2;
                        IsCorrect = true;
                    }
                }

                if (!IsCorrect)
                {
                    ForgotAccountModel.Status = -1;
                }

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ForgotAccount/password", Guid.Empty);
            }
            return ForgotAccountModel;
        }

        public bool GetByUserSecurityAnswer(userSecurityAnswerModel _Dto)
        {
            bool IsSecurityAnsExist = false;
            try
            {
                var data = db.emp_CustomerLoginInformation.Where(o => o.EMPUserId == _Dto.UserName).FirstOrDefault();

                Guid Id = Guid.Empty;
                if (data == null)
                {
                    var User = db.UserMasters.Where(o => o.UserName == _Dto.UserName).FirstOrDefault();
                    if (User != null)
                    {
                        Id = User.Id;
                    }
                }
                else
                {
                    Id = data.CustomerOfficeId ?? Guid.Empty;
                }

                if (Id != Guid.Empty)
                {
                    string Answer1 = "";
                    string Answer2 = "";
                    string Answer3 = "";
                    Guid Question1 = Guid.Empty;
                    Guid Question2 = Guid.Empty;
                    Guid Question3 = Guid.Empty;

                    List<SecurityAnswerUserMap> SecurityAnswerUserMapList = new List<SecurityAnswerUserMap>();
                    SecurityAnswerUserMapList = db.SecurityAnswerUserMaps.Where(o => o.UserId == Id).ToList();

                    int i = 0;
                    int m = 0;
                    foreach (var item in _Dto.QuestionsLst)
                    {
                        if (i == 0)
                        {
                            Question1 = item.QuestionId;
                            Answer1 = item.Answer.ToString();
                            m = m + SecurityAnswerUserMapList.Where(o => o.Answer == Answer1 && o.QuestionId == Question1).ToList().Count;
                        }

                        if (i == 1)
                        {
                            Question2 = item.QuestionId;
                            Answer2 = item.Answer.ToString();
                            m = m + SecurityAnswerUserMapList.Where(o => o.Answer == Answer2 && o.QuestionId == Question2).ToList().Count;
                        }

                        if (i == 2)
                        {
                            Question3 = item.QuestionId;
                            Answer3 = item.Answer.ToString();
                            m = m + SecurityAnswerUserMapList.Where(o => o.Answer == Answer3 && o.QuestionId == Question3).ToList().Count;
                        }

                        i = i + 1;

                    }

                    if (m >= 2)
                    {
                        IsSecurityAnsExist = true;
                    }
                    else
                    {
                        IsSecurityAnsExist = false;
                    }
                }
                else
                {
                    return IsSecurityAnsExist;
                }

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ForgotAccount/GetByUserSecurityAnswer", Guid.Empty);
            }
            return IsSecurityAnsExist;
        }
    }
}
