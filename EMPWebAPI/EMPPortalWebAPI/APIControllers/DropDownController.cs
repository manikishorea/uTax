using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMPPortalWebAPI.Filters;
using EMPPortal.Transactions.DropDowns.DTO;
using EMPPortal.Transactions.DropDowns;
using System.Web.Http.Description;

namespace EMPPortalWebAPI.APIControllers
{
    [TokenAuthorization]
    public class DropDownController : ApiController
    {
        public DropDownService _DropDownService = new DropDownService();
        // GET: api/UserMasters

        [HttpGet]
        [ResponseType(typeof(DropDownDTO))]
        public IHttpActionResult GetDropDown(string id, string entityid = "", string bankid = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (id.Trim().ToLower() == "phonetype")
            {
                var data = _DropDownService.GetPhoneTypes();
                if (data == null)
                {
                    return NotFound();
                }
                return Ok(data);
            }

            if (id.Trim().ToLower() == "title")
            {
                var data = _DropDownService.GetTitles();
                if (data == null)
                {
                    return NotFound();
                }
                return Ok(data);
            }

            //Guid bankGuid = Guid.Empty;
            //Guid entityGuid = Guid.Empty; ;
            //if (!Guid.TryParse(entityid, out entityGuid) && !Guid.TryParse(bankid, out bankGuid))
            //{
            //    return NotFound();
            //}

            //if (id.Trim().ToLower() == "affiliate")
            //{
            //    var data = _DropDownService.GetAffiliateProgram(entityGuid);
            //    if (data == null)
            //    {
            //        return NotFound();
            //    }
            //    return Ok(data);
            //}

            //if (id.Trim().ToLower() == "bank")
            //{
            //    var data = _DropDownService.GetBankMaster(entityGuid);
            //    if (data == null)
            //    {
            //        return NotFound();
            //    }
            //    return Ok(data);
            //}


            //if (id.Trim().ToLower() == "banksubquestion")
            //{
            //    var data = _DropDownService.GetBankSubQuestionsMaster(bankGuid);
            //    if (data == null)
            //    {
            //        return NotFound();
            //    }
            //    return Ok(data);
            //}

            return Ok(false);
        }

