using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DataModel;
using DataModel.UnitOfWork;

namespace BL
{
    public class PaymentServices : IPaymentServices
    {
        private readonly UnitOfWork _unitOfWork;

        public PaymentServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public List<Payment> GetAll()
        //{
        //    return _unitOfWork.PaymentRepository.GetMany(x => x.IsDelete == false).ToList();
        //}

        public Payment GetAll()
        {
            return _unitOfWork.PaymentRepository.GetFirstOrDefault(x => x.IsDelete == false);
        }

        public Payment GetByID(Guid paymentOID)
        {
            return _unitOfWork.PaymentRepository.GetFirstOrDefault(x => x.PaymentOID == paymentOID && x.IsDelete == false);
        }

        public void SaveOrUpdate(Payment payment)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.PaymentRepository.GetFirstOrDefault(x => x.PaymentOID == payment.PaymentOID);
                if (check == null)
                {
                    _unitOfWork.PaymentRepository.Insert(payment);
                }
                else
                {
                    _unitOfWork.PaymentRepository.Update(payment);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }

        public List<PaymentReference> GetAllPaymentReference()
        {
            return _unitOfWork.PaymentReferenceRepository.GetAll().ToList();
        }

        public PaymentReference GetPaymentReferenceByID(Guid PaymentRefID)
        {
            return _unitOfWork.PaymentReferenceRepository.GetFirstOrDefault(x => x.PaymentRefID == PaymentRefID);
        }


        public PaymentReference GetPaymentReferenceByOrderNo(string orderNo, string paymentType)
        {
            return _unitOfWork.PaymentReferenceRepository.GetFirstOrDefault(x => x.PaymentRefOrderNo == orderNo && x.PaymentType == paymentType);
        }

        public void SaveOrUpdatePaymentReference(PaymentReference paymentReference)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.PaymentReferenceRepository.GetFirstOrDefault(x => x.PaymentRefID == paymentReference.PaymentRefID);
                if (check == null)
                {
                    _unitOfWork.PaymentReferenceRepository.Insert(paymentReference);
                }
                else
                {
                    _unitOfWork.PaymentReferenceRepository.Update(paymentReference);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }

        public ChillPayBackground GetChillPayBackgroundByOrderNo(string orderNo)
        {
            return _unitOfWork.ChillPayBackgroundRepository.GetFirstOrDefault(x => x.OrderNo == orderNo);
        }

        public void SaveOrUpdateChillPayBackground(ChillPayBackground chillPayBackground)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.ChillPayBackgroundRepository.GetFirstOrDefault(x => x.ChillPayBackgroundID == chillPayBackground.ChillPayBackgroundID);
                if (check == null)
                {
                    _unitOfWork.ChillPayBackgroundRepository.Insert(chillPayBackground);
                }
                else
                {
                    _unitOfWork.ChillPayBackgroundRepository.Update(chillPayBackground);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }

        public void SaveOrUpdateChillPayResult(ChillPayResult chillPayResult)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.ChillPayResultRepository.GetFirstOrDefault(x => x.ChillPayResultID == chillPayResult.ChillPayResultID);
                if (check == null)
                {
                    _unitOfWork.ChillPayResultRepository.Insert(chillPayResult);
                }
                else
                {
                    _unitOfWork.ChillPayResultRepository.Update(chillPayResult);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }

        public void SaveOrUpdateChillPayInquiry(ChillPayInquiry chillPayInquiry)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.ChillPayInquiryRepository.GetFirstOrDefault(x => x.ChillPayInquiryID == chillPayInquiry.ChillPayInquiryID);
                if (check == null)
                {
                    _unitOfWork.ChillPayInquiryRepository.Insert(chillPayInquiry);
                }
                else
                {
                    _unitOfWork.ChillPayInquiryRepository.Update(chillPayInquiry);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }

        public void SavePaymentLog(PaymentLog paymentLog)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.PaymentLogRepository.GetFirstOrDefault(x => x.PaymentLogOID == paymentLog.PaymentLogOID);
                if (check == null)
                {
                    _unitOfWork.PaymentLogRepository.Insert(paymentLog);
                }
                else
                {
                    _unitOfWork.PaymentLogRepository.Update(paymentLog);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }

        public PaymentLog GetPaymentLog(string RobinhoodID)
        {
            return _unitOfWork.PaymentLogRepository.GetFirstOrDefault(x => x.RobinhoodID== RobinhoodID);
        }

    }
}
