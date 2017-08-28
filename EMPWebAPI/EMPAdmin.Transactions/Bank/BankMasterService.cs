using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.Bank.DTO;
using EMPAdmin.Transactions.Entity.DTO;
using System.Data.Entity;
using EMP.Core.Utilities;
using System.Data.Entity.Infrastructure;

namespace EMPAdmin.Transactions.Bank
{
    public class BankMasterService
    {
        private DatabaseEntities db = new DatabaseEntities();

        public IQueryable<BankMasterDTO> GetAllBankMaster()
        {
            db = new DatabaseEntities();
            var data = db.BankMasters.Select(o => new BankMasterDTO
            {
                Id = o.Id,
                BankName = o.BankName,
                BankCode = o.BankCode,
                BankServiceFees = o.BankServiceFees,
                MaxFeeLimitDeskTop = o.MaxFeeLimitDeskTop,
                MaxFeeLimitMSO = o.MaxFeeLimitMSO,
                MaxTranFeeDeskTop = o.MaxTranFeeDeskTop,
                MaxTranFeeMSO = o.MaxTranFeeMSO,
                Description = o.Description,
                BankProductDocument = o.BankProductDocument,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();
            return data.OrderBy(a => a.BankName);
        }

        public async Task<BankMasterDTO> GetBankDetailsById(Guid? Id)
        {
            var data = await db.BankMasters.Where(o => o.Id == Id).Select(o => new BankMasterDTO
            {
                BankName = o.BankName,
                BankCode = o.BankCode,
                BankServiceFees = o.BankServiceFees,
                MaxFeeLimitDeskTop = o.MaxFeeLimitDeskTop,
                MaxFeeLimitMSO = o.MaxFeeLimitMSO,
                MaxTranFeeDeskTop = o.MaxTranFeeDeskTop,
                MaxTranFeeMSO = o.MaxTranFeeMSO,
                Id = o.Id,
                Description = o.Description,
                BankProductDocument = o.BankProductDocument,
                Entities = o.BankEntityMaps.Select(s => new EntityDTO() { Id = s.EntityId ?? 0, Name = s.EntityMaster.Name }).ToList(),
                BankSubQuestions = o.BankSubQuestions.Select(s => new BankSubQuestionDTO() { Id = s.Id, Description = s.Description, Questions = s.Questions, ActivatedDate = s.ActivatedDate, DeActivatedDate = s.DeActivatedDate, StatusCode = s.StatusCode }).ToList()
            }).FirstOrDefaultAsync();

            return data;
        }

        private bool BankMasterExists(Guid id)
        {
            return db.BankMasters.Count(e => e.Id == id) > 0;
        }

        public async Task<int> Save(BankMasterDTO _Dto, Guid Id, int EntityState)
        {
            BankMaster bankmaster = new BankMaster();

            if (_Dto != null)
            {
                bankmaster.Id = Id;
                bankmaster.BankName = _Dto.BankName;
                bankmaster.BankServiceFees = _Dto.BankServiceFees;
                bankmaster.MaxFeeLimitDeskTop = _Dto.MaxFeeLimitDeskTop;
                bankmaster.MaxTranFeeDeskTop = _Dto.MaxTranFeeDeskTop;
                bankmaster.MaxFeeLimitMSO = _Dto.MaxFeeLimitMSO;
                bankmaster.MaxTranFeeMSO = _Dto.MaxTranFeeMSO;
                bankmaster.ActivatedDate = _Dto.ActivatedDate;
                bankmaster.BankProductDocument = _Dto.BankProductDocument;
                bankmaster.DeActivatedDate = _Dto.DeActivatedDate;
                bankmaster.Description = _Dto.Description;
                bankmaster.StatusCode = EMPConstants.Active;
                bankmaster.BankCode = _Dto.BankCode;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                var ExistBank = db.BankMasters.Where(o => o.Id != Id && o.BankName == _Dto.BankName).Any();
                if (ExistBank)
                    return -1;

                bankmaster.LastUpdatedDate = DateTime.Now;
                bankmaster.LastUpdatedBy = _Dto.UserId;
                db.Entry(bankmaster).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                var ExistBank = db.BankMasters.Where(o => o.BankName == _Dto.BankName).Any();
                if (ExistBank)
                    return -1;
                bankmaster.CreatedBy = _Dto.UserId;
                bankmaster.CreatedDate = DateTime.Now;
                bankmaster.LastUpdatedDate = DateTime.Now;
                bankmaster.LastUpdatedBy = _Dto.UserId;
                db.BankMasters.Add(bankmaster);
            }


            if (_Dto.Entities.ToList().Count > 0)
            {
                if (EntityState == (int)System.Data.Entity.EntityState.Modified)
                {
                    var MapData = db.BankEntityMaps.Where(o => o.BankId == Id).ToList();

                    if (MapData.Count() > 0)
                        db.BankEntityMaps.RemoveRange(MapData);
                }

                List<BankEntityMap> _BankEntityMapsList = new List<BankEntityMap>();
                foreach (EntityDTO item in _Dto.Entities)
                {
                    BankEntityMap _BankEntityMap = new BankEntityMap();
                    _BankEntityMap.Id = Guid.NewGuid();
                    _BankEntityMap.BankId = bankmaster.Id;
                    _BankEntityMap.EntityId = item.Id;
                    _BankEntityMapsList.Add(_BankEntityMap);
                }

                db.BankEntityMaps.AddRange(_BankEntityMapsList);
            }

            try
            {
                await db.SaveChangesAsync();
                db.Dispose();
                return 1;
            }

            catch (DbUpdateConcurrencyException)
            {
                return 0;
            }
        }

        public async Task<Guid> SaveStatus(BankMasterDTO _Dto, Guid Id, int EntityState)
        {
            BankMaster bankmaster = new BankMaster();
            bankmaster = await db.BankMasters.Where(o => o.Id == Id).FirstOrDefaultAsync();

            if (bankmaster.StatusCode == EMPConstants.InActive)
            {
                bankmaster.StatusCode = EMPConstants.Active;
            }
            else if (bankmaster.StatusCode == EMPConstants.Active)
            {
                bankmaster.StatusCode = EMPConstants.InActive;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                bankmaster.LastUpdatedDate = DateTime.Now;
                bankmaster.LastUpdatedBy = _Dto.UserId;
                db.Entry(bankmaster).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                await db.SaveChangesAsync();
                db.Dispose();
                return bankmaster.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!BankMasterExists(bankmaster.Id))
                {
                    return Guid.Empty;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
