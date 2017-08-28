using EMPEntityFramework.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.CustomerPayment.DTO;
using EMP.Core.Utilities;

namespace EMPPortal.Transactions.CustomerPayment
{
    public class CustomerPaymentOptionsService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public CustomerPaymentInfo GetCustomerPaymentInfo(Guid UserId, int EntityId, int SiteType, Guid BankId)
        {
            CustomerPaymentInfo info = new CustomerPaymentInfo();
            try
            {
                bool _isnonebank = false;
                var _banksel = db.EnrollmentBankSelections.Where(x => x.CustomerId == UserId && x.StatusCode == EMPConstants.Active && x.BankId == BankId).FirstOrDefault();
                if (_banksel != null)
                {
                    _isnonebank = _banksel.BankId == Guid.Empty;
                }

                info.IsEnrollment = _isnonebank;
                info.IsonHold = db.emp_CustomerInformation.Where(x => x.Id == UserId).Select(x => x.IsHold).FirstOrDefault() ?? false;

                var IsExist = (from pay in db.CustomerPaymentOptions
                               where pay.CustomerId == UserId && pay.SiteType == SiteType && pay.BankId == BankId
                               select new { pay }).FirstOrDefault();
                if (IsExist != null)
                {
                    info.Id = IsExist.pay.Id.ToString();
                    info.IsSameBankAccount = IsExist.pay.IsSameasBankAccount ?? 0;
                    info.PaymentType = IsExist.pay.PaymentType ?? 0;
                    info.SiteType = IsExist.pay.SiteType ?? 0;
                    if (info.PaymentType == 1)
                    {
                        PaymentCreditCardInfo CardInfo = new PaymentCreditCardInfo();
                        var carddetails = (from card in db.CustomerPaymentViaCreditCards
                                           where card.PaymentOptionId == IsExist.pay.Id
                                           select card).FirstOrDefault();
                        if (carddetails != null)
                        {
                            CardInfo.Address = carddetails.BillingAddress;
                            CardInfo.CardHolderName = carddetails.CardHolderName;
                            CardInfo.CardNumber = carddetails.CardNumber;
                            CardInfo.CardType = carddetails.Cardtype ?? 0;
                            CardInfo.City = carddetails.City;
                            CardInfo.Expiration = carddetails.Expiration;
                            CardInfo.StateCode = db.StateMasters.Where(x => x.StateID == carddetails.State).Select(x => x.StateCode).FirstOrDefault();
                            CardInfo.StateId = carddetails.State ?? 0;
                            CardInfo.status = true;
                            CardInfo.ZipCode = carddetails.ZipCode;
                        }
                        else
                            CardInfo.status = false;
                        info.CreditCard = CardInfo;
                    }
                    else if (info.PaymentType == 2)
                    {
                        PaymetnACH ACH = new PaymetnACH();
                        var ach = (from s in db.CustomerPaymentViaACHes
                                   where s.PaymentOptionId == IsExist.pay.Id
                                   select s).FirstOrDefault();
                        if (ach != null)
                        {
                            ACH.status = true;
                            ACH.AccountName = ach.AccountName;
                            ACH.AccountNumber = ach.AccountNumber;
                            ACH.AccountType = ach.AccountType ?? 0;
                            ACH.BankName = ach.BankName;
                            ACH.RTN = ach.RTN;
                        }
                        else
                            ACH.status = false;
                        info.ACH = ACH;
                    }
                }
                else
                {
                    var oldData = (from pay in db.CustomerPaymentOptions
                                   where pay.CustomerId == UserId && pay.SiteType == SiteType
                                   orderby pay.UpdatedDate descending
                                   select new { pay }).FirstOrDefault();
                    if (oldData != null)
                    {
                        info.Id = Guid.Empty.ToString();
                        info.IsSameBankAccount = oldData.pay.IsSameasBankAccount ?? 0;
                        info.PaymentType = oldData.pay.PaymentType ?? 0;
                        info.SiteType = oldData.pay.SiteType ?? 0;
                        if (info.PaymentType == 1)
                        {
                            PaymentCreditCardInfo CardInfo = new PaymentCreditCardInfo();
                            var carddetails = (from card in db.CustomerPaymentViaCreditCards
                                               where card.PaymentOptionId == oldData.pay.Id
                                               select card).FirstOrDefault();
                            if (carddetails != null)
                            {
                                CardInfo.Address = carddetails.BillingAddress;
                                CardInfo.CardHolderName = carddetails.CardHolderName;
                                CardInfo.CardNumber = carddetails.CardNumber;
                                CardInfo.CardType = carddetails.Cardtype ?? 0;
                                CardInfo.City = carddetails.City;
                                CardInfo.Expiration = carddetails.Expiration;
                                CardInfo.StateCode = db.StateMasters.Where(x => x.StateID == carddetails.State).Select(x => x.StateCode).FirstOrDefault();
                                CardInfo.StateId = carddetails.State ?? 0;
                                CardInfo.status = true;
                                CardInfo.ZipCode = carddetails.ZipCode;
                            }
                            else
                                CardInfo.status = false;
                            info.CreditCard = CardInfo;
                        }
                        else if (info.PaymentType == 2)
                        {
                            PaymetnACH ACH = new PaymetnACH();
                            var ach = (from s in db.CustomerPaymentViaACHes
                                       where s.PaymentOptionId == oldData.pay.Id
                                       select s).FirstOrDefault();
                            if (ach != null)
                            {
                                ACH.status = true;
                                ACH.AccountName = ach.AccountName;
                                ACH.AccountNumber = ach.AccountNumber;
                                ACH.AccountType = ach.AccountType ?? 0;
                                ACH.BankName = ach.BankName;
                                ACH.RTN = ach.RTN;
                            }
                            else
                                ACH.status = false;
                            info.ACH = ACH;
                        }
                    }
                }

                if (SiteType == 1)
                {
                    List<FeeSummary> fess = new List<FeeSummary>();
                    var custinfo = db.emp_CustomerInformation.Where(x => x.Id == UserId).FirstOrDefault();
                    if (custinfo != null)
                    {
                        fess.Add(new FeeSummary { Amount = custinfo.Federal_EF_Fee_New__c ?? 0, Fee = "uTax Federal e-File Fee" });
                        fess.Add(new FeeSummary { Amount = custinfo.State_EF_Fee_New__c ?? 0, Fee = "uTax State e-File Fee" });
                    }
                    info.Fees = fess;
                }
                else if (SiteType == 2)
                {
                    List<FeeSummary> fess = new List<FeeSummary>();
                    var custinfo = db.emp_CustomerInformation.Where(x => x.Id == UserId).FirstOrDefault();
                    if (custinfo != null)
                    {
                        fess.Add(new FeeSummary { Amount = custinfo.pymt__Balance__c ?? 0, Fee = "Cash Saver" });
                        fess.Add(new FeeSummary { Amount = custinfo.Total_Amount_Loaned__c ?? 0, Fee = "LOC Program Participant" });
                        fess.Add(new FeeSummary { Amount = custinfo.A_R_Amount_Due_Credit__c ?? 0, Fee = "A/R Amount Due Credit" });
                    }
                    info.Fees = fess;
                }

                //var custid = db.emp_CustomerLoginInformation.Where(x => x.Id == UserId).Select(x => x.CustomerOfficeId).FirstOrDefault();
                //var fees = (from fee in db.CustomerAssociatedFees
                //            join fm in db.FeeMasters on fee.FeeMaster_ID equals fm.Id
                //            join map in db.FeeEntityMaps on fm.Id equals map.FeeId
                //            where fee.emp_CustomerInformation_ID == UserId && fee.IsActive == true &&
                //            fm.FeeCategoryID == EMPConstants.eFileFeeCategory && map.EntityId == EntityId && fm.StatusCode == EMPConstants.Active
                //            select new FeeSummary
                //            {
                //                Amount = fee.Amount,
                //                Fee = fm.Name
                //            }).ToList();
                //info.Fees = fees;

                CustomerReimBankDetails bankinfo = new CustomerReimBankDetails();
                var bankdetails = (from bank in db.EnrollmentFeeReimbursementConfigs
                                   where bank.emp_CustomerInformation_ID == UserId && bank.StatusCode == EMPConstants.Active && bank.BankId == BankId
                                   select bank).FirstOrDefault();
                if (bankdetails != null)
                {
                    bankinfo.BankName = bankdetails.BankName;
                    bankinfo.Status = bankdetails.StatusCode;
                    bankinfo.Availble = true;
                }
                else
                {
                    var mobankdetails = (from bank in db.FeeReimbursementConfigs
                                         where bank.emp_CustomerInformation_ID == UserId && bank.StatusCode == EMPConstants.Active
                                         select bank).FirstOrDefault();
                    if (mobankdetails != null)
                    {
                        bankinfo.BankName = mobankdetails.BankName;
                        bankinfo.Status = mobankdetails.StatusCode;
                        bankinfo.Availble = true;
                    }
                    else
                    {
                        bankinfo.Availble = false;
                        bankinfo.BankName = "";
                        bankinfo.Status = "";
                    }
                }

                info.BankDetails = bankinfo;
                Guid feereium = new Guid("60025459-7568-4a77-b152-f81904aaaa63"); // Main office fee-reimbursement screen
                Guid ssfeereium = new Guid("a55334d1-3960-44c4-8cf1-e3ba9901f2be"); // Enrollment fee-reimbursement screen
                var issaved = db.CustomerConfigurationStatus.Where(x => x.CustomerId == UserId && x.StatusCode == EMPConstants.Done && x.SitemapId == feereium).FirstOrDefault();
                //var enrissaved = db.CustomerConfigurationStatus.Where(x => x.CustomerId == UserId && x.StatusCode == EMPConstants.Done && x.SitemapId == ssfeereium).FirstOrDefault();
                info.IsFeeReimbursement = (issaved == null && bankdetails == null) ? false : true;
                if ((EntityId == (int)EMPConstants.Entity.SO || EntityId == (int)EMPConstants.Entity.SOME) && info.IsFeeReimbursement)
                {
                    var banksel = db.EnrollmentBankSelections.Where(x => x.CustomerId == UserId && x.StatusCode == EMPConstants.Active && x.BankId == BankId).FirstOrDefault();
                    if (banksel != null)
                    {
                        if (!(banksel.IsServiceBureauFee ?? false) && !(banksel.IsTransmissionFee ?? false))
                            info.IsFeeReimbursement = false;
                    }
                    else
                        info.IsFeeReimbursement = false;
                }

                if (!info.IsFeeReimbursement)
                {
                    var bankdetails1 = (from bank in db.EnrollmentFeeReimbursementConfigs
                                        where bank.emp_CustomerInformation_ID == UserId && bank.StatusCode == EMPConstants.Active
                                        select bank).FirstOrDefault();
                    if (bankdetails1 != null)
                    {
                        info.IsFeeReimbursement = true;
                    }
                }

                info.status = true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/GetCustomerPaymentInfo", UserId);
                info.status = false;
            }
            return info;
        }

