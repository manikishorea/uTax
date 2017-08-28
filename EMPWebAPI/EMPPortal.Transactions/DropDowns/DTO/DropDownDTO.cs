using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;

namespace EMPPortal.Transactions.DropDowns.DTO
{
    public class DropDownDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class EntityDropDwonDTO : CoreModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class FeeEntityDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int FeeTypeID { get; set; }
        public int FeeCategoryID { get; set; }
        public bool IsEdit { get; set; }
        public int FeeFor { get; set; }
    }

    public class BankQuestionDTO : CoreModel
    {
        public System.Guid BankId { get; set; }
        public string BankName { get; set; }
        public string DocumentPath { get; set; }
        public decimal DesktopFee { get; set; }
        public decimal MSOFee { get; set; }
        public bool IsSameforAll { get; set; }
        public int ServiceorTransmission { get; set; }
        public List<DropDownDTO> Questions { get; set; }

        public decimal BankSVBDesktopFee { get; set; }
        public decimal BankSVBMSOFee { get; set; }

        public decimal BankTranDesktopFee { get; set; }
        public decimal BankTranMSOFee { get; set; }

        public decimal SubDesktopFee { get; set; }
        public decimal SubMSOFee { get; set; }

        public Guid QuestionId { get; set; }
    }




    public class TooltipDTO
    {
        public System.Guid Id { get; set; }
        public string Field { get; set; }
        public string Tooltip { get; set; }
        public string Description { get; set; }
    }

    public class BankDTO : CoreModel
    {
        public System.Guid BankId { get; set; }
        public string BankName { get; set; }
        public Nullable<decimal> FeeDesktop { get; set; }
        public Nullable<decimal> FeeMSO { get; set; }
        public Nullable<decimal> TranFeeDesktop { get; set; }
        public Nullable<decimal> TranFeeMSO { get; set; }
        public string DocumentPath { get; set; }
    }


    public class StatusCodeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayText { get; set; }
    }


    public class StateMasterDTO
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
    }
}
