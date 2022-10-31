using BL.Entities.DiscountTag;
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
    public class DiscountTagServices : IDiscountTagServices
    {
        private readonly UnitOfWork _unitOfWork;

        public DiscountTagServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public List<DiscountTagEntities> GetAll()
        {
            List<DiscountTagEntities> discountTags = null;
            var discountTagList = _unitOfWork.DiscountTagRepository.GetMany(x => x.IsDelete == false).ToList();
            if(discountTagList!=null && discountTagList.Count > 0)
            {
                discountTags = new List<DiscountTagEntities>();
                DiscountTagEntities discountTag = new DiscountTagEntities();
                foreach(var _tag in discountTagList)
                {
                    discountTag = new DiscountTagEntities();
                    discountTag.discountTag = _tag;
                    discountTag.discountTagDetail = _unitOfWork.DiscountTagDetailRepository.GetMany(x => x.DiscountTagOID == _tag.DiscountTagOID).ToList();
                    discountTag.promotionGroupCode = _unitOfWork.PromotionGroupCodeRepository.GetFirstOrDefault(x => x.PromotionGroupCodeOID == _tag.PromotionGroupCodeOID);
                    discountTags.Add(discountTag);
                }
            }

            return discountTags;
        }

        public DiscountTagEntities GetByID(Guid DiscountTagOID)
        {
            DiscountTagEntities discountTag = null;
            var _discountTag = _unitOfWork.DiscountTagRepository.GetFirstOrDefault(x => x.DiscountTagOID == DiscountTagOID && x.IsDelete == false);
            if (_discountTag != null)
            {
                discountTag = new DiscountTagEntities();
                discountTag.discountTag = _discountTag;
                discountTag.discountTagDetail = _unitOfWork.DiscountTagDetailRepository.GetMany(x => x.DiscountTagOID == _discountTag.DiscountTagOID).ToList();
                discountTag.promotionGroupCode = _unitOfWork.PromotionGroupCodeRepository.GetFirstOrDefault(x => x.PromotionGroupCodeOID == _discountTag.PromotionGroupCodeOID);
            }
            return discountTag;
        }

        public void SaveOrUpdate(DiscountTagEntities DiscountTag)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.DiscountTagRepository.GetFirstOrDefault(x => x.DiscountTagOID == DiscountTag.discountTag.DiscountTagOID);
                if (check == null)
                {
                    _unitOfWork.DiscountTagRepository.Insert(DiscountTag.discountTag);
                    foreach (var detail in DiscountTag.discountTagDetail)
                    {
                        detail.DiscountTagOID = DiscountTag.discountTag.DiscountTagOID;
                        _unitOfWork.DiscountTagDetailRepository.Insert(detail);
                    }
                }
                else
                {
                    _unitOfWork.DiscountTagDetailRepository.DetachAll();
                    _unitOfWork.DiscountTagRepository.Update(DiscountTag.discountTag);
                    foreach (var detail in DiscountTag.discountTagDetail)
                    {
                        detail.DiscountTagOID = DiscountTag.discountTag.DiscountTagOID;
                        var _check = _unitOfWork.DiscountTagDetailRepository.GetFirstOrDefault(x => x.DiscountTagOID == DiscountTag.discountTag.DiscountTagOID && x.LanguageCode.ToLower() == detail.LanguageCode.ToLower());
                        if (_check == null)
                        {
                            _unitOfWork.DiscountTagDetailRepository.Insert(detail);
                        }
                        else
                        {

                            _unitOfWork.DiscountTagDetailRepository.Update(detail);
                        }
                    }
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }
    }
}
