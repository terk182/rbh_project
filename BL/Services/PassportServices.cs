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
    public class PassportServices : IPassportServices
    {
        private readonly UnitOfWork _unitOfWork;

        public PassportServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<PassportConfig> GetAll()
        {
            return _unitOfWork.PassportConfigRepository.GetMany(x => x.IsDelete == false).ToList();
        }

        public PassportConfig GetByID(Guid PassportOID)
        {
            return _unitOfWork.PassportConfigRepository.GetFirstOrDefault(x => x.PassportOID == PassportOID && x.IsDelete == false);
        }

        public void SaveOrUpdate(PassportConfig PassportConfig)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.PassportConfigRepository.GetFirstOrDefault(x => x.PassportOID == PassportConfig.PassportOID);
                if (check == null)
                {
                    _unitOfWork.PassportConfigRepository.Insert(PassportConfig);
                }
                else
                {
                    _unitOfWork.PassportConfigRepository.Update(PassportConfig);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }
    }
}