        public PaymentOptionResponse Save(CustomerPaymentInfo info)
        {
            PaymentOptionResponse res = new PaymentOptionResponse();
            try
            {
                CustomerPaymentOption CPO = new CustomerPaymentOption();
                bool isNew = true;

                Guid userid, oldid;
                if (Guid.TryParse(info.Id, out oldid) && info.Id != Guid.Empty.ToString())
                {
                    CPO = db.CustomerPaymentOptions.Where(a => a.Id == oldid).FirstOrDefault();
                    if (CPO != null)
                    {
                        isNew = false;
                    }
                    else { res.status = false; }
                }
                else
                {
                    CPO.Id = Guid.NewGuid();
                }
                bool IsRefId = Guid.TryParse(info.UserId, out userid);

                CPO.IsSameasBankAccount = info.IsSameBankAccount;
                CPO.PaymentType = info.PaymentType;
                if (isNew)
                {
                    Guid CustId;
                    var customerid = db.emp_CustomerLoginInformation.Where(x => x.Id == userid).Select(x => x.CustomerOfficeId).FirstOrDefault();
                    bool IsCustId = Guid.TryParse(customerid.ToString(), out CustId);

                    CPO.CreatedBy = userid;
                    CPO.BankId = info.BankId;
                    CPO.CreatedDate = DateTime.Now;
                    CPO.CustomerId = userid;
                    CPO.SiteType = info.SiteType;
                    db.CustomerPaymentOptions.Add(CPO);
                }
                else
                {
                    CPO.BankId = info.BankId;
                    CPO.UpdatedBy = userid;
                    CPO.UpdatedDate = DateTime.Now;
                    if (info.IsSameBankAccount == 2 && info.PaymentType == 2)
                    {
                        var ach = (from s in db.CustomerPaymentViaACHes
                                   where s.PaymentOptionId == CPO.Id
                                   select s).FirstOrDefault();
                        if (ach != null)
                        {
                            db.CustomerPaymentViaACHes.Remove(ach);
                        }
                    }
                }
                db.SaveChanges();
                db.Dispose();
                res.status = true;
                res.Id = CPO.Id.ToString();

            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/Save", Guid.Empty);
                res.status = false;
            }
            return res;
        }

