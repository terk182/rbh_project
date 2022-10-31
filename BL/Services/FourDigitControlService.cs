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
    public class FourDigitControlServices : IFourDigitControlServices
    {
        private readonly UnitOfWork _unitOfWork;

        public FourDigitControlServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<FourDigitControl> GetAll()
        {
            return _unitOfWork.FourDigitRepository.GetMany(x => x.IsDelete == false).ToList();
        }

        public FourDigitControl GetByID(Guid FourDigitOID)
        {
            return _unitOfWork.FourDigitRepository.GetFirstOrDefault(x => x.FourDigitOID == FourDigitOID && x.IsDelete == false);
        }

        public void SaveOrUpdate(FourDigitControl fourDigit)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.FourDigitRepository.GetFirstOrDefault(x => x.FourDigitOID == fourDigit.FourDigitOID);
                if (check == null)
                {
                    _unitOfWork.FourDigitRepository.Insert(fourDigit);
                }
                else
                {
                    _unitOfWork.FourDigitRepository.Update(fourDigit);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }
    }
}
