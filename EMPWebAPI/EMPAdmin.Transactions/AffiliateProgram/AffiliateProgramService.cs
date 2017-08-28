using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.AffiliateProgram.DTO;
using EMPAdmin.Transactions.Entity.DTO;
using EMP.Core.Utilities;
using System.IO;

namespace EMPAdmin.Transactions.AffiliateProgram
{
    public class AffiliateProgramService : IAffiliateProgramService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<AffiliateProgramDTO> GetAll()
        {
            db = new DatabaseEntities();
            var data = db.AffiliateProgramMasters.Select(o => new AffiliateProgramDTO
            {
                Id = o.Id,
                Name = o.Name,
                Cost = o.Cost,
                ActivationDate = o.ActivationDate,
                DeactivationDate = o.DeactivationDate,
                DocumentPath = o.DocumentPath,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();
            return data.OrderBy(a => a.Name);
        }

        public async Task<AffiliateProgramDTO> GetById(Guid Id)
        {
            var data = await db.AffiliateProgramMasters.Where(o => o.Id == Id).Select(o => new AffiliateProgramDTO
            {
                Id = o.Id,
                Name = o.Name,
                Cost = o.Cost,
                ActivationDate = o.ActivationDate,
                DeactivationDate = o.DeactivationDate,
                DocumentPath = o.DocumentPath,
                StatusCode = o.StatusCode,
                Entities = o.AffiliationProgramEntityMaps.Select(s => new EntityDTO() { Id = s.EntityId ?? 0, Name = s.EntityMaster.Name }).ToList()
            }).FirstOrDefaultAsync();

            return data;
        }

        public int Save(AffiliateProgramDTO _Dto, Guid Id, int EntityState)
        {
            AffiliateProgramMaster _AffiliateProgramMaster = new AffiliateProgramMaster();

            if (_Dto != null)
            {
                _AffiliateProgramMaster.Id = Id;
                _AffiliateProgramMaster.Name = _Dto.Name;
                _AffiliateProgramMaster.Cost = _Dto.Cost;
                _AffiliateProgramMaster.ActivationDate = DateTime.Now;
                _AffiliateProgramMaster.DeactivationDate = DateTime.Now;
                _AffiliateProgramMaster.DocumentPath = _Dto.DocumentPath;
                _AffiliateProgramMaster.StatusCode = EMPConstants.Active;
            }

            if (_Dto.Id != null)
            {
                _AffiliateProgramMaster.CreatedBy = _Dto.UserId;
                _AffiliateProgramMaster.CreatedDate = DateTime.Now;
                _AffiliateProgramMaster.LastUpdatedBy = _Dto.UserId;
                _AffiliateProgramMaster.LastUpdatedDate = DateTime.Now;
            }
            else
            {
                _AffiliateProgramMaster.LastUpdatedBy = _Dto.UserId;
                _AffiliateProgramMaster.LastUpdatedDate = DateTime.Now;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                var ExistAP = db.AffiliateProgramMasters.Where(o => o.Id != Id && o.Name == _Dto.Name).Any();
                if (ExistAP)
                    return -1;

                db.Entry(_AffiliateProgramMaster).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                var ExistAP = db.AffiliateProgramMasters.Where(o => o.Name == _Dto.Name).Any();
                if (ExistAP)
                    return -1;
                db.AffiliateProgramMasters.Add(_AffiliateProgramMaster);
            }


            if (_Dto.Entities.ToList().Count > 0)
            {
                if (EntityState == (int)System.Data.Entity.EntityState.Modified)
                {
                    var MapData = db.AffiliationProgramEntityMaps.Where(o => o.AffiliateProgramId == Id).ToList();

                    if (MapData.Count() > 0)
                        db.AffiliationProgramEntityMaps.RemoveRange(MapData);
                }

                List<AffiliationProgramEntityMap> _AffiliateEntityMapDTOList = new List<AffiliationProgramEntityMap>();
                foreach (EntityDTO item in _Dto.Entities)
                {
                    AffiliationProgramEntityMap _AffiliateEntityMap = new AffiliationProgramEntityMap();
                    _AffiliateEntityMap.Id = Guid.NewGuid();
                    _AffiliateEntityMap.AffiliateProgramId = _AffiliateProgramMaster.Id;
                    _AffiliateEntityMap.EntityId = item.Id;
                    _AffiliateEntityMapDTOList.Add(_AffiliateEntityMap);
                }
                db.AffiliationProgramEntityMaps.AddRange(_AffiliateEntityMapDTOList);
            }
            try
            {
                db.SaveChanges();
                return 1;
            }
            catch (DbUpdateConcurrencyException)
            {
                return 0;
            }
        }

        public Guid SaveStatus(AffiliateProgramDTO _Dto, Guid Id, int EntityState)
        {
            AffiliateProgramMaster AffiliateProgram = new AffiliateProgramMaster();
            AffiliateProgram = db.AffiliateProgramMasters.Where(o => o.Id == Id).FirstOrDefault();

            if (AffiliateProgram != null)
            {
                AffiliateProgram.DeactivationDate = DateTime.Now;

                if (AffiliateProgram.StatusCode == EMPConstants.InActive)
                {
                    AffiliateProgram.StatusCode = EMPConstants.Active;
                }
                else if (AffiliateProgram.StatusCode == EMPConstants.Active)
                {
                    AffiliateProgram.StatusCode = EMPConstants.InActive;
                }

                if (EntityState == (int)System.Data.Entity.EntityState.Modified)
                {
                    AffiliateProgram.LastUpdatedDate = DateTime.Now;
                    AffiliateProgram.LastUpdatedBy = _Dto.UserId;
                    db.Entry(AffiliateProgram).State = System.Data.Entity.EntityState.Modified;
                }
            }

            try
            {
                db.SaveChanges();
                db.Dispose();
                return AffiliateProgram.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(AffiliateProgram.Id))
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
            return db.AffiliateProgramMasters.Count(e => e.Id == id) > 0;
        }

        public bool Delete(Guid Id)
        {
            try
            {
                var deletedata = db.AffiliateProgramMasters.Where(o => o.Id == Id).FirstOrDefault();
                if (deletedata != null)
                {
                    db.AffiliateProgramMasters.Remove(deletedata);
                    db.SaveChanges();
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
