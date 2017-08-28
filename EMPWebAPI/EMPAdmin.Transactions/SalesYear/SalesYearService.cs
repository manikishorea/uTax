//@*--------------------------------------------------------------------------------
// Copyright (c) Kensium. All rights reserved.
// 
//--------------------------------------------------------------------------------
//--------------------------------------------------------------------------------
// Project Name : FSA
// Module Name  : Sales Year  
// Description  : This will contain the Sales Year Transactions details
// Organization : Kensium 
// Author       : Jitendra 
// Created on   : 23 Aug 2016
// Revision History : 24th Aug 2016 : chagne update method 
//--------------------------------------------------------------------------------*@
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.SalesYear;
using EMPAdmin.Transactions.SalesYear.DTO;
using EMPAdmin.Transactions.Entity.DTO;
using EMP.Core.Utilities;
using System.IO;
using EMP.Core.DataMigration;

namespace EMPAdmin.Transactions.SalesYear
{
    public class SalesYearService : ISalesYearService
    {
        public DatabaseEntities db = new DatabaseEntities();

        DataMigrationService _DataMigrationService = new DataMigrationService();


        public IQueryable<SalesYearDTO> GetAll()
        {
            db = new DatabaseEntities();
            var data = db.SalesYearMasters.Select(o => new SalesYearDTO
            {
                Id = o.Id,
                SalesYear = o.SalesYear ?? 0,
                ApplicableFromDate = o.ApplicableFromDate,
                ApplicableToDate = o.ApplicableToDate,
                DateType = o.DateType,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty().OrderByDescending(o=>o.SalesYear);
            return data;
        }

        public async Task<SalesYearDTO> GetById(Guid Id)
        {
            var data = await db.SalesYearMasters.Where(o => o.Id == Id).Select(o => new SalesYearDTO
            {
                Id = o.Id,
                SalesYear = o.SalesYear ?? 0,
                ApplicableFromDate = o.ApplicableFromDate,
                ApplicableToDate = o.ApplicableToDate,
                Description = o.Description,
                StatusCode = o.StatusCode,
                DateType = o.DateType,
                BankInfoList = db.BankAssociatedCutofDates.Where(a => a.SalesYearID == o.Id).Select(a => new BankInfoDTO { BankID = a.BankID, CutOfDate = a.CutofDate ?? DateTime.MinValue }).ToList(),
                Entities = o.SalesYearEntityMaps.Select(s => new EntityDTO()
                {
                    Name = s.EntityMaster.Name,
                }).ToList()

            }).FirstOrDefaultAsync();

            return data;
        }

        public IQueryable<EntityDTO> GetAllEntity()
        {
            List<string> StatusList = new List<string>();
            StatusList.Add(EMPConstants.Active);
            StatusList.Add(EMPConstants.Created);

            List<EntityDTO> EntityDTOlst = new List<EntityDTO>();
            var dbentity = db.EntityMasters.Where(a => a.StatusCode != EMPConstants.InActive).OrderBy(o => o.DisplayOrder);
            foreach (var itm in dbentity)
            {
                EntityDTO oSalesYearDTO = new EntityDTO();
                oSalesYearDTO.Name = itm.Name;
                oSalesYearDTO.EntityCount = db.emp_CustomerInformation.Where(a => a.EntityId == itm.Id && StatusList.Contains(a.StatusCode)).Count();
                EntityDTOlst.Add(oSalesYearDTO);
            }
            return EntityDTOlst.AsQueryable();
        }

        private bool IsExists(Guid id)
        {
            return db.SalesYearMasters.Count(e => e.Id == id) > 0;
        }

        /// <summary>
        /// this method is used for Save & Update Sales Year Details 
        /// </summary>
        /// <param name="_Dto"></param>
        /// <param name="Id"></param>
        /// <param name="EntityState"></param>
        /// <returns></returns>
        public Guid Save(SalesYearDTO _Dto, Guid Id, int EntityState)
        {
            SalesYearMaster _SalesYearMaster = new SalesYearMaster();

            if (_Dto != null)
            {
                if (_Dto.Id == Guid.Empty)
                {
                    var salesYearcheck = db.SalesYearMasters.Where(a => a.SalesYear == _Dto.SalesYear).Any();
                    if (salesYearcheck)
                    {
                        return Guid.Empty;
                    }
                }

                _SalesYearMaster.Id = Id;
                _SalesYearMaster.SalesYear = _Dto.SalesYear;

                _SalesYearMaster.Description = _Dto.Description;
                _SalesYearMaster.DateType = _Dto.DateType;
                //if (_SalesYearMaster.DateType == true)
                if (System.DateTime.Now.Year < _SalesYearMaster.SalesYear)
                {
                    _SalesYearMaster.ApplicableFromDate = _Dto.ApplicableFromDate.Value.AddDays(1);
                    _SalesYearMaster.ApplicableToDate = null;
                }
                else
                {
                    _SalesYearMaster.ApplicableToDate = DateTime.Now;// _Dto.ApplicableFromDate;
                }
                _SalesYearMaster.StatusCode = EMPConstants.Active;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                _SalesYearMaster.LastUpdatedDate = DateTime.Now;
                _SalesYearMaster.LastUpdatedBy = _Dto.UserId;
                db.Entry(_SalesYearMaster).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                bool iretbal = UpdatePreviousYearData(_SalesYearMaster.SalesYear ?? 0);
                if (iretbal)
                {
                    _SalesYearMaster.CreatedBy = _Dto.UserId;
                    _SalesYearMaster.CreatedDate = DateTime.Now;
                    _SalesYearMaster.LastUpdatedDate = DateTime.Now;
                    _SalesYearMaster.LastUpdatedBy = _Dto.UserId;
                    db.SalesYearMasters.Add(_SalesYearMaster);
                }
                else
                {
                    return Guid.Empty;
                }
            }

            try
            {
                db.SaveChanges();
                db.Dispose();

                SaveBankAssociatedDetail(Id, _Dto.BankInfoList, _Dto.UserId ?? Guid.Empty);
                //bool result = _DataMigrationService.SetArchiveData();
                return _SalesYearMaster.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(_SalesYearMaster.Id))
                {
                    return Guid.Empty;
                }
                else
                {
                    throw;
                }
            }
        }

        public bool UpdatePreviousYearData(int SalesYear)
        {
            bool retbool = true;
            if (SalesYear.ToString().Length != 4)
                retbool = false;
            db = new DatabaseEntities();
            var dbExist = db.SalesYearMasters.Where(a => a.SalesYear == SalesYear - 1 && a.ApplicableToDate == null).FirstOrDefault();
            if (dbExist != null)
            {
                if (dbExist.ApplicableFromDate <= System.DateTime.Now)
                {
                    dbExist.ApplicableToDate = System.DateTime.Now;
                    db.SaveChanges();
                    retbool = true;
                }
                else
                    retbool = false;
            }
            return retbool;
        }

        public bool SaveBankAssociatedDetail(Guid SalesYearID, List<BankInfoDTO> BankInfoList, Guid userId)
        {
            try
            {
                db = new DatabaseEntities();
                var dbExistd = db.BankAssociatedCutofDates.Any(a => a.SalesYearID == SalesYearID);
                if (dbExistd)
                {
                    foreach (BankAssociatedCutofDate bac in db.BankAssociatedCutofDates.Where(a => a.SalesYearID == SalesYearID))
                    {
                        bac.IsActive = false;
                        bac.LastUpdatedBy = userId;
                        bac.LastUpdatedDate = System.DateTime.Now;
                    }
                    db.SaveChanges();
                    //BankAssociatedCutofDate obankassociatedcudate = new BankAssociatedCutofDate();
                    //obankassociatedcudate = db.BankAssociatedCutofDates.Where(a => a.SalesYearID == SalesYearID).Select(a=>a.Id);
                }
                foreach (var itm in BankInfoList)
                {
                    BankAssociatedCutofDate obankassociatedcudate = new BankAssociatedCutofDate();
                    obankassociatedcudate.Id = Guid.NewGuid();
                    obankassociatedcudate.BankID = itm.BankID;
                    obankassociatedcudate.CutofDate = itm.CutOfDate;
                    obankassociatedcudate.SalesYearID = SalesYearID;
                    obankassociatedcudate.IsActive = true;
                    obankassociatedcudate.CreatedBy = userId;
                    obankassociatedcudate.CreatedDate = System.DateTime.Now;
                    obankassociatedcudate.LastUpdatedBy = userId;
                    obankassociatedcudate.LastUpdatedDate = System.DateTime.Now;
                    db.BankAssociatedCutofDates.Add(obankassociatedcudate);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

          //  return false;
        }

        public async Task<Guid> SaveStatus(SalesYearDTO _Dto, Guid Id, int EntityState)
        {
            SalesYearMaster _SalesYearMaster = new SalesYearMaster();
            _SalesYearMaster = await db.SalesYearMasters.Where(o => o.Id == Id).FirstOrDefaultAsync();

            if (_SalesYearMaster.StatusCode == EMPConstants.InActive)
            {
                _SalesYearMaster.StatusCode = EMPConstants.Active;
                //_SalesYearMaster.DeActivatedDate = null;
                //_SalesYearMaster.ActivatedDate = DateTime.Now;
            }
            else if (_SalesYearMaster.StatusCode == EMPConstants.Active)
            {
                //_SalesYearMaster.StatusCode = EMPConstants.InActive;
                //_SalesYearMaster.DeActivatedDate = DateTime.Now;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                _SalesYearMaster.LastUpdatedDate = DateTime.Now;
                _SalesYearMaster.LastUpdatedBy = _Dto.UserId;
                db.Entry(_SalesYearMaster).State = System.Data.Entity.EntityState.Modified;
            }
            try
            {
                await db.SaveChangesAsync();
                db.Dispose();
                return _SalesYearMaster.Id;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(_SalesYearMaster.Id))
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
