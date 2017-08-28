using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.Fees;
using EMPAdmin.Transactions.Fees.DTO;
using EMPAdmin.Transactions.Entity.DTO;
using EMP.Core.Utilities;
using System.IO;

namespace EMPAdmin.Transactions.Fees
{
    public class FeesService : IFeesService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<FeesDTO> GetAll()
        {
            db = new DatabaseEntities();
            var data = db.FeeMasters.Select(o => new FeesDTO
            {
                Id = o.Id,
                Name = o.Name,
                FeeType = o.FeeType,
                FeeNature = o.FeeNature,
                ActivatedDate = o.ActivatedDate,
                DeActivatedDate = o.DeActivatedDate,
                StatusCode = o.StatusCode,
                FeeCategoryID = o.FeeCategoryID,
                IsIncludedMaxAmtCalculation = o.IsIncludedMaxAmtCalculation,
                SalesforceFeesFieldID = o.SalesforceFeesFieldID,
                FeesFor = o.FeesFor.ToString(),
                FeesForName = o.FeesFor == 1 ? EMPConstants.FeesFor.Others.ToString() : o.FeesFor == 2 ? EMPConstants.FeesFor.SVBFees.ToString() : EMPConstants.FeesFor.TransmissionFees.ToString()
            }).DefaultIfEmpty();
            return data.OrderBy(o=>o.Name);
        }

        public async Task<FeesDTO> GetById(Guid Id)
        {

            var data = await db.FeeMasters.Where(o => o.Id == Id).Select(o => new FeesDTO
            {
                Id = o.Id,
                Name = o.Name,
                FeeType = o.FeeType,
                FeeTypeId = o.FeeTypeId,
                Amount = o.Amount,
                FeeNature = o.FeeNature,
                FeeNatureId = o.FeeNatureId,
                NoteForUser = o.NoteForUser,
                Note = o.Note,
                StatusCode = o.StatusCode,
                FeeCategoryID = o.FeeCategoryID,
                IsIncludedMaxAmtCalculation = o.IsIncludedMaxAmtCalculation,
                SalesforceFeesFieldID = o.SalesforceFeesFieldID,
                FeesFor = o.FeesFor.ToString(),
                FeesForName = o.FeesFor == 1 ? EMPConstants.FeesFor.Others.ToString() : o.FeesFor == 2 ? EMPConstants.FeesFor.SVBFees.ToString() : EMPConstants.FeesFor.TransmissionFees.ToString(),
                Entities = o.FeeEntityMaps.Select(s => new EntityDTO()
                {
                    Id = s.EntityId ??0
                }).ToList()

            }).FirstOrDefaultAsync();

            return data;
        }

        private bool IsExists(Guid id)
        {
            return db.FeeMasters.Count(e => e.Id == id) > 0;
        }

