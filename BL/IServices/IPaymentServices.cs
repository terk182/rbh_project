using BL.Entities;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IPaymentServices
    {

        //List<Payment> GetAll();
        Payment GetAll();
        Payment GetByID(Guid id);
        void SaveOrUpdate(Payment paymment);

        List<PaymentReference> GetAllPaymentReference();
        PaymentReference GetPaymentReferenceByID(Guid PaymentRefID);
        PaymentReference GetPaymentReferenceByOrderNo(string orderNo, string paymentType);
        void SaveOrUpdatePaymentReference(PaymentReference paymentReference);
        ChillPayBackground GetChillPayBackgroundByOrderNo(string orderNo);
        void SaveOrUpdateChillPayBackground(ChillPayBackground chillPayBackground);
        void SaveOrUpdateChillPayResult(ChillPayResult chillPayResult);
        void SaveOrUpdateChillPayInquiry(ChillPayInquiry chillPayInquiry);
        void SavePaymentLog(PaymentLog paymentLog);
    }
}