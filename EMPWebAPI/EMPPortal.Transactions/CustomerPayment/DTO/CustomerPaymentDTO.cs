using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.CustomerPayment.DTO
{
    public class CustomerPaymentDTO
    {
    }

    public class CustomerPaymentInfoSummary
    {
        public List<CustomerPaymentInfo> PaymentOptions { get; set; }
        public bool status { get; set; }
    }

    public class CustomerPaymentInfo
    {
        public int PaymentType { get; set; }
        public int IsSameBankAccount { get; set; }
        public List<FeeSummary> Fees { get; set; }
        public PaymentCreditCardInfo CreditCard { get; set; }
        public PaymetnACH ACH { get; set; }
        public bool status { get; set; }
        public string UserId { get; set; }
        public string Id { get; set; }
        public CustomerReimBankDetails BankDetails { get; set; }
        public int SiteType { get; set; }
        public bool IsFeeReimbursement { get; set; }
        public bool IsEnrollment { get; set; }        
        public Guid BankId { get; set; }
        public bool IsonHold { get; set; }
    }

    public class FeeSummary
    {
        public string Fee { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentCreditCardInfo
    {
        public string CardHolderName { get; set; }
        public int CardType { get; set; }
        public string Address { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public int StateId { get; set; }
        public string ZipCode { get; set; }
        public bool status { get; set; }
        public Guid UserId { get; set; }
        public Guid PaymentOptionId { get; set; }
    }

    public class PaymetnACH
    {
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string RTN { get; set; }
        public int AccountType { get; set; }
        public bool status { get; set; }
        public Guid UserId { get; set; }
        public Guid PaymentOptionId { get; set; }
    }

    public class PaymentOptionResponse
    {
        public bool status { get; set; }
        public string Id { get; set; }
    }

    public class CustomerReimBankDetails
    {
        public string BankName { get; set; }
        public string Status { get; set; }
        public bool Availble { get; set; }
    }
}
