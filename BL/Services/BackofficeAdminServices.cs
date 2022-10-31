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
    public class BackofficeAdminServices : IBackofficeAdminServices
    {
        private readonly UnitOfWork _unitOfWork;
        public BackofficeAdminServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<BackOfficeAdmin> GetAll()
        {
            return _unitOfWork.BackOffceAdminRepository.GetMany(x => x.IsDelete == false).ToList();
        }

        public BackOfficeAdmin GetByID(Guid BackOfficeAdminOID)
        {
            return _unitOfWork.BackOffceAdminRepository.GetFirstOrDefault(x => x.BackOfficeAdminOID == BackOfficeAdminOID && x.IsDelete == false);
        }

        public BackOfficeAdmin GetByLogin(string username, string password)
        {
            string encryptPwd = Utilities.Encryptor.MD5Hash(password);
            return _unitOfWork.BackOffceAdminRepository.GetFirstOrDefault(x => x.Username == username && x.Password == encryptPwd );
        }

        public BackOfficeAdmin GetByUsermame(string username)
        {
            return _unitOfWork.BackOffceAdminRepository.GetFirstOrDefault(x => x.Username == username && x.IsDelete == false);
        }

        public void SaveOrUpdate(BackOfficeAdmin BackOfficeAdmin)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.BackOffceAdminRepository.GetFirstOrDefault(x => x.BackOfficeAdminOID == BackOfficeAdmin.BackOfficeAdminOID);
                if (check == null)
                {
                    _unitOfWork.BackOffceAdminRepository.Insert(BackOfficeAdmin);
                }
                else
                {
                    _unitOfWork.BackOffceAdminRepository.Update(BackOfficeAdmin);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }

     
    }
}
