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
    public class SeatMapControlServices : ISeatMapControlServices
    {
        private readonly UnitOfWork _unitOfWork;

        public SeatMapControlServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<SeatMapControl> GetAll()
        {
            return _unitOfWork.SeatMapRepository.GetMany(x => x.IsDelete == false).ToList();
        }

        public SeatMapControl GetByID(Guid SeatMapOID)
        {
            return _unitOfWork.SeatMapRepository.GetFirstOrDefault(x => x.SeatMapOID == SeatMapOID && x.IsDelete == false);
        }

        public void SaveOrUpdate(SeatMapControl seatMap)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.SeatMapRepository.GetFirstOrDefault(x => x.SeatMapOID == seatMap.SeatMapOID);
                if (check == null)
                {
                    _unitOfWork.SeatMapRepository.Insert(seatMap);
                }
                else
                {
                    _unitOfWork.SeatMapRepository.Update(seatMap);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }
    }
}
