using DataModel;
using DataModel.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BL
{
    public class PromotionGroupCodeServices : IPromotionGroupCodeServices
    {
        private readonly UnitOfWork _unitOfWork;

        public PromotionGroupCodeServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public List<PromotionGroupCode> GetAll()
        {
            List<PromotionGroupCode> groupCodeList = _unitOfWork.PromotionGroupCodeRepository.GetAll().OrderBy(x => x.PromotionGroupCode1).ToList();
            return groupCodeList;
        }
        public PromotionGroupCode GetByID(Guid PromotionGroupCodeOID)
        {
            PromotionGroupCode groupCode = _unitOfWork.PromotionGroupCodeRepository.GetFirstOrDefault(x => x.PromotionGroupCodeOID == PromotionGroupCodeOID);
            return groupCode;
        }
        public void SaveOrUpdate(PromotionGroupCode groupCode)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.PromotionGroupCodeRepository.GetFirstOrDefault(x => x.PromotionGroupCodeOID == groupCode.PromotionGroupCodeOID);
                if (check == null)
                {
                    _unitOfWork.PromotionGroupCodeRepository.Insert(groupCode);

                }
                else
                {
                    _unitOfWork.PromotionGroupCodeRepository.Update(groupCode);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }
    }
}