        public PaymetnACH GetCustomerBankDetails(Guid UserId, int EntityId, Guid CustId, Guid BankId)
        {
            PaymetnACH info = new PaymetnACH();
            try
            {
                var IsExist = (from bank in db.EnrollmentFeeReimbursementConfigs
                               where bank.emp_CustomerInformation_ID == CustId && bank.StatusCode == EMPConstants.Active && bank.BankId == BankId
                               select bank).FirstOrDefault();
                if (IsExist != null)
                {
                    info.AccountName = IsExist.AccountName;
                    info.AccountNumber = IsExist.BankAccountNo;
                    info.AccountType = IsExist.AccountType;
                    info.BankName = IsExist.BankName;
                    info.RTN = IsExist.RTN;
                    info.status = true;
                }
                else
                {
                    var moIsExist = (from bank in db.FeeReimbursementConfigs
                                     where bank.emp_CustomerInformation_ID == CustId && bank.StatusCode == EMPConstants.Active
                                     select bank).FirstOrDefault();
                    if (moIsExist != null)
                    {
                        info.AccountName = moIsExist.AccountName;
                        info.AccountNumber = moIsExist.BankAccountNo;
                        info.AccountType = moIsExist.AccountType;
                        info.BankName = moIsExist.BankName;
                        info.RTN = moIsExist.RTN;
                        info.status = true;
                    }
                    else
                        info.status = false;
                }
                if (!info.status)
                {
                    var IsExist1 = (from bank in db.EnrollmentFeeReimbursementConfigs
                                    where bank.emp_CustomerInformation_ID == CustId && bank.StatusCode == EMPConstants.Active
                                    select bank).FirstOrDefault();
                    if (IsExist1 != null)
                    {
                        info.AccountName = IsExist1.AccountName;
                        info.AccountNumber = IsExist1.BankAccountNo;
                        info.AccountType = IsExist1.AccountType;
                        info.BankName = IsExist1.BankName;
                        info.RTN = IsExist1.RTN;
                        info.status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/GetCustomerBankDetails", UserId);
                info.status = false;
            }
            return info;
        }

        public bool SaveCreditCardDetails(PaymentCreditCardInfo Info)
        {
            try
            {
                var isexist = db.CustomerPaymentViaCreditCards.Where(x => x.PaymentOptionId == Info.PaymentOptionId).FirstOrDefault();
                if (isexist != null)
                {
                    isexist.BillingAddress = Info.Address;
                    isexist.CardHolderName = Info.CardHolderName;
                    isexist.CardNumber = Info.CardNumber;
                    isexist.Cardtype = Info.CardType;
                    isexist.City = Info.City;
                    isexist.Expiration = Info.Expiration;
                    isexist.State = Info.StateId;
                    isexist.UpdatedBy = Info.UserId;
                    isexist.UpdatedDate = DateTime.Now;
                    isexist.ZipCode = Info.ZipCode;
                }
                else
                {
                    CustomerPaymentViaCreditCard cc = new CustomerPaymentViaCreditCard();
                    cc.BillingAddress = Info.Address;
                    cc.CardHolderName = Info.CardHolderName;
                    cc.CardNumber = Info.CardNumber;
                    cc.Cardtype = Info.CardType;
                    cc.City = Info.City;
                    cc.Expiration = Info.Expiration;
                    cc.State = Info.StateId;
                    cc.ZipCode = Info.ZipCode;
                    cc.CreatdedBy = Info.UserId;
                    cc.CreatedDate = DateTime.Now;
                    cc.Id = Guid.NewGuid();
                    cc.PaymentOptionId = Info.PaymentOptionId;
                    db.CustomerPaymentViaCreditCards.Add(cc);
                    db.SaveChanges();

                    //var data = db.CustomerPaymentOptions.Where(x => x.Id == Info.PaymentOptionId).FirstOrDefault();
                    //if (data != null)
                    //{
                    //    CustomerConfigurationStatu stat = new CustomerConfigurationStatu();
                    //    stat.CustomerId = data.CustomerId;
                    //    stat.Id = Guid.NewGuid();
                    //    stat.SitemapId = data.SiteType == 1 ? new Guid("0eda5d25-591c-4e01-a845-fb580572cff5") : new Guid("0eda5d25-591c-4e01-a845-fb580572cfe8");
                    //    stat.StatusCode = EMPConstants.Done;
                    //    stat.UpdatedBy = Info.UserId;
                    //    stat.UpdatedDate = DateTime.Now;
                    //    db.CustomerConfigurationStatus.Add(stat);
                    //    db.SaveChanges();
                    //}
                }
                return true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/SaveCreditCardDetails", Guid.Empty);
                return false;
            }
        }

        public bool SaveACHDetails(PaymetnACH Info)
        {
            try
            {
                var isexist = db.CustomerPaymentViaACHes.Where(x => x.PaymentOptionId == Info.PaymentOptionId).FirstOrDefault();
                if (isexist != null)
                {
                    isexist.AccountName = Info.AccountName;
                    isexist.AccountNumber = Info.AccountNumber;
                    isexist.AccountType = Info.AccountType;
                    isexist.BankName = Info.BankName;
                    isexist.RTN = Info.RTN;
                    isexist.UpdatedBy = Info.UserId;
                    isexist.UpdatedDate = DateTime.Now;
                }
                else
                {
                    CustomerPaymentViaACH ach = new CustomerPaymentViaACH();
                    ach.AccountName = Info.AccountName;
                    ach.AccountNumber = Info.AccountNumber;
                    ach.AccountType = Info.AccountType;
                    ach.BankName = Info.BankName;
                    ach.RTN = Info.RTN;
                    ach.CreatdedBy = Info.UserId;
                    ach.CreatedDate = DateTime.Now;
                    ach.Id = Guid.NewGuid();
                    ach.PaymentOptionId = Info.PaymentOptionId;
                    db.CustomerPaymentViaACHes.Add(ach);

                    //var data = db.CustomerPaymentOptions.Where(x => x.Id == Info.PaymentOptionId).FirstOrDefault();
                    //if (data != null)
                    //{
                    //    CustomerConfigurationStatu stat = new CustomerConfigurationStatu();
                    //    stat.CustomerId = data.CustomerId;
                    //    stat.Id = Guid.NewGuid();
                    //    stat.SitemapId = data.SiteType == 1 ? new Guid("0eda5d25-591c-4e01-a845-fb580572cff5") : new Guid("0eda5d25-591c-4e01-a845-fb580572cfe8");
                    //    stat.StatusCode = EMPConstants.Done;
                    //    stat.UpdatedBy = Info.UserId;
                    //    stat.UpdatedDate = DateTime.Now;
                    //    db.CustomerConfigurationStatus.Add(stat);
                    //    db.SaveChanges();
                    //}
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/SaveACHDetails", Guid.Empty);
                return false;
            }
        }

        public CustomerPaymentInfoSummary GetCustomerPaymentInfoSummary(Guid UserId, int EntityId)
        {
            CustomerPaymentInfoSummary res = new CustomerPaymentInfoSummary();
            List<CustomerPaymentInfo> infoLst = new List<CustomerPaymentInfo>();
            try
            {
                var payments = (from pay in db.CustomerPaymentOptions
                                join user in db.emp_CustomerLoginInformation on pay.CustomerId equals user.CustomerOfficeId
                                where user.Id == UserId
                                select new { pay, user.CustomerOfficeId }).ToList();
                foreach (var item in payments)
                {
                    CustomerPaymentInfo info = new CustomerPaymentInfo();
                    info.Id = item.pay.Id.ToString();
                    info.status = true;
                    info.IsSameBankAccount = item.pay.IsSameasBankAccount ?? 0;
                    info.PaymentType = item.pay.PaymentType ?? 0;
                    info.SiteType = item.pay.SiteType ?? 0;
                    var fees = (from fee in db.CustomerAssociatedFees
                                join fm in db.FeeMasters on fee.FeeMaster_ID equals fm.Id
                                join map in db.FeeEntityMaps on fm.Id equals map.FeeId
                                where fee.emp_CustomerInformation_ID == item.CustomerOfficeId && fee.IsActive == true &&
                                fm.FeeCategoryID == EMPConstants.eFileFeeCategory && map.EntityId == EntityId && fm.StatusCode == EMPConstants.Active
                                select new FeeSummary
                                {
                                    Amount = fee.Amount,
                                    Fee = fm.Name
                                }).ToList();
                    info.Fees = fees;
                    if (info.PaymentType == 1)
                    {
                        PaymentCreditCardInfo CardInfo = new PaymentCreditCardInfo();
                        var carddetails = (from card in db.CustomerPaymentViaCreditCards
                                           where card.PaymentOptionId == item.pay.Id
                                           select card).FirstOrDefault();
                        if (carddetails != null)
                        {
                            CardInfo.Address = carddetails.BillingAddress;
                            CardInfo.CardHolderName = carddetails.CardHolderName;
                            CardInfo.CardNumber = carddetails.CardNumber;
                            CardInfo.CardType = carddetails.Cardtype ?? 0;
                            CardInfo.City = carddetails.City;
                            CardInfo.Expiration = carddetails.Expiration;
                            CardInfo.StateCode = db.StateMasters.Where(x => x.StateID == carddetails.State).Select(x => x.StateCode).FirstOrDefault();
                            CardInfo.StateId = carddetails.State ?? 0;
                            CardInfo.status = true;
                            CardInfo.ZipCode = carddetails.ZipCode;
                        }
                        else
                            CardInfo.status = false;
                        info.CreditCard = CardInfo;
                    }
                    else if (info.PaymentType == 2)
                    {
                        PaymetnACH ACH = new PaymetnACH();
                        var ach = (from s in db.CustomerPaymentViaACHes
                                   where s.PaymentOptionId == item.pay.Id
                                   select s).FirstOrDefault();
                        if (ach != null)
                        {
                            ACH.status = true;
                            ACH.AccountName = ach.AccountName;
                            ACH.AccountNumber = ach.AccountNumber;
                            ACH.AccountType = ach.AccountType ?? 0;
                            ACH.BankName = ach.BankName;
                            ACH.RTN = ach.RTN;
                        }
                        else
                            ACH.status = false;
                        info.ACH = ACH;
                    }
                    infoLst.Add(info);
                }
                res.PaymentOptions = infoLst;
                res.status = true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/GetCustomerPaymentInfoSummary", UserId);
                res.status = false;
            }
            return res;
        }
    }
}
