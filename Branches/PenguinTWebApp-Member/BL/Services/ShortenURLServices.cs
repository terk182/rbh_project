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
    public class ShortenURLServices : IShortenURLServices
    {
        private readonly UnitOfWork _unitOfWork;
        public ShortenURLServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void Delete(string code)
        {
            using (var scope = new TransactionScope())
            {
                _unitOfWork.ShortenURLRepository.Delete(code);
                _unitOfWork.Save();
                scope.Complete();
            }
        }

        public List<ShortenURL> GetAll()
        {
            return _unitOfWork.ShortenURLRepository.GetAll().ToList();
        }

        public ShortenURL GetById(string code)
        {
            return _unitOfWork.ShortenURLRepository.GetFirstOrDefault(x => x.Code == code);
        }

        public void SaveOrUpdate(ShortenURL shortenURL)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.ShortenURLRepository.GetFirstOrDefault(x => x.Code == shortenURL.Code);
                if (check == null)
                {
                    _unitOfWork.ShortenURLRepository.Insert(shortenURL);
                }
                else
                {
                    _unitOfWork.ShortenURLRepository.Update(shortenURL);
                }
                _unitOfWork.Save();
                scope.Complete();
            }
        }
    }
}
