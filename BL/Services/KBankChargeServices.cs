using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using DataModel.UnitOfWork;

namespace BL
{
    public class KBankChargeServices : IKBankChargeServices
    {
        private readonly UnitOfWork _unitOfWork;

        public KBankChargeServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public KBankCharge GetCharge(string id)
        {
            return _unitOfWork.KBankChargeRepository.GetFirstOrDefault(x => x.ChargeID == id);
        }

        public void Save(KBankCharge data)
        {
            _unitOfWork.KBankChargeRepository.Insert(data);
            _unitOfWork.Save();
        }
    }
}
