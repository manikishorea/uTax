using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPPortal.Transactions.DropDowns.DTO;
using EMP.Core.Utilities;
using EMP.Core.Documents.DTO;
using System.IO;
using System.Configuration;
using EMPPortal.Transactions.SubSiteFees.DTO;
using EMPPortal.Transactions.SubSiteFees;

namespace EMPPortal.Transactions.DropDowns
{
    public class DropDownService : IDropDownService
    {
        public DatabaseEntities db;
        public IQueryable<DropDownDTO> GetPhoneTypes()
        {
            db = new DatabaseEntities();
            var data = db.PhoneTypeMasters.Where(o => o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO
            {
                Id = o.Id,
                Name = o.PhoneType,
                Description = o.Description,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();
            return data;
        }

        public IQueryable<DropDownDTO> GetTitles()
        {
            db = new DatabaseEntities();
            var data = db.ContactPersonTitleMasters.Where(a => a.TypeId == 0 && a.StatusCode != EMPConstants.InActive).Select(o => new DropDownDTO
            {
                Id = o.Id,
                Name = o.ContactPersonTitle,
                Description = o.TypeId.ToString(),
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();
            return data.OrderBy( c=> c.Name);
        }

        public IQueryable<DropDownDTO> GetAlternativeTitles()
        {
            db = new DatabaseEntities();
            var data = db.ContactPersonTitleMasters.Where(a => a.TypeId == 1).Select(o => new DropDownDTO
            {
                Id = o.Id,
                Name = o.ContactPersonTitle,
                Description = o.Description,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();
            return data;
        }

        public IQueryable<DropDownDTO> GetAffiliateProgram(int entityid)
        {
            try
            {
                db = new DatabaseEntities();
                var data = (from affprog in db.AffiliateProgramMasters
                            join affprogent in db.AffiliationProgramEntityMaps on affprog.Id equals affprogent.AffiliateProgramId
                            where (affprogent.EntityId == entityid && affprog.StatusCode != EMPConstants.InActive)
                            select new { affprog, affprogent }).ToList();
                //select new DropDownDTO
                //{
                //    Id = affprog.Id,
                //    Name = affprog.Name,
                //    StatusCode = affprog.StatusCode,
                //    Description = DropDownDTO(affprog.DocumentPath).FilePath
                //}).DefaultIfEmpty();
                List<DropDownDTO> DropDownDTOlst = new List<DropDownDTO>();
                foreach (var itm in data)
                {
                    DropDownDTO omodel = new DropDownDTO();
                    omodel.Id = itm.affprog.Id;
                    omodel.Name = itm.affprog.Name;
                    omodel.StatusCode = itm.affprog.StatusCode;
                    omodel.Description = GetDocumentPath(itm.affprog.DocumentPath).FilePath;
                    DropDownDTOlst.Add(omodel);
                }
                return DropDownDTOlst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetAffiliateProgram", Guid.Empty);
                return new List<DropDownDTO>().AsQueryable();
            }
        }

        public IQueryable<DropDownDTO> GetAffiliateProgram(int entityid, Guid CustomerId)
        {
            try
            {
                DropDownService ddService = new DropDownService();
                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                EntityHierarchyDTOs = ddService.GetEntityHierarchies(CustomerId);

                Guid ParentId = Guid.Empty;
                int Level = EntityHierarchyDTOs.Count;
                if (EntityHierarchyDTOs.Count == 1)
                {
                    var LevelOne = EntityHierarchyDTOs.Where(o => o.Customer_Level == 0).FirstOrDefault();
                    if (LevelOne != null)
                    {
                        ParentId = LevelOne.CustomerId ?? Guid.Empty;
                    }
                }
                else
                {
                    var LevelOne = EntityHierarchyDTOs.Where(o => o.Customer_Level == 1).FirstOrDefault();
                    if (LevelOne != null)
                    {
                        ParentId = LevelOne.CustomerId ?? Guid.Empty;
                    }
                }

                db = new DatabaseEntities();
                var data = (from affprog in db.AffiliateProgramMasters
                            join affprogent in db.AffiliationProgramEntityMaps on affprog.Id equals affprogent.AffiliateProgramId
                            where (affprogent.EntityId == entityid && affprog.StatusCode != EMPConstants.InActive)
                            select new { affprog, affprogent }).ToList();

                //var P_CustoemrID = db.emp_CustomerInformation.Where(a => a.Id == CustomerId).Select(a => a.ParentId).FirstOrDefault();
                //if (P_CustoemrID == null)
                //{
                //    P_CustoemrID = CustomerId;
                //}

                //if(SubSiteAffiliateProgramConfig)

                List<DropDownDTO> DropDownDTOlst = new List<DropDownDTO>();
                foreach (var itm in data)
                {
                    DropDownDTO omodel = new DropDownDTO();
                    omodel.Id = itm.affprog.Id;
                    omodel.Name = itm.affprog.Name;
                    omodel.StatusCode = itm.affprog.StatusCode;
                    omodel.Description = GetDocumentPath(itm.affprog.DocumentPath).FilePath;

                    if (Level > 2)
                    {
                        if (db.EnrollmentAffiliateConfigurations.Any(a => a.CustomerId == ParentId && a.AffiliateProgramId == itm.affprog.Id)) //entityid == new Guid("0676dfd0-da29-41e3-a262-81cb528b796c")
                            DropDownDTOlst.Add(omodel);
                    }
                    else
                    {
                        if (db.SubSiteAffiliateProgramConfigs.Any(a => a.emp_CustomerInformation_ID == ParentId && a.AffiliateProgramMaster_ID == itm.affprog.Id) || (entityid == (int)EMPConstants.Entity.SO) || (entityid == (int)EMPConstants.Entity.SOME) || (entityid == (int)EMPConstants.Entity.SOME_SS)) //entityid == new Guid("0676dfd0-da29-41e3-a262-81cb528b796c")
                            DropDownDTOlst.Add(omodel);
                    }
                }
                return DropDownDTOlst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetAffiliateProgram", CustomerId);
                return new List<DropDownDTO>().AsQueryable();
            }
        }

        public DocumentMasterDTO GetDocumentPath(string id)
        {
            try
            {
                DocumentMasterDTO result = new DocumentMasterDTO();
                string filename = "";
                if (id.Trim().ToLower() != "nofile")
                {
                    Guid idval;
                    if (!Guid.TryParse(id, out idval))
                    {
                        return result;
                    }

                    var dataDocs = db.DocumentMasters.Where(o => o.Id == idval).Select(o => new DocumentMasterDTO
                    {
                        Id = o.Id,
                        FileName = o.FileName,
                        FileType = o.FileType,
                        FileData = o.FileData,
                        UserID = o.UserID,
                        CreatedDate = o.CreatedDate
                    }).FirstOrDefault();

                    if (dataDocs != null)
                    {
                        filename = id.ToString().Replace("-", "").Replace(" ", "") + "_" + dataDocs.FileName.Replace("-", "").Replace(" ", "");
                        if (!File.Exists(ConfigurationManager.AppSettings["DownloadPath"].ToString() + filename))
                        {
                            System.IO.File.WriteAllBytes(ConfigurationManager.AppSettings["DownloadPath"].ToString() + filename, dataDocs.FileData);
                        }
                        filename = "Download\\" + filename;
                    }
                    dataDocs.FilePath = filename;
                    return dataDocs;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetSubSiteBankAndQuestions2", Guid.Empty);
                return new DocumentMasterDTO();
            }
        }

        public IQueryable<DropDownDTO> GetBankMaster(int entityid)
        {
            // Guid Entity = new Guid("8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73");
            int Entity = (int)EMPConstants.Entity.uTax;

            db = new DatabaseEntities();
            var data = (from bank in db.BankMasters
                        join bankent in db.BankEntityMaps on bank.Id equals bankent.BankId
                        join ent in db.EntityMasters on bankent.EntityId equals ent.Id
                        where (Entity == entityid) ? bankent.EntityId == bankent.EntityId : bankent.EntityId == entityid
                        select new { bank }).Distinct().DefaultIfEmpty();

            List<DropDownDTO> DropDownDTOlst = new List<DropDownDTO>();
            foreach (var itm in data)
            {
                DropDownDTO omodel = new DropDownDTO();
                omodel.Id = itm.bank.Id;
                omodel.Name = itm.bank.BankName;
                omodel.StatusCode = itm.bank.StatusCode;
                omodel.Description = GetDocumentPath(itm.bank.BankProductDocument).FilePath;
                DropDownDTOlst.Add(omodel);
            }

            return DropDownDTOlst.AsQueryable();
        }

        public IQueryable<BankQuestionDTO> GetBankAndQuestions(int entityid)
        {
            db = new DatabaseEntities();
            var data = (from bank in db.BankMasters
                        join bankent in db.BankEntityMaps on bank.Id equals bankent.BankId
                        where (bankent.EntityId == entityid && bank.StatusCode != EMPConstants.InActive)
                        select new
                        {
                            bank,
                        }).ToList();

            List<BankQuestionDTO> DropDownDTOlst = new List<BankQuestionDTO>();
            foreach (var itm in data)
            {
                BankQuestionDTO omodel = new BankQuestionDTO();
                omodel.BankId = itm.bank.Id;
                omodel.BankName = itm.bank.BankName;
                omodel.Questions = db.BankSubQuestions.Where(o => o.StatusCode != EMPConstants.InActive && o.BankId == omodel.BankId).OrderBy(a => a.Options).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions, Description = o.Options.ToString() }).ToList();
                omodel.StatusCode = itm.bank.StatusCode;
                omodel.DocumentPath = GetDocumentPath(itm.bank.BankProductDocument).FilePath;
                DropDownDTOlst.Add(omodel);
            }

            return DropDownDTOlst.AsQueryable();
        }

        public IQueryable<BankQuestionDTO> GetBankAndQuestionsForSelection(int entityid, Guid CustomerId)//, Guid BankId
        {
            List<BankQuestionDTO> DropDownDTOlst = new List<BankQuestionDTO>();
            db = new DatabaseEntities();
            //var entitytype = db.EntityMasters.Where(x => x.Id == entityid).Select(x => x.DisplayId).FirstOrDefault();

            List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
            EntityHierarchyDTOs = GetEntityHierarchies(CustomerId);

            Guid TopParentId = Guid.Empty;
            Guid FeeSourceParentId = Guid.Empty;

            if (EntityHierarchyDTOs.Count > 0)
            {
                var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;

                var FeeSource = EntityHierarchyDTOs.Where(o => o.EntityId == o.FeeSourceEntityId).FirstOrDefault();
                if (FeeSource != null)
                {
                    FeeSourceParentId = FeeSource.CustomerId ?? Guid.Empty;
                }
                else
                {
                    FeeSourceParentId = TopParentId;
                }
            }


            if (entityid == (int)EMPConstants.Entity.SO || entityid == (int)EMPConstants.Entity.SOME || entityid == (int)EMPConstants.Entity.SOME_SS)
            {
                //var entity = db.EntityMasters.Where(x => x.Id == entityid).Select(x => x.Id).FirstOrDefault();
                //if (entity > 0)
                //{
                var data = (from bank in db.BankMasters
                            join bankent in db.BankEntityMaps on bank.Id equals bankent.BankId
                            where (bankent.EntityId == entityid && bank.StatusCode != EMPConstants.InActive) //&& bank.Id == BankId
                            select new
                            {
                                bank,
                            }).Distinct().ToList();

                foreach (var itm in data)
                {
                    BankQuestionDTO omodel = new BankQuestionDTO();
                    omodel.BankId = itm.bank.Id;
                    omodel.BankName = itm.bank.BankName;
                    omodel.Questions = db.BankSubQuestions.Where(o => o.StatusCode != EMPConstants.InActive && o.BankId == omodel.BankId).OrderBy(a => a.Options).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions, Description = o.Options.ToString() }).ToList();
                    omodel.StatusCode = itm.bank.StatusCode;
                    omodel.DocumentPath = GetDocumentPath(itm.bank.BankProductDocument).FilePath;
                    DropDownDTOlst.Add(omodel);
                }
                // }
            }
            else
            {
                //if (entitydisplayid == (int)EMPConstants.Entities.MOSubSite || entitydisplayid == (int)EMPConstants.Entities.SVBSubSite)
                //{
                //    var CustomerParentId = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).Select(x => x.ParentId).FirstOrDefault();
                //    if (CustomerParentId != null)
                //    {
                //        CustomerId = CustomerParentId.Value;
                //    }
                //}
                //else if(entitydisplayid == (int)EMPConstants.Entities.SOME_SubSite)
                //{
                //    var CustomerParentId = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).Select(x => x.ParentId).FirstOrDefault();
                //    if (CustomerParentId != null)
                //    {
                //        CustomerId = CustomerParentId.Value;
                //    }

                //}

                var data = (from bank in db.BankMasters
                            join config in db.SubSiteBankConfigs on bank.Id equals config.BankMaster_ID
                            where config.emp_CustomerInformation_ID == TopParentId && bank.StatusCode != EMPConstants.InActive // && bank.Id == BankId
                            select new
                            {
                                bank,
                            }).Distinct().ToList();

                foreach (var itm in data)
                {
                    BankQuestionDTO omodel = new BankQuestionDTO();
                    omodel.BankId = itm.bank.Id;
                    omodel.BankName = itm.bank.BankName;
                    omodel.Questions = db.BankSubQuestions.Where(o => o.StatusCode != EMPConstants.InActive && o.BankId == omodel.BankId).OrderBy(a => a.Options).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions, Description = o.Options.ToString() }).ToList();
                    omodel.StatusCode = itm.bank.StatusCode;
                    omodel.DocumentPath = GetDocumentPath(itm.bank.BankProductDocument).FilePath;
                    DropDownDTOlst.Add(omodel);
                }
            }
            return DropDownDTOlst.AsQueryable();
        }

        public IQueryable<FeeEntityDTO> GetFeeMaster(int entityid, Guid userid)
        {
            db = new DatabaseEntities();
            List<FeeEntityDTO> feeentitydtolst = new List<FeeEntityDTO>();
            var data = (from fee in db.FeeMasters
                        join feeent in db.FeeEntityMaps on fee.Id equals feeent.FeeId
                        where feeent.EntityId == entityid
                        select fee);
            if (data != null)
            {
                foreach (var itm in data)
                {
                    FeeEntityDTO feeentitydto = new FeeEntityDTO();
                    feeentitydto.Id = itm.Id;
                    feeentitydto.Name = itm.Name;

                    if (itm.FeeTypeId == 2)
                    {
                        feeentitydto.Amount = db.CustomerAssociatedFees.Where(a => a.FeeMaster_ID == itm.Id && a.emp_CustomerInformation_ID == userid && a.IsActive == true).Select(a => a.Amount).FirstOrDefault();
                    }
                    else
                    {
                        feeentitydto.Amount = itm.Amount ?? 0;
                    }

                    if (db.CustomerAssociatedFees.Where(a => a.FeeMaster_ID == itm.Id && a.emp_CustomerInformation_ID == userid && a.IsActive == true).Select(a => a.IsEdit).FirstOrDefault() == true)
                        feeentitydto.IsEdit = true;
                    else
                        feeentitydto.IsEdit = false;

                    feeentitydto.FeeTypeID = itm.FeeTypeId ?? 0;
                    feeentitydto.FeeCategoryID = itm.FeeCategoryID;
                    feeentitydto.FeeFor = itm.FeesFor ?? 0;
                    feeentitydtolst.Add(feeentitydto);
                }
            }
            //select new FeeEntityDTO
            //{
            //    Id = fee.Id,
            //    Name = fee.Name,
            //    Amount = fee.Amount ?? 0,
            //    FeeType = fee.FeeType
            //}).DefaultIfEmpty();

            return feeentitydtolst.AsQueryable();
        }

        public IQueryable<TooltipDTO> GetTooltip(Guid sitemapid)
        {
            db = new DatabaseEntities();
            var data = db.TooltipMasters.Where(o => o.IsUIVisible == true && o.SitemapId == sitemapid).Select(o => new TooltipDTO
            {
                Id = o.Id,
                Field = o.Field,
                Description = o.Description,
                Tooltip = o.ToolTipText
            }).DefaultIfEmpty();
            return data;
        }

        public IQueryable<BankQuestionDTO> GetSubSiteBankAndQuestionsOld(Guid Userid)
        {
            try
            {
                db = new DatabaseEntities();

                var data = (from bank in db.BankMasters
                            join ssb in db.SubSiteBankConfigs on bank.Id equals ssb.BankMaster_ID
                            where ssb.emp_CustomerInformation_ID == Userid && bank.StatusCode == EMPConstants.Active
                            select new { bank, ssb });
                List<BankQuestionDTO> BankLst = new List<BankQuestionDTO>();
                foreach (var itm in data)
                {
                    BankQuestionDTO bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.bank.Id;
                    bankmodel.BankName = itm.bank.BankName;
                    bankmodel.Questions = itm.bank.BankSubQuestions.Where(o => o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.bank.StatusCode;

                    bankmodel.BankSVBDesktopFee = itm.bank.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.bank.MaxFeeLimitMSO ?? 0;

                    bankmodel.BankTranDesktopFee = itm.bank.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.bank.MaxTranFeeMSO ?? 0;

                    bankmodel.StatusCode = itm.bank.StatusCode;
                    var vbc = (from ssbfc in db.SubSiteBankFeesConfigs
                               join ssfc in db.SubSiteFeeConfigs on ssbfc.SubSiteFeeConfig_ID equals ssfc.ID
                               where ssbfc.BankMaster_ID == itm.bank.Id && ssfc.emp_CustomerInformation_ID == Userid
                               select new { ssbfc, ssfc });
                    if (vbc.Any(a => a.ssfc.ServiceorTransmission == 1))
                    {
                        foreach (var itv in vbc)
                        {
                            bankmodel.DesktopFee = itv.ssbfc.BankMaxFees;
                            bankmodel.MSOFee = itv.ssbfc.BankMaxFees_MSO ?? 0;
                            bankmodel.IsSameforAll = itv.ssfc.IsSameforAll;
                            bankmodel.ServiceorTransmission = itv.ssfc.ServiceorTransmission;
                            BankLst.Add(bankmodel);
                        }
                    }
                    else if (vbc.Any(a => a.ssfc.ServiceorTransmission == 2))
                    {
                        foreach (var itv in vbc)
                        {
                            bankmodel.DesktopFee = itv.ssbfc.BankMaxFees;
                            bankmodel.MSOFee = itv.ssbfc.BankMaxFees_MSO ?? 0;
                            bankmodel.IsSameforAll = itv.ssfc.IsSameforAll;
                            bankmodel.ServiceorTransmission = itv.ssfc.ServiceorTransmission;
                            BankLst.Add(bankmodel);
                        }
                    }
                    else
                    {
                        bankmodel.DesktopFee = 0;
                        bankmodel.MSOFee = 0;
                        bankmodel.IsSameforAll = false;
                        bankmodel.ServiceorTransmission = 2;
                        BankLst.Add(bankmodel);
                    }

                }
                return BankLst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetSubSiteBankAndQuestionsOld", Userid);
                return new List<BankQuestionDTO>().AsQueryable();
            }
        }

        public IQueryable<BankQuestionDTO> GetSubSiteBankAndQuestions2(Guid Userid)
        {
            try
            {
                db = new DatabaseEntities();

                var data = (from bank in db.BankMasters
                            join ssbf in db.SubSiteBankFeesConfigs on bank.Id equals ssbf.BankMaster_ID
                            where ssbf.emp_CustomerInformation_ID == Userid && bank.StatusCode == EMPConstants.Active
                            && ssbf.emp_CustomerInformation_ID == Userid
                            select new { bank, ssbf });

                List<BankQuestionDTO> BankLst = new List<BankQuestionDTO>();

                foreach (var itm in data)
                {
                    BankQuestionDTO bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.bank.Id;
                    bankmodel.BankName = itm.bank.BankName;
                    bankmodel.Questions = db.BankSubQuestions.Where(o => o.BankId == itm.bank.Id && o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.bank.StatusCode;

                    bankmodel.DesktopFee = itm.ssbf.BankMaxFees;
                    bankmodel.MSOFee = itm.ssbf.BankMaxFees_MSO ?? 0;

                    bankmodel.BankSVBDesktopFee = itm.bank.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.bank.MaxFeeLimitMSO ?? 0;

                    bankmodel.BankTranDesktopFee = itm.bank.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.bank.MaxTranFeeMSO ?? 0;

                    bankmodel.StatusCode = itm.bank.StatusCode;

                    bankmodel.ServiceorTransmission = itm.ssbf.ServiceOrTransmitter ?? 0;

                    BankLst.Add(bankmodel);
                }
                return BankLst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetSubSiteBankAndQuestions2", Userid);
                return new List<BankQuestionDTO>().AsQueryable();
            }
        }

        /// <summary>
        /// This Method is used to Get the Bank Fee Config For Main And Sub Site //11082016
        /// </summary>
        /// <returns></returns>
        public IQueryable<BankQuestionDTO> GetSubSiteBankAndQuestionsOld2(Guid strguid)
        {
            try
            {
                //  DropDownService _DropDownService = new DropDownService();
                List<BankQuestionDTO> BankLst = new List<BankQuestionDTO>();

                //  var result = _DropDownService.GetHierarchyList(strguid);

                //  var datat = result.Where(o => o.ActiveStatus == 1).Select(o => o.Id).ToList();

                HierarchyDTO Hierarchy = new HierarchyDTO();
                Hierarchy = GetCustomerActiveStatus(strguid);

                db = new DatabaseEntities();
                var items = db.SubSiteBankConfigs
                       .Join(db.BankMasters, ssbc => ssbc.BankMaster_ID, bank => bank.Id,
                               (ssbc, bank) => new { ssbc, bank })
                .Where(o => o.ssbc.emp_CustomerInformation_ID == strguid && o.bank.StatusCode != EMPConstants.InActive)
                .GroupBy(x => new
                {
                    // x.ssbc.ServiceOrTransmitter,
                    x.ssbc.BankMaster_ID,
                    x.bank.BankName,
                    x.bank.StatusCode,
                    x.bank.MaxFeeLimitDeskTop,
                    x.bank.MaxFeeLimitMSO,
                    x.bank.MaxTranFeeDeskTop,
                    x.bank.MaxTranFeeMSO,
                    x.bank.BankProductDocument
                })
                .Select(g => new
                {
                    g.Key.BankMaster_ID,
                    //   g.Key.ServiceOrTransmitter,
                    //  BankMaxFees = g.Sum(z => z.ssbfc.BankMaxFees),
                    //   BankMaxFees_MSO = g.Sum(z => z.ssbfc.BankMaxFees_MSO),

                    g.Key.BankName,
                    g.Key.StatusCode,
                    g.Key.MaxFeeLimitDeskTop,
                    g.Key.MaxFeeLimitMSO,
                    g.Key.MaxTranFeeDeskTop,
                    g.Key.MaxTranFeeMSO,
                    g.Key.BankProductDocument
                }).ToList();

                var mainbankfeeconfig = db.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == strguid).ToList();
                List<SubSiteBankFeesDTO> lstsbfee = new List<SubSiteBankFeesDTO>();
                //GD-11042016
                //  List<BankQuestionDTO> SubBankLst = GetSubSiteOfficeBankAndQuestions(strguid).ToList();

                foreach (var itm in items)
                {

                    BankQuestionDTO bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.BankMaster_ID;
                    bankmodel.BankName = itm.BankName;
                    bankmodel.Questions = db.BankSubQuestions.Where(o => o.BankId == itm.BankMaster_ID && o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.StatusCode;

                    if (Hierarchy.ActiveStatus == 1)
                    {
                        var data3 = mainbankfeeconfig.Where(o => o.BankMaster_ID == itm.BankMaster_ID && o.ServiceOrTransmitter == 1).FirstOrDefault();
                        if (data3 != null)
                        {
                            bankmodel.DesktopFee = data3.BankMaxFees;// itm.BankMaxFees;
                            bankmodel.MSOFee = data3.BankMaxFees_MSO ?? 0;
                            //bankmodel.QuestionId = data3.QuestionID.HasValue ? data3.QuestionID.Value : Guid.Empty;
                        }
                    }
                    else
                    {
                        bankmodel.DesktopFee = 0;
                        bankmodel.MSOFee = 0;
                        //bankmodel.QuestionId =  Guid.Empty;
                    }


                    bankmodel.BankSVBDesktopFee = itm.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.MaxFeeLimitMSO ?? 0;

                    bankmodel.BankTranDesktopFee = itm.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.MaxTranFeeMSO ?? 0;

                    bankmodel.ServiceorTransmission = 1;
                    bankmodel.DocumentPath = GetDocumentPath(itm.BankProductDocument).FilePath;

                    //GD-11042016
                    //BankQuestionDTO SubBankQuestion = SubBankLst.Where(o => o.BankId == bankmodel.BankId && o.ServiceorTransmission == 1).FirstOrDefault();

                    //if (SubBankQuestion != null)
                    //{
                    //    bankmodel.SubDesktopFee = SubBankQuestion.DesktopFee;
                    //    bankmodel.SubMSOFee = SubBankQuestion.MSOFee;
                    //}

                    BankLst.Add(bankmodel);


                    bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.BankMaster_ID;
                    bankmodel.BankName = itm.BankName;
                    bankmodel.Questions = db.BankSubQuestions.Where(o => o.BankId == itm.BankMaster_ID && o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.StatusCode;

                    if (Hierarchy.ActiveStatus == 1)
                    {
                        var data3 = mainbankfeeconfig.Where(o => o.BankMaster_ID == itm.BankMaster_ID && o.ServiceOrTransmitter == 2).FirstOrDefault();
                        if (data3 != null)
                        {
                            bankmodel.DesktopFee = data3.BankMaxFees;// itm.BankMaxFees;
                            bankmodel.MSOFee = data3.BankMaxFees_MSO ?? 0;
                        }
                    }
                    else
                    {
                        bankmodel.DesktopFee = 0;
                        bankmodel.MSOFee = 0;
                    }


                    bankmodel.BankSVBDesktopFee = itm.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.MaxFeeLimitMSO ?? 0;

                    bankmodel.BankTranDesktopFee = itm.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.MaxTranFeeMSO ?? 0;

                    bankmodel.ServiceorTransmission = 2;
                    bankmodel.DocumentPath = GetDocumentPath(itm.BankProductDocument).FilePath;

                    //GD-11042016
                    //SubBankQuestion = SubBankLst.Where(o => o.BankId == bankmodel.BankId && o.ServiceorTransmission == 2).FirstOrDefault();

                    //if (SubBankQuestion != null)
                    //{
                    //    bankmodel.SubDesktopFee = SubBankQuestion.DesktopFee;
                    //    bankmodel.SubMSOFee = SubBankQuestion.MSOFee;
                    //}

                    BankLst.Add(bankmodel);
                }

                return BankLst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetSubSiteBankAndQuestions", strguid);
                return new List<BankQuestionDTO>().AsQueryable();
            }
        }


        /// <summary>
        /// This Method is used to Get the Bank Fee Config For Main And Sub Site
        /// </summary>
        /// <returns></returns>
        public IQueryable<BankQuestionDTO> GetSubSiteBankAndQuestions(Guid strguid)
        {
            try
            {
                List<SubSiteBankFeesDTO> lstsbfee = new List<SubSiteBankFeesDTO>();

                List<BankQuestionDTO> BankLst = new List<BankQuestionDTO>();

                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                EntityHierarchyDTOs = GetEntityHierarchies(strguid);

                Guid TopParentId = Guid.Empty;
                Guid FeeSourceParentId = Guid.Empty;

                if (EntityHierarchyDTOs.Count > 0)
                {
                    var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                    TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;

                    var FeeSource = EntityHierarchyDTOs.Where(o => o.EntityId == o.FeeSourceEntityId).FirstOrDefault();
                    if (FeeSource != null)
                    {
                        FeeSourceParentId = FeeSource.CustomerId ?? Guid.Empty;
                    }
                    else
                    {
                        FeeSourceParentId = TopParentId;
                    }
                }

                if (EntityHierarchyDTOs.Count <= 1)
                {
                    if (EntityHierarchyDTOs.Where(o => o.EntityId == (int)EMPConstants.Entity.SO || o.EntityId == (int)EMPConstants.Entity.SOME).Any())
                    {
                        return GetSubSiteBankAndQuestions_Level0();
                    }
                }

                if (EntityHierarchyDTOs.Count > 1)
                {
                    if (EntityHierarchyDTOs.Where(o => o.EntityId == (int)EMPConstants.Entity.SOME_SS).Any())
                    {
                        return GetSubSiteBankAndQuestions_LevelAE(strguid);
                    }
                }

                db = new DatabaseEntities();
                var items = db.SubSiteBankConfigs
                       .Join(db.BankMasters, ssbc => ssbc.BankMaster_ID, bank => bank.Id,
                               (ssbc, bank) => new { ssbc, bank })
                .Where(o => o.ssbc.emp_CustomerInformation_ID == TopParentId && o.bank.StatusCode != EMPConstants.InActive)
                .GroupBy(x => new
                {
                    x.ssbc.BankMaster_ID,
                    x.bank.BankName,
                    x.bank.StatusCode,
                    x.bank.MaxFeeLimitDeskTop,
                    x.bank.MaxFeeLimitMSO,
                    x.bank.MaxTranFeeDeskTop,
                    x.bank.MaxTranFeeMSO,
                    x.bank.BankProductDocument
                })
                .Select(g => new
                {
                    g.Key.BankMaster_ID,
                    g.Key.BankName,
                    g.Key.StatusCode,
                    g.Key.MaxFeeLimitDeskTop,
                    g.Key.MaxFeeLimitMSO,
                    g.Key.MaxTranFeeDeskTop,
                    g.Key.MaxTranFeeMSO,
                    g.Key.BankProductDocument
                }).ToList();

                var mainbankfeeconfig = db.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == FeeSourceParentId).ToList();


                //GD-11042016
                //  List<BankQuestionDTO> SubBankLst = GetSubSiteOfficeBankAndQuestions(strguid).ToList();

                foreach (var itm in items)
                {

                    BankQuestionDTO bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.BankMaster_ID;
                    bankmodel.BankName = itm.BankName;
                    bankmodel.Questions = db.BankSubQuestions.Where(o => o.BankId == itm.BankMaster_ID && o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.StatusCode;

                    //if (Hierarchy.ActiveStatus == 1)
                    //{
                    var SVBFeeData = mainbankfeeconfig.Where(o => o.BankMaster_ID == itm.BankMaster_ID && o.ServiceOrTransmitter == 1).FirstOrDefault();
                    if (SVBFeeData != null)
                    {
                        bankmodel.DesktopFee = SVBFeeData.BankMaxFees;// itm.BankMaxFees;
                        bankmodel.MSOFee = SVBFeeData.BankMaxFees_MSO ?? 0;
                        //bankmodel.QuestionId = data3.QuestionID.HasValue ? data3.QuestionID.Value : Guid.Empty;
                    }
                    else
                    {
                        bankmodel.DesktopFee = 0;
                        bankmodel.MSOFee = 0;
                    }
                    //}
                    //else
                    //{
                    //    bankmodel.DesktopFee = 0;
                    //    bankmodel.MSOFee = 0;
                    //    //bankmodel.QuestionId =  Guid.Empty;
                    //}


                    bankmodel.BankSVBDesktopFee = itm.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.MaxFeeLimitMSO ?? 0;

                    bankmodel.BankTranDesktopFee = itm.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.MaxTranFeeMSO ?? 0;

                    bankmodel.ServiceorTransmission = 1;
                    bankmodel.DocumentPath = GetDocumentPath(itm.BankProductDocument).FilePath;

                    BankLst.Add(bankmodel);


                    bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.BankMaster_ID;
                    bankmodel.BankName = itm.BankName;
                    bankmodel.Questions = db.BankSubQuestions.Where(o => o.BankId == itm.BankMaster_ID && o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.StatusCode;

                    var TranFeeData = mainbankfeeconfig.Where(o => o.BankMaster_ID == itm.BankMaster_ID && o.ServiceOrTransmitter == 2).FirstOrDefault();
                    if (TranFeeData != null)
                    {
                        bankmodel.DesktopFee = TranFeeData.BankMaxFees;// itm.BankMaxFees;
                        bankmodel.MSOFee = TranFeeData.BankMaxFees_MSO ?? 0;
                    }
                    else
                    {
                        bankmodel.DesktopFee = 0;
                        bankmodel.MSOFee = 0;
                    }


                    bankmodel.BankSVBDesktopFee = itm.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.MaxFeeLimitMSO ?? 0;

                    bankmodel.BankTranDesktopFee = itm.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.MaxTranFeeMSO ?? 0;

                    bankmodel.ServiceorTransmission = 2;
                    bankmodel.DocumentPath = GetDocumentPath(itm.BankProductDocument).FilePath;

                    BankLst.Add(bankmodel);
                }

                return BankLst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetSubSiteBankAndQuestions", strguid);
                return new List<BankQuestionDTO>().AsQueryable();
            }
        }


        /// <summary>
        /// This Method is used to Get the Bank Fee Config For SO,SOME and Level=0
        /// </summary>
        /// <returns></returns>
        public IQueryable<BankQuestionDTO> GetSubSiteBankAndQuestions_Level0()
        {
            try
            {
                List<BankQuestionDTO> BankLst = new List<BankQuestionDTO>();

                db = new DatabaseEntities();
                var items = db.BankMasters
                .Where(o => o.StatusCode != EMPConstants.InActive)
                .GroupBy(x => new
                {
                    x.Id,
                    x.BankName,
                    x.StatusCode,
                    x.MaxFeeLimitDeskTop,
                    x.MaxFeeLimitMSO,
                    x.MaxTranFeeDeskTop,
                    x.MaxTranFeeMSO,
                    x.BankProductDocument
                })
                .Select(g => new
                {
                    g.Key.Id,
                    g.Key.BankName,
                    g.Key.StatusCode,
                    g.Key.MaxFeeLimitDeskTop,
                    g.Key.MaxFeeLimitMSO,
                    g.Key.MaxTranFeeDeskTop,
                    g.Key.MaxTranFeeMSO,
                    g.Key.BankProductDocument
                }).ToList();

                // var mainbankfeeconfig = db.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == FeeSourceParentId).ToList();
                List<int> FeeFor = new List<int>();
                FeeFor.Add((int)EMPConstants.FeesFor.SVBFees);

                var mainbankfeeconfig = db.FeeMasters.Where(o => FeeFor.Contains(o.FeesFor ?? 0));
                List<SubSiteBankFeesDTO> lstsbfee = new List<SubSiteBankFeesDTO>();


                foreach (var itm in items)
                {

                    BankQuestionDTO bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.Id;
                    bankmodel.BankName = itm.BankName;
                    bankmodel.Questions = db.BankSubQuestions.Where(o => o.BankId == itm.Id && o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.StatusCode;

                    var SVBFeeData = mainbankfeeconfig.Where(o => o.FeesFor == (int)EMPConstants.FeesFor.SVBFees).FirstOrDefault();
                    if (SVBFeeData != null)
                    {
                        bankmodel.DesktopFee = SVBFeeData.Amount ?? 0;// itm.BankMaxFees;
                        bankmodel.MSOFee = 0;// SVBFeeData.BankMaxFees_MSO ?? 0;
                    }
                    else
                    {
                        bankmodel.DesktopFee = 0;
                        bankmodel.MSOFee = 0;
                    }


                    bankmodel.BankSVBDesktopFee = itm.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.MaxFeeLimitMSO ?? 0;

                    bankmodel.BankTranDesktopFee = itm.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.MaxTranFeeMSO ?? 0;

                    bankmodel.ServiceorTransmission = 1;
                    bankmodel.DocumentPath = GetDocumentPath(itm.BankProductDocument).FilePath;

                    BankLst.Add(bankmodel);


                    bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.Id;
                    bankmodel.BankName = itm.BankName;
                    bankmodel.Questions = db.BankSubQuestions.Where(o => o.BankId == itm.Id && o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.StatusCode;

                    var TranFeeData = mainbankfeeconfig.Where(o => o.FeesFor == (int)EMPConstants.FeesFor.TransmissionFees).FirstOrDefault();
                    if (TranFeeData != null)
                    {
                        bankmodel.DesktopFee = TranFeeData.Amount ?? 0;// itm.BankMaxFees;
                        bankmodel.MSOFee = 0;// TranFeeData.BankMaxFees_MSO ?? 0;
                    }
                    else
                    {
                        bankmodel.DesktopFee = 0;
                        bankmodel.MSOFee = 0;
                    }

                    bankmodel.BankSVBDesktopFee = itm.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.MaxFeeLimitMSO ?? 0;

                    bankmodel.BankTranDesktopFee = itm.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.MaxTranFeeMSO ?? 0;

                    bankmodel.ServiceorTransmission = 2;
                    bankmodel.DocumentPath = GetDocumentPath(itm.BankProductDocument).FilePath;

                    BankLst.Add(bankmodel);
                }

                return BankLst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetSubSiteBankAndQuestions", null);
                return new List<BankQuestionDTO>().AsQueryable();
            }
        }

        public IQueryable<BankQuestionDTO> GetSubSiteBankAndQuestions_LevelAE(Guid MyId)
        {
            try
            {
                List<BankQuestionDTO> BankLst = new List<BankQuestionDTO>();

                db = new DatabaseEntities();

                db = new DatabaseEntities();
                var items = db.SubSiteBankFeesConfigs
                       .Join(db.BankMasters, ssbc => ssbc.BankMaster_ID, bank => bank.Id,
                               (ssbc, bank) => new { ssbc, bank })
                .Where(o => o.ssbc.emp_CustomerInformation_ID == MyId && o.bank.StatusCode != EMPConstants.InActive).Select(x => new
                {
                    x.ssbc.BankMaster_ID,
                    x.bank.BankName,
                    x.bank.StatusCode,
                    x.bank.MaxFeeLimitDeskTop,
                    x.bank.MaxFeeLimitMSO,
                    x.bank.MaxTranFeeDeskTop,
                    x.bank.MaxTranFeeMSO,
                    x.bank.BankProductDocument,
                    x.ssbc.BankMaxFees,
                    x.ssbc.BankMaxFees_MSO,
                    x.ssbc.ServiceOrTransmitter
                }).ToList();

                List<int> FeeFor = new List<int>();
                FeeFor.Add((int)EMPConstants.FeesFor.SVBFees);

                var mainbankfeeconfig = db.FeeMasters.Where(o => FeeFor.Contains(o.FeesFor ?? 0));
                List<SubSiteBankFeesDTO> lstsbfee = new List<SubSiteBankFeesDTO>();


                foreach (var itm in items)
                {

                    BankQuestionDTO bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.BankMaster_ID;
                    bankmodel.BankName = itm.BankName;
                    bankmodel.Questions = db.BankSubQuestions.Where(o => o.BankId == itm.BankMaster_ID && o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.StatusCode;
                    bankmodel.DesktopFee = itm.BankMaxFees;
                    bankmodel.MSOFee = itm.BankMaxFees_MSO ?? 0;

                    bankmodel.BankSVBDesktopFee = itm.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.MaxFeeLimitMSO ?? 0;

                    var SVBFeeData = mainbankfeeconfig.Where(o => o.FeesFor == (int)EMPConstants.FeesFor.SVBFees).FirstOrDefault();
                    if (SVBFeeData != null)
                    {
                        bankmodel.BankSVBDesktopFee = bankmodel.BankSVBDesktopFee - SVBFeeData.Amount ?? 0;
                        bankmodel.BankSVBMSOFee = bankmodel.BankSVBMSOFee - SVBFeeData.Amount ?? 0;
                    }

                    bankmodel.BankTranDesktopFee = itm.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.MaxTranFeeMSO ?? 0;

                    bankmodel.ServiceorTransmission = itm.ServiceOrTransmitter ?? 0;
                    bankmodel.DocumentPath = GetDocumentPath(itm.BankProductDocument).FilePath;
                    BankLst.Add(bankmodel);
                }

                return BankLst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetSubSiteBankAndQuestions", null);
                return new List<BankQuestionDTO>().AsQueryable();
            }
        }

        /// <summary>
        /// This Method is used to Get the Main Office Configuration Details
        /// </summary>
        /// <returns></returns>
        public IQueryable<BankQuestionDTO> GetSubSiteOfficeBankAndQuestions(Guid strguid)
        {
            try
            {
                DropDownService _DropDownService = new DropDownService();
                List<BankQuestionDTO> BankLst = new List<BankQuestionDTO>();

                var result = _DropDownService.GetHierarchyList(strguid);

                var datat = result.Where(o => o.ActiveStatus == 1).Select(o => o.Id).ToList();

                db = new DatabaseEntities();
                if (datat.ToList().Count > 0)
                {
                    var items = db.SubSiteBankFeesConfigs
                    .Where(o => datat.Contains(o.emp_CustomerInformation_ID))
                    .GroupBy(x => new
                    {
                        x.ServiceOrTransmitter,
                        x.BankMaster_ID,
                        // x.BankMaxFees,
                        // x.BankMaxFees_MSO
                    })
                    .Select(g => new
                    {
                        g.Key.BankMaster_ID,
                        g.Key.ServiceOrTransmitter,
                        BankMaxFees = g.Max(z => z.BankMaxFees),
                        BankMaxFees_MSO = g.Max(z => z.BankMaxFees_MSO ?? 0),
                    }).ToList();


                    List<SubSiteBankFeesDTO> lstsbfee = new List<SubSiteBankFeesDTO>();

                    foreach (var itm in items)
                    {
                        BankQuestionDTO bankmodel = new BankQuestionDTO();
                        bankmodel.BankId = itm.BankMaster_ID;
                        bankmodel.DesktopFee = itm.BankMaxFees;
                        bankmodel.MSOFee = itm.BankMaxFees_MSO;
                        bankmodel.ServiceorTransmission = itm.ServiceOrTransmitter ?? 0;
                        BankLst.Add(bankmodel);
                    }

                }
                return BankLst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetSubSiteOfficeBankAndQuestions", strguid);
                return new List<BankQuestionDTO>().AsQueryable();
            }
        }

        public IQueryable<BankDTO> GetUserBanks(Guid UserId)
        {
            db = new DatabaseEntities();
            var data = (from subsitebank in db.SubSiteBankConfigs
                        join bank in db.BankMasters on subsitebank.BankMaster_ID equals bank.Id
                        where (subsitebank.emp_CustomerInformation_ID == UserId && bank.StatusCode == EMPConstants.Active)
                        select new
                        {
                            bank
                            //BankId = bank.Id,
                            //BankName = bank.BankName,
                            //StatusCode = bank.StatusCode,
                            //FeeDesktop = bank.MaxFeeLimitDeskTop,
                            //FeeMSO = bank.MaxFeeLimitMSO,
                            //TranFeeDesktop = bank.MaxTranFeeDeskTop,
                            //TranFeeMSO = bank.MaxTranFeeMSO
                        }).DefaultIfEmpty();

            List<BankDTO> DropDownDTOlst = new List<BankDTO>();
            foreach (var itm in data)
            {
                BankDTO omodel = new BankDTO();
                omodel.BankId = itm.bank.Id;
                omodel.BankName = itm.bank.BankName;
                omodel.StatusCode = itm.bank.StatusCode;
                omodel.FeeDesktop = itm.bank.MaxFeeLimitDeskTop;
                omodel.FeeMSO = itm.bank.MaxFeeLimitMSO;
                omodel.TranFeeDesktop = itm.bank.MaxTranFeeDeskTop;
                omodel.TranFeeMSO = itm.bank.MaxTranFeeMSO;
                omodel.DocumentPath = GetDocumentPath(itm.bank.BankProductDocument).FilePath;
                DropDownDTOlst.Add(omodel);
            }

            return DropDownDTOlst.AsQueryable();
        }

        //public IQueryable<SalesYearDTO> GetAllSalesYears(Guid UserId)
        //{
        //    db = new DatabaseEntities();
        //    var Entity = (from cl in db.emp_CustomerLoginInformation
        //                  join cu in db.emp_CustomerInformation on cl.CustomerOfficeId equals cu.Id
        //                  join sy in db.SalesYearMasters on cu.SalesYearID equals sy.Id
        //                  where cu.Id == UserId && sy.ApplicableToDate != null
        //                  select new SalesYearDTO { Id = cu.SalesYearID ?? Guid.Empty, SalesYear = sy.SalesYear }).DefaultIfEmpty();
        //    return Entity;
        //}


        public IQueryable<StatusCodeDTO> GetStatusCode()
        {
            db = new DatabaseEntities();

            int StatusCode = (int)EMPConstants.StatusType.statuscode;
            var data = db.StatusCodes.Where(o => o.RefTypeId == StatusCode && o.IsActive == true).Select(o => new StatusCodeDTO
            {
                Id = o.Id,
                Name = o.Status,
                DisplayText = o.DisplayText
            }).DefaultIfEmpty();
            return data;
        }

        public IQueryable<StatusCodeDTO> GetEnrollmentStatus()
        {
            db = new DatabaseEntities();

            int enrollmentstatus = (int)EMPConstants.StatusType.enrollmentstatus;
            var data = db.StatusCodes.Where(o => o.RefTypeId == enrollmentstatus && o.IsActive == true).Select(o => new StatusCodeDTO
            {
                Id = o.Id,
                Name = o.Status,
                DisplayText = o.DisplayText
            }).DefaultIfEmpty();
            return data;
        }

        public IQueryable<StatusCodeDTO> GetOnBoardingStatus()
        {
            db = new DatabaseEntities();

            int onboarding = (int)EMPConstants.StatusType.onboarding;
            var data = db.StatusCodes.Where(o => o.RefTypeId == onboarding).Select(o => new StatusCodeDTO
            {
                Id = o.Id,
                Name = o.Status,
                DisplayText = o.DisplayText
            }).DefaultIfEmpty();
            return data;
        }

        public IQueryable<EntityDropDwonDTO> GetEntities()
        {
            db = new DatabaseEntities();
            var data = db.EntityMasters.Where(o => o.StatusCode == EMPConstants.Active && o.ParentId == 1).OrderBy(o => o.DisplayOrder).Select(o => new EntityDropDwonDTO
            {
                Id = o.Id,
                Name = o.Name,
                Description = o.Description
            }).DefaultIfEmpty();
            return data;
        }

        public IQueryable<StateMasterDTO> GetStateMasterList()
        {
            try
            {
                db = new DatabaseEntities();
                var data = db.StateMasters.Select(o => new StateMasterDTO
                {
                    StateID = o.StateID,
                    StateName = o.StateName,
                    StateCode = o.StateCode
                }).DefaultIfEmpty();
                return data.OrderBy(a => a.StateName);
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetStateMasterList", Guid.Empty);
                return new List<StateMasterDTO>().AsQueryable();
            }
        }

        public IQueryable<HierarchyDTO> GetHierarchyList(Guid UserId)
        {
            try
            {
                db = new DatabaseEntities();
                List<HierarchyDTO> HierarchyList = new List<HierarchyDTO>();
                var items = db.emp_CustomerInformation.Where(o => o.ParentId == UserId)
                               .Select(g => new
                               {
                                   g.Id,
                                   g.ParentId,
                                   g.CompanyName,
                                   g.EFIN,
                                   g.MasterIdentifier,
                                   g.IsActivationCompleted,
                               }).ToList();

                foreach (var item in items)
                {
                    HierarchyDTO Hierarchy = new HierarchyDTO();
                    Hierarchy.Id = item.Id;
                    Hierarchy.CompanyName = item.CompanyName;
                    Hierarchy.EFIN = item.EFIN ?? 0;
                    Hierarchy.ParentId = item.ParentId;
                    Hierarchy.ActiveStatus = item.IsActivationCompleted ?? 0;
                    HierarchyList.Add(Hierarchy);

                    //if (Hierarchy.ParentId == null || Hierarchy.ParentId == Guid.Empty)
                    {
                        var data = GetHierarchyList2(Hierarchy.Id, HierarchyList);
                    }
                }

                return HierarchyList.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetHierarchyList", UserId);
                return new List<HierarchyDTO>().AsQueryable();
            }
        }

        public IQueryable<HierarchyDTO> GetHierarchyList2(Guid UserId, List<HierarchyDTO> HierarchyList)
        {
            try
            {
                db = new DatabaseEntities();
                // List<HierarchyDTO> HierarchyList = new List<HierarchyDTO>();
                var items = db.emp_CustomerInformation.Where(o => o.ParentId == UserId)
                               .Select(g => new
                               {
                                   g.Id,
                                   g.ParentId,
                                   g.CompanyName,
                                   g.EFIN,
                                   g.MasterIdentifier,
                                   g.IsActivationCompleted,
                               }).ToList();

                foreach (var item in items)
                {
                    HierarchyDTO Hierarchy = new HierarchyDTO();
                    Hierarchy.Id = item.Id;
                    Hierarchy.CompanyName = item.CompanyName;
                    Hierarchy.EFIN = item.EFIN ?? 0;
                    Hierarchy.ParentId = item.ParentId;
                    Hierarchy.ActiveStatus = item.IsActivationCompleted ?? 0;
                    HierarchyList.Add(Hierarchy);

                    //if (Hierarchy.ParentId == null || Hierarchy.ParentId == Guid.Empty)
                    {
                        var data = GetHierarchyList2(Hierarchy.Id, HierarchyList);
                    }
                }

                return HierarchyList.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetChildCustomerInfo_AdditionalEFIN", UserId);
                return new List<HierarchyDTO>().AsQueryable();
            }
        }

        public HierarchyDTO GetCustomerActiveStatus(Guid UserId)
        {
            try
            {
                db = new DatabaseEntities();
                HierarchyDTO Hierarchy = new HierarchyDTO();
                var item = db.emp_CustomerInformation.Where(o => o.Id == UserId)
                               .Select(g => new
                               {
                                   g.Id,
                                   g.ParentId,
                                   g.CompanyName,
                                   g.EFIN,
                                   g.MasterIdentifier,
                                   g.IsActivationCompleted,
                               }).FirstOrDefault();

                if (item != null)
                {

                    Hierarchy.Id = item.Id;
                    Hierarchy.CompanyName = item.CompanyName;
                    Hierarchy.EFIN = item.EFIN ?? 0;
                    Hierarchy.ParentId = item.ParentId;
                    Hierarchy.ActiveStatus = item.IsActivationCompleted ?? 0;
                }

                return Hierarchy;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetCustomerActiveStatus", UserId);
                return null;
            }
        }

        public EntityHierarchyDTO GetEntityHierarchy(Guid UserId)
        {
            try
            {
                db = new DatabaseEntities();
                EntityHierarchyDTO Hierarchy = new EntityHierarchyDTO();
                var items = (from emp in db.emp_CustomerInformation
                             join ent in db.EntityMasters on emp.EntityId equals ent.Id
                             join enthie in db.EntityHierarchies on emp.Id equals enthie.CustomerId
                             where enthie.RelationId == UserId
                             select new
                             {
                                 Id = enthie.Id,
                                 RelationId = enthie.RelationId,
                                 Customer_Level = enthie.Customer_Level,
                                 CustomerId = enthie.CustomerId,
                                 EntityId = enthie.EntityId,
                                 Status = enthie.Status,
                                 FeeSourceEntityId = ent.FeeSourceEntityId
                             });

                foreach (var item in items)
                {
                    if (item.EntityId == item.FeeSourceEntityId)
                    {
                        Hierarchy.Id = item.Id;
                        Hierarchy.RelationId = item.RelationId;
                        Hierarchy.Customer_Level = item.Customer_Level;
                        Hierarchy.CustomerId = item.CustomerId;
                        Hierarchy.EntityId = item.EntityId;
                        Hierarchy.Status = item.Status;
                        Hierarchy.FeeSourceEntityId = item.FeeSourceEntityId;
                    }
                }

                return Hierarchy;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetCustomerActiveStatus", UserId);
                return null;
            }
        }

        public List<EntityHierarchyDTO> GetEntityHierarchies(Guid UserId)
        {
            try
            {
                int FeeSourceEntityId = 0;
                db = new DatabaseEntities();
                List<EntityHierarchyDTO> Hierarchies = new List<EntityHierarchyDTO>();
                var items = (from emp in db.emp_CustomerInformation
                             join ent in db.EntityMasters on emp.EntityId equals ent.Id
                             join enthie in db.EntityHierarchies on emp.Id equals enthie.CustomerId
                             where enthie.RelationId == UserId
                             select new
                             {
                                 Id = enthie.Id,
                                 RelationId = enthie.RelationId,
                                 Customer_Level = enthie.Customer_Level,
                                 CustomerId = enthie.CustomerId,
                                 EntityId = enthie.EntityId,
                                 Status = enthie.Status,
                                 FeeSourceEntityId = ent.FeeSourceEntityId
                             }).ToList();

                foreach (var item in items)
                {
                    EntityHierarchyDTO Hierarchy = new EntityHierarchyDTO();
                    Hierarchy.Id = item.Id;
                    Hierarchy.RelationId = item.RelationId;
                    Hierarchy.Customer_Level = item.Customer_Level;
                    Hierarchy.CustomerId = item.CustomerId;
                    Hierarchy.EntityId = item.EntityId;
                    Hierarchy.Status = item.Status;
                    if (Hierarchy.Customer_Level == 0)
                    {
                        FeeSourceEntityId = item.FeeSourceEntityId ?? 0;
                    }
                    Hierarchy.FeeSourceEntityId = FeeSourceEntityId;

                    Hierarchies.Add(Hierarchy);
                }

                return Hierarchies;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetCustomerActiveStatus", UserId);
                return null;
            }
        }

        //select emp.CompanyName, emp.Id, enthie.Customer_Level, enthie.RelationId, ent.ParentId, ent.Id, ent.Name, ent.FeeSourceEntityId, ent.BaseEntityId from emp_CustomerInformation emp
        //join EntityMaster ent on emp.EntityId = ent.Id
        //join EntityHierarchy enthie on enthie.CustomerId= emp.Id
        //where enthie.RelationId in ('4D42F2AE-26FD-424E-97AD-B4CCBCD46061')

        public IQueryable<EntityHierarchy> GetBottomToTopHierarchy(Guid MyId)
        {
            try
            {
                db = new DatabaseEntities();

                var EntityHies = db.EntityHierarchies.Where(o => o.RelationId == MyId).ToList();
                if (EntityHies.Count > 0)
                    db.EntityHierarchies.RemoveRange(EntityHies);

                List<EntityHierarchy> HierarchyList = new List<EntityHierarchy>();
                var item = db.emp_CustomerInformation.Where(o => o.Id == MyId)
                               .Select(g => new
                               {
                                   g.Id,
                                   g.ParentId,
                                   g.EntityId,
                                   // FeeEntitySourceId = g.EntityMaster.FeeSourceEntityId,
                               }).FirstOrDefault();

                if (item != null)
                {
                    int level = 0;
                    EntityHierarchy Hierarchy = new EntityHierarchy();
                    Hierarchy.Id = 0;
                    Hierarchy.CustomerId = item.Id;
                    Hierarchy.Customer_Level = level;
                    Hierarchy.EntityId = item.EntityId;
                    Hierarchy.RelationId = MyId;
                    Hierarchy.Status = "ACT";

                    db.EntityHierarchies.Add(Hierarchy);
                    db.SaveChanges();

                    HierarchyList.Add(Hierarchy);

                    if (item.ParentId != null && item.ParentId != Guid.Empty)
                    {
                        var data = GetBottomToTopHierarchy2(item.ParentId, MyId, HierarchyList, level);
                    }
                }


                return HierarchyList.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetHierarchyList", MyId);
                return new List<EntityHierarchy>().AsQueryable();
            }
        }

        private IQueryable<EntityHierarchy> GetBottomToTopHierarchy2(Guid? MyId, Guid UserId, List<EntityHierarchy> HierarchyList, int level)
        {
            try
            {
                db = new DatabaseEntities();
                var item = db.emp_CustomerInformation.Where(o => o.Id == MyId)
                               .Select(g => new
                               {
                                   g.Id,
                                   g.ParentId,
                                   g.EntityId,
                                   //FeeEntitySourceId = g.EntityMaster.FeeSourceEntityId,
                               }).FirstOrDefault();

                if (item != null)
                {
                    level = level + 1;
                    EntityHierarchy Hierarchy = new EntityHierarchy();
                    Hierarchy.Id = 0;
                    Hierarchy.CustomerId = item.Id;
                    Hierarchy.Customer_Level = level;
                    Hierarchy.EntityId = item.EntityId;
                    Hierarchy.RelationId = UserId;
                    Hierarchy.Status = "ACT";



                    db.EntityHierarchies.Add(Hierarchy);
                    db.SaveChanges();
                    HierarchyList.Add(Hierarchy);
                    if (item.ParentId != null && item.ParentId != Guid.Empty)
                    {
                        var data = GetBottomToTopHierarchy2(item.ParentId, UserId, HierarchyList, level);
                    }
                }

                return HierarchyList.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetChildCustomerInfo_AdditionalEFIN", UserId);
                return new List<EntityHierarchy>().AsQueryable();
            }
        }

        public IQueryable<StatusCodeDTO> GetEFINStatus(int entityid)
        {
            db = new DatabaseEntities();
            int StatusCode = 0;
            if (entityid == (int)EMPConstants.Entity.MO || entityid == (int)EMPConstants.Entity.SVB || entityid == (int)EMPConstants.Entity.SO || entityid == (int)EMPConstants.Entity.SOME)
                StatusCode = (int)EMPConstants.StatusType.efinstatusmain;
            else
                StatusCode = (int)EMPConstants.StatusType.efinstatussub;

            var data = db.StatusCodes.Where(o => o.RefTypeId == StatusCode && o.IsActive == true).Select(o => new StatusCodeDTO
            {
                Id = o.Id,
                Name = o.Status,
                DisplayText = o.DisplayText
            }).DefaultIfEmpty();

            return data;
        }

        public List<Guid> GetHierarchyCustomerIds(Guid CustomerId)
        {
            List<Guid> customers = new List<Guid>();
            try
            {
                db = new DatabaseEntities();
                Guid TopId = CustomerId;
                var customer = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                if (customer != null)
                {
                    int soEntity = (int)EMPConstants.Entity.SO;
                    int someEntity = (int)EMPConstants.Entity.SOME;
                    if (customer.EntityId == soEntity || customer.EntityId == someEntity)
                    {
                        var ids = db.emp_CustomerInformation.Where(x => x.StatusCode != EMPConstants.NewCustomer && (x.EntityId == someEntity || x.EntityId == soEntity)).Select(x => x.Id).ToList();
                        return ids;
                    }
                    if (customer.ParentId != null && customer.ParentId != Guid.Empty)
                    {
                        List<EntityHierarchyDTO> EntityHierarchyDTOs = GetEntityHierarchies(CustomerId);
                        if (EntityHierarchyDTOs.Count > 0)
                        {
                            var LevelOne = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                            if (LevelOne != null)
                            {
                                TopId = LevelOne.CustomerId ?? Guid.Empty;
                            }
                        }
                    }
                    customers.Add(TopId);
                    getChildrenIds(TopId, ref customers);
                }
            }
            catch(Exception)
            {

            }
            return customers.Distinct().ToList();
        }

        public void getChildrenIds(Guid CustomerId, ref List<Guid> customers)
        {
            try
            {
                db = new DatabaseEntities();
                var childs = db.emp_CustomerInformation.Where(x => x.ParentId == CustomerId && x.StatusCode != EMPConstants.NewCustomer).ToList();
                foreach (var item in childs)
                {
                    customers.Add(item.Id);
                    getChildrenIds(item.Id, ref customers);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}