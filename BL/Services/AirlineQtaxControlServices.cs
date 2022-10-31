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
    public class AirlineQtaxControlServices : IAirlineQtaxControlServices
    {
        private readonly UnitOfWork _unitOfWork;

        public AirlineQtaxControlServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<AirlineQtaxControl> GetAll()
        {
            return _unitOfWork.AirlineQtaxControlRepository.GetMany(x => x.IsDelete == false).ToList();
        }

        public AirlineQtaxControl GetByID(Guid AirlineQtaxOID)
        {
            return _unitOfWork.AirlineQtaxControlRepository.GetFirstOrDefault(x => x.AirlineQtaxOID == AirlineQtaxOID && x.IsDelete == false);
        }

        public void SaveOrUpdate(AirlineQtaxControl airlineQtax)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.AirlineQtaxControlRepository.GetFirstOrDefault(x => x.AirlineQtaxOID == airlineQtax.AirlineQtaxOID);
                if (check == null)
                {
                    _unitOfWork.AirlineQtaxControlRepository.Insert(airlineQtax);
                }
                else
                {
                    _unitOfWork.AirlineQtaxControlRepository.Update(airlineQtax);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }
        public bool CheckAirlineQTaxConfig(string orginalCountryCode, string destinationCountryCode, string airlineCode)
        {
            bool isTax = false;//true is tax, false is fare
            List<AirlineQtaxControl> airlineQtaxControls = this.GetAll();
            if (airlineQtaxControls != null && airlineQtaxControls.Count > 0)
            {
                AirlineQtaxControl airlineQtax = airlineQtaxControls.Find(x => x.AirlineCode.IndexOf(airlineCode) != -1 && x.OriginalCountryCode.IndexOf(orginalCountryCode) != -1 && x.DestinationCountryCode.IndexOf(destinationCountryCode) != -1);
                if (airlineQtax != null)
                {
                    isTax = airlineQtax.IsActive.Value;
                }
                else
                {
                    airlineQtax = airlineQtaxControls.Find(x => x.AirlineCode.IndexOf(airlineCode) != -1 && x.OriginalCountryCode.IndexOf(orginalCountryCode) != -1 && x.DestinationCountryCode == "*");
                    if (airlineQtax != null)
                    {
                        isTax = airlineQtax.IsActive.Value;
                    }
                    else
                    {
                        airlineQtax = airlineQtaxControls.Find(x => x.AirlineCode.IndexOf(airlineCode) != -1 && x.OriginalCountryCode == "*" && x.DestinationCountryCode.IndexOf(destinationCountryCode) != -1);
                        if (airlineQtax != null)
                        {
                            isTax = airlineQtax.IsActive.Value;
                        }
                        else
                        {
                            airlineQtax = airlineQtaxControls.Find(x => x.AirlineCode.IndexOf(airlineCode) != -1 && x.OriginalCountryCode == "*" && x.DestinationCountryCode == "*");
                            if (airlineQtax != null)
                            {
                                isTax = airlineQtax.IsActive.Value;
                            }
                            else
                            {
                                airlineQtax = airlineQtaxControls.Find(x => x.AirlineCode == "YY" && x.OriginalCountryCode == "*" && x.DestinationCountryCode == "*");
                                if (airlineQtax != null)
                                {
                                    isTax = airlineQtax.IsActive.Value;
                                }
                            }
                        }
                    }
                }
            }

            return isTax;
        }
    }
}
