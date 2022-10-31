using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using DataModel.UnitOfWork;
using System.Transactions;

namespace BL
{
    public class AirPromotionServices : IAirPromotionServices
    {
        private readonly UnitOfWork _unitOfWork;
        public AirPromotionServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void Delete(Guid id)
        {
            using (var scope = new TransactionScope())
            {
                _unitOfWork.AirPromotionRepository.Delete(id);
                _unitOfWork.Save();
                scope.Complete();
            }
        }

        public List<AirPromotion> GetAll()
        {
            return _unitOfWork.AirPromotionRepository.GetAll().ToList();
        }

        public AirPromotion GetById(Guid id)
        {
            return _unitOfWork.AirPromotionRepository.GetFirstOrDefault(x => x.AirPromotionOID == id);
        }

        public void SaveOrUpdate(AirPromotion airPromotion)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.AirPromotionRepository.GetFirstOrDefault(x => x.AirPromotionOID == airPromotion.AirPromotionOID);
                if (check == null)
                {
                    _unitOfWork.AirPromotionRepository.Insert(airPromotion);
                }
                else
                {
                    _unitOfWork.AirPromotionRepository.Update(airPromotion);
                }
                _unitOfWork.Save();
                scope.Complete();
            }
        }
    }
}