        [HttpGet]
        [ActionName("title")]
        [ResponseType(typeof(DropDownDTO))]
        public IHttpActionResult title(string id = "")
        {
            var data = _DropDownService.GetTitles();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("Alternativetitle")]
        [ResponseType(typeof(DropDownDTO))]
        public IHttpActionResult Alternativetitle(string id = "")
        {
            var data = _DropDownService.GetAlternativeTitles();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("phonetype")]
        [ResponseType(typeof(DropDownDTO))]
        public IHttpActionResult phonetype(string id = "")
        {
            var data = _DropDownService.GetPhoneTypes();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("affiliate")]
        [ResponseType(typeof(DropDownDTO))]
        public IHttpActionResult affiliate(string entityid = "")
        {
            //if (string.IsNullOrEmpty(id))
            //{
            //    return NotFound();
            //}

            Guid bankGuid = Guid.Empty;
            int entityGuid = 0;
            if (!int.TryParse(entityid, out entityGuid))
            {
                return NotFound();
            }

            var data = _DropDownService.GetAffiliateProgram(entityGuid);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("affiliate_Sub")]
        [ResponseType(typeof(DropDownDTO))]
        public IHttpActionResult affiliateSub(string entityid = "", string CustomerID = "")
        {
            //if (string.IsNullOrEmpty(id))
            //{
            //    return NotFound();
            //}

            Guid customerGuid = Guid.Empty;
            int entityGuid = 0;
            if (!int.TryParse(entityid, out entityGuid))
            {
                return NotFound();
            }
            if (!Guid.TryParse(CustomerID, out customerGuid))
            {
                return NotFound();
            }
            var data = _DropDownService.GetAffiliateProgram(entityGuid, customerGuid);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("bank")]
        [ResponseType(typeof(DropDownDTO))]
        public IHttpActionResult bank(string entityid = "")
        {
            //if (string.IsNullOrEmpty(id))
            //{
            //    return NotFound();
            //}

            Guid bankGuid = Guid.Empty;
            int entityGuid = 0;
            if (!int.TryParse(entityid, out entityGuid))
            {
                return NotFound();
            }

            var data = _DropDownService.GetBankMaster(entityGuid).OrderBy(a => a.Name);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("banksubquestion")]
        [ResponseType(typeof(BankQuestionDTO))]
        public IHttpActionResult banksubquestion(string entityid = "")
        {
            int entityGuid = 0;
            if (!int.TryParse(entityid, out entityGuid))
            {
                return NotFound();
            }

            var data = _DropDownService.GetBankAndQuestions(entityGuid);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("banksubquestionForSelection")]
        [ResponseType(typeof(BankQuestionDTO))]
        public IHttpActionResult banksubquestionForSelection(string entityid = "", string CustomerId = "")
        {
            int _Entityid = 0;

            if (!int.TryParse(entityid, out _Entityid))
            {
                return NotFound();
            }

            Guid _CustomerGuid = Guid.Empty; ;
            if (!Guid.TryParse(CustomerId, out _CustomerGuid))
            {
                return NotFound();
            }

            var data = _DropDownService.GetBankAndQuestionsForSelection(_Entityid, _CustomerGuid);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("Fees")]
        [ResponseType(typeof(FeeEntityDTO))]
        public IHttpActionResult Fees(string entityid = "", string userid = "")
        {
            int entityGuid = 0;
            if (!int.TryParse(entityid, out entityGuid))
            {
                return NotFound();
            }

            Guid userGuid = Guid.Empty; ;
            if (!Guid.TryParse(userid, out userGuid))
            {
                return NotFound();
            }

            var data = _DropDownService.GetFeeMaster(entityGuid, userGuid);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("tooltip")]
        [ResponseType(typeof(TooltipDTO))]
        public IHttpActionResult tooltip(string sitemapid = "")
        {
            Guid sitemapGuid = Guid.Empty; ;
            if (!Guid.TryParse(sitemapid, out sitemapGuid))
            {
                return NotFound();
            }

            var data = _DropDownService.GetTooltip(sitemapGuid);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("userbanks")]
        [ResponseType(typeof(BankDTO))]
        public IHttpActionResult UserBanks(string UserId = "")
        {
            Guid UserGuid = Guid.Empty; ;
            if (!Guid.TryParse(UserId, out UserGuid))
            {
                return NotFound();
            }

            var data = _DropDownService.GetUserBanks(UserGuid);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("subsitebank")]
        [ResponseType(typeof(BankQuestionDTO))]
        public IHttpActionResult banksubsitebankquestion(string id = "")
        {
            Guid entityGuid = Guid.Empty;
            if (!Guid.TryParse(id, out entityGuid))
            {
                return NotFound();
            }
            var data = _DropDownService.GetSubSiteBankAndQuestions(entityGuid);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        ///// <summary>
        ///// This method is used to get the sub site office fee config details by id
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[ActionName("AllSalesYears")]
        //[ResponseType(typeof(IQueryable<SalesYearDTO>))]
        //public IHttpActionResult GetAllSalesYears(string Id)
        //{
        //    Guid GUserId = Guid.Empty;
        //    if (!Guid.TryParse(Id, out GUserId))
        //    {
        //        return NotFound();
        //    }

        //    var result = _DropDownService.GetSalesYearsForArchive(GUserId);
        //    return Ok(result);
        //}

        [HttpGet]
        [ActionName("Status")]
        [ResponseType(typeof(StatusCodeDTO))]
        public IHttpActionResult GetStatus()
        {
            var data = _DropDownService.GetStatusCode();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("EnrollmentStatus")]
        [ResponseType(typeof(StatusCodeDTO))]
        public IHttpActionResult GetEnrollmentStatus()
        {
            var data = _DropDownService.GetEnrollmentStatus();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("OnBoardingStatus")]
        [ResponseType(typeof(StatusCodeDTO))]
        public IHttpActionResult GetOnBoardingStatus()
        {
            var data = _DropDownService.GetOnBoardingStatus();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        [ActionName("Entities")]
        [ResponseType(typeof(DropDownDTO))]
        public IHttpActionResult GetEntities()
        {
            var data = _DropDownService.GetEntities();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }


        [HttpGet]
        [ActionName("States")]
        [ResponseType(typeof(StateMasterDTO))]
        public IHttpActionResult GetStateMaster()
        {
            var data = _DropDownService.GetStateMasterList();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }


        [ActionName("Hierarchy")]
        [ResponseType(typeof(IQueryable<HierarchyDTO>))]
        public IHttpActionResult GetCustomerconfigStatus(string id)
        {
            Guid uId;
            if (!Guid.TryParse(id, out uId))
            {
                return NotFound();
            }

            var result = _DropDownService.GetHierarchyList(uId);
            return Ok(result);
        }


        [ActionName("EntityHierarchy")]
        [ResponseType(typeof(IQueryable<EntityHierarchyDTO>))]
        public IHttpActionResult GetEntityHierarchy(string id)
        {
            Guid uId;
            if (!Guid.TryParse(id, out uId))
            {
                return NotFound();
            }

            var result = _DropDownService.GetEntityHierarchy(uId);
            return Ok(result);
        }

        [HttpGet]
        [ActionName("EFINStatus")]
        [ResponseType(typeof(StatusCodeDTO))]
        public IHttpActionResult GetStatus(int? entityid)
        {
            int xentityid = entityid ?? 0;
            if (xentityid > 0)
            {
                var data = _DropDownService.GetEFINStatus(xentityid);
                if (data == null)
                {
                    return NotFound();
                }
                return Ok(data);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