        public async Task<int> Save(FeesDTO _Dto, Guid Id, int EntityState)
        {
            FeeMaster _FeeMaster = new FeeMaster();

            if (_Dto != null)
            {
                _FeeMaster.Id = Id;
                _FeeMaster.Name = _Dto.Name;
                _FeeMaster.FeeType = _Dto.FeeType;
                _FeeMaster.FeeCategoryID = _Dto.FeeCategoryID;
                _FeeMaster.IsIncludedMaxAmtCalculation = _Dto.IsIncludedMaxAmtCalculation;
                _FeeMaster.SalesforceFeesFieldID = _Dto.SalesforceFeesFieldID;
                if (_FeeMaster.FeeType == EMPConstants.Fixedamount)
                {
                    _FeeMaster.FeeTypeId = 1;
                }
                if (_FeeMaster.FeeType == EMPConstants.Useramount)
                {
                    _FeeMaster.FeeTypeId = 2;
                }
                if (_FeeMaster.FeeType == EMPConstants.SalesForce)
                {
                    _FeeMaster.FeeTypeId = 3;
                }
                _FeeMaster.Amount = _Dto.Amount;
                _FeeMaster.FeeNature = _Dto.FeeNature;
                if (_FeeMaster.FeeNature == EMPConstants.Mandatory)
                {
                    _FeeMaster.FeeNatureId = 1;
                }
                if (_FeeMaster.FeeNature == EMPConstants.Optional)
                {
                    _FeeMaster.FeeNatureId = 2;
                }
                _FeeMaster.ActivatedDate = DateTime.Now;
                _FeeMaster.DeActivatedDate = null;// _Dto.DeActivatedDate; 
                _FeeMaster.NoteForUser = _Dto.NoteForUser;
                _FeeMaster.Note = _Dto.Note;

                _FeeMaster.StatusCode = EMPConstants.Active;
                int FeesFor = 0;
                if (int.TryParse(_Dto.FeesFor, out FeesFor))
                {
                    _FeeMaster.FeesFor = FeesFor;
                }
                else
                {
                    return 0;
                }
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                var ExistFee = db.FeeMasters.Where(o => o.Id != Id && o.Name == _Dto.Name).Any();
                if (ExistFee)
                    return -1;

                _FeeMaster.LastUpdatedDate = DateTime.Now;
                _FeeMaster.LastUpdatedBy = _Dto.UserId;
                db.Entry(_FeeMaster).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                var ExistFee = db.FeeMasters.Where(o => o.Name == _Dto.Name).Any();
                if (ExistFee)
                    return -1;
                _FeeMaster.CreatedBy = _Dto.UserId;
                _FeeMaster.CreatedDate = DateTime.Now;
                _FeeMaster.LastUpdatedDate = DateTime.Now;
                _FeeMaster.LastUpdatedBy = _Dto.UserId;
                db.FeeMasters.Add(_FeeMaster);
            }


            if (_Dto.Entities.ToList().Count > 0)
            {
                if (EntityState == (int)System.Data.Entity.EntityState.Modified)
                {
                    var MapData = db.FeeEntityMaps.Where(o => o.FeeId == Id).ToList();

                    if (MapData.Count() > 0)
                        db.FeeEntityMaps.RemoveRange(MapData);
                }

                List<FeeEntityMap> _FeeEntityMapList = new List<FeeEntityMap>();
                foreach (EntityDTO item in _Dto.Entities)
                {
                    FeeEntityMap _FeeEntityMap = new FeeEntityMap();
                    _FeeEntityMap.Id = Guid.NewGuid();
                    _FeeEntityMap.FeeId = _FeeMaster.Id;
                    _FeeEntityMap.EntityId = item.Id;
                    _FeeEntityMapList.Add(_FeeEntityMap);
                }

                db.FeeEntityMaps.AddRange(_FeeEntityMapList);
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

        public async Task<Guid> SaveStatus(FeesDTO _Dto, Guid Id, int EntityState)
        {
            FeeMaster _FeeMaster = new FeeMaster();
            _FeeMaster = await db.FeeMasters.Where(o => o.Id == Id).FirstOrDefaultAsync();

            if (_FeeMaster.StatusCode == EMPConstants.InActive)
            {
                _FeeMaster.StatusCode = EMPConstants.Active;
                _FeeMaster.DeActivatedDate = null;
                _FeeMaster.ActivatedDate = DateTime.Now;
            }
            else if (_FeeMaster.StatusCode == EMPConstants.Active)
            {
                _FeeMaster.StatusCode = EMPConstants.InActive;
                _FeeMaster.DeActivatedDate = DateTime.Now;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                _FeeMaster.LastUpdatedDate = DateTime.Now;
                _FeeMaster.LastUpdatedBy = _Dto.UserId;
                db.Entry(_FeeMaster).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                await db.SaveChangesAsync();
                db.Dispose();
                return _FeeMaster.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(_FeeMaster.Id))
                {
                    return Guid.Empty;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> Delete(Guid Id)
        {
            try
            {
                var deletedata = db.AffiliateProgramMasters.Where(o => o.Id == Id).FirstOrDefault();
                if (deletedata != null)
                {
                    db.AffiliateProgramMasters.Remove(deletedata);
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
