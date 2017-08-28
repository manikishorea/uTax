using EMPPortal.Transactions.CustomerPayment.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.CustomerPayment
{
    interface ICustomerPaymentOptionsService
    {
        Task<CustomerPaymentInfo> GetCustomerPaymentInfo(Guid UserId,int EntityId,int SiteType);
        Task<CustomerPaymentInfoSummary> GetCustomerPaymentInfoSummary(Guid UserId, int EntityId);        
        Task<PaymentOptionResponse> Save(CustomerPaymentInfo Info);
        Task<PaymetnACH> GetCustomerBankDetails(Guid UserId, int EntityId,Guid CustId);
        Task<bool> SaveCreditCardDetails(PaymentCreditCardInfo Info);
        Task<bool> SaveACHDetails(PaymetnACH Info);
    }
}
