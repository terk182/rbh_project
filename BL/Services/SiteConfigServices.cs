using DataModel.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BL
{
    public class SiteConfigServices : ISiteConfigServices
    {
        private readonly UnitOfWork _unitOfWork;
        public SiteConfigServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public List<DataModel.SiteConfig> GetAll()
        {
            return _unitOfWork.SiteConfigRepository.GetAll().OrderBy(x=>x.Seq).ToList();
        }
        public DataModel.SiteConfig GetByID(Guid ConfigOID)
        {
            return _unitOfWork.SiteConfigRepository.GetFirstOrDefault(x => x.ConfigOID == ConfigOID);
        }
        public DataModel.SiteConfig GetByKey(string ConfigKey)
        {
            return _unitOfWork.SiteConfigRepository.GetFirstOrDefault(x => x.ConfigKey == ConfigKey);
        }
        public void SaveOrUpdate(DataModel.SiteConfig airline)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.SiteConfigRepository.GetFirstOrDefault(x => x.ConfigOID == airline.ConfigOID);
                if (check == null)
                {
                    _unitOfWork.SiteConfigRepository.Insert(airline);
                }
                else
                {
                    _unitOfWork.SiteConfigRepository.Update(airline);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }
    }
}
