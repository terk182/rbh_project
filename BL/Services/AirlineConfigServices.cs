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
    public class AirlineConfigServices : IAirlineConfigServices
    {
        private readonly UnitOfWork _unitOfWork;

        public AirlineConfigServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<AirlineConfig> GetAll()
        {
            return _unitOfWork.AirlineConfigRepository.GetMany(x => x.IsDelete == false).ToList();
        }
        public AirlineConfig GetByID(Guid AirlineConfigOID)
        {
            return _unitOfWork.AirlineConfigRepository.GetFirstOrDefault(x => x.AirlineConfigOID == AirlineConfigOID && x.IsDelete == false);
        }
        public void SaveOrUpdate(AirlineConfig airline)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.AirlineConfigRepository.GetFirstOrDefault(x => x.AirlineConfigOID == airline.AirlineConfigOID);
                if (check == null)
                {
                    _unitOfWork.AirlineConfigRepository.Insert(airline);
                }
                else
                {
                    _unitOfWork.AirlineConfigRepository.Update(airline);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }
        public AirlineConfig GetAirlineConfig(List<AirlineConfig> allAirlineConfig, string departureCode, string destinationCode)
        {
            if (allAirlineConfig == null)
                return null;
            NamingServices namingServices = new NamingServices(_unitOfWork);
            string depCountry = namingServices.GetCountryCode(departureCode);
            string arrCountry = namingServices.GetCountryCode(destinationCode);

            var airlineConfig = allAirlineConfig.Find(x => x.DepartureCode.IndexOf(departureCode) != -1 && x.DestinationCode.IndexOf(destinationCode) != -1);
            if (airlineConfig == null)
            {
                airlineConfig = allAirlineConfig.Find(x => x.DepartureCode.IndexOf(departureCode) != -1 && x.DestinationCode.IndexOf(arrCountry) != -1);
            }
            if (airlineConfig == null)
            {
                airlineConfig = allAirlineConfig.Find(x => x.DepartureCode.IndexOf(departureCode) != -1 && x.DestinationCode=="*");
            }
            if (airlineConfig == null)
            {
                airlineConfig = allAirlineConfig.Find(x => x.DepartureCode.IndexOf(depCountry) != -1 && x.DestinationCode.IndexOf(destinationCode) != -1);
            }
            if (airlineConfig == null)
            {
                airlineConfig = allAirlineConfig.Find(x => x.DepartureCode.IndexOf(depCountry) != -1 && x.DestinationCode.IndexOf(arrCountry) != -1);
            }
            if (airlineConfig == null)
            {
                airlineConfig = allAirlineConfig.Find(x => x.DepartureCode.IndexOf(depCountry) != -1 && x.DestinationCode == "*");
            }
            if (airlineConfig == null)
            {
                airlineConfig = allAirlineConfig.Find(x => x.DepartureCode == "*" && x.DestinationCode.IndexOf(destinationCode) != -1);
            }
            if (airlineConfig == null)
            {
                airlineConfig = allAirlineConfig.Find(x => x.DepartureCode == "*" && x.DestinationCode.IndexOf(arrCountry) != -1);
            }
            if (airlineConfig == null)
            {
                airlineConfig = allAirlineConfig.Find(x => x.DepartureCode=="*" && x.DestinationCode == "*");
            }
            return airlineConfig;
        }
    }
}
