using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BL.Entities.AirlineControl;
using DataModel;
using DataModel.UnitOfWork;

namespace BL
{
    public class AirlineControlServices : IAirlineControlServices
    {
        private readonly UnitOfWork _unitOfWork;

        public AirlineControlServices (UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<AirlineControl> GetAll()
        {
            return _unitOfWork.AirlineControlRepository.GetMany(x => x.IsDelete == false).ToList();
        }

        public AirlineControl GetByID(Guid AirlineOID)
        {
            return _unitOfWork.AirlineControlRepository.GetFirstOrDefault(x => x.AirlineOID == AirlineOID && x.IsDelete == false);
            //AirlineControlEntities airlineentities = new AirlineControlEntities();
            //airlineentities.airlineDetail = _unitOfWork.AirlineControlRepository.GetFirstOrDefault(x => x.AirlineOID == AirlineOID && x.IsDelete == false);
            //airlineentities.airlineSubDetail = _unitOfWork.AirlineControlSubRepository.GetMany(x => x.AirlineSubOID == AirlineOID && x.IsDelete == false).ToList();
            //return airlineentities;
        }

        public void SaveOrUpdate(AirlineControl airline)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.AirlineControlRepository.GetFirstOrDefault(x => x.AirlineOID == airline.AirlineOID);
                if (check == null)
                {
                    _unitOfWork.AirlineControlRepository.Insert(airline);
                }
                else
                {
                    _unitOfWork.AirlineControlRepository.Update(airline);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }

        public List<AirlineControlSub> GetAllAirlineSub(Guid AirlineOID)
        {
            return _unitOfWork.AirlineControlSubRepository.GetMany(x =>x.AirlineOID == AirlineOID && x.IsDelete == false).ToList();
        }

        public AirlineControlSub GetByIDAirlineSub(Guid AirlineSubOID, Guid AirlineOID)
        {
            return _unitOfWork.AirlineControlSubRepository.GetFirstOrDefault(x => x.AirlineSubOID == AirlineSubOID && x.IsDelete == false);
        }

        public void SaveOrUpdateAirlinesub(AirlineControlSub airlinesub)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.AirlineControlSubRepository.GetFirstOrDefault(x => x.AirlineSubOID == airlinesub.AirlineSubOID);
                if (check == null)
                {
                    _unitOfWork.AirlineControlSubRepository.Insert(airlinesub);
                }
                else
                {
                    _unitOfWork.AirlineControlSubRepository.Update(airlinesub);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }

        public bool CheckAirlineControl(List<AirlineControlSub> airlinesub, string orginalCountryCode, string destinationCountryCode, string airlineCode, string rbd, string fareBasis)
        {
            bool bAllow = false;
            if (airlinesub == null || (airlinesub!=null && airlinesub.Count==0))
            {
                return false;
            }
            AirlineControlSub airline = airlinesub.Find(x => x.AirlineCode.IndexOf(airlineCode) != -1 && x.FareBasis.IndexOf(fareBasis) != -1 && x.ClassOfService.IndexOf(rbd) != -1);
            if (airline != null)
            {
                bAllow = airline.IsActive.Value;
            }
            else
            {
                airline = airlinesub.Find(x => x.AirlineCode.IndexOf(airlineCode) != -1 && x.FareBasis.IndexOf(fareBasis) != -1 && x.ClassOfService == "*");
                if (airline != null)
                {
                    bAllow = airline.IsActive.Value;
                }
                else
                {
                    airline = airlinesub.Find(x => x.AirlineCode.IndexOf(airlineCode) != -1 && x.FareBasis == "*" && x.ClassOfService.IndexOf(rbd) != -1);
                    if (airline != null)
                    {
                        bAllow = airline.IsActive.Value;
                    }
                    else
                    {
                        airline = airlinesub.Find(x => x.AirlineCode.IndexOf(airlineCode) != -1 && x.FareBasis == "*" && x.ClassOfService == "*");
                        if (airline != null)
                        {
                            bAllow = airline.IsActive.Value;
                        }
                        else
                        {
                            airline = airlinesub.Find(x => x.AirlineCode=="YY" && x.FareBasis.IndexOf(fareBasis) != -1 && x.ClassOfService.IndexOf(rbd) != -1);
                            if (airline != null)
                            {
                                bAllow = airline.IsActive.Value;
                            }
                            else
                            {
                                airline = airlinesub.Find(x => x.AirlineCode == "YY" && x.FareBasis.IndexOf(fareBasis) != -1 && x.ClassOfService == "*");
                                if (airline != null)
                                {
                                    bAllow = airline.IsActive.Value;
                                }
                                else
                                {
                                    airline = airlinesub.Find(x => x.AirlineCode == "YY" && x.FareBasis == "*" && x.ClassOfService.IndexOf(rbd) != -1);
                                    if (airline != null)
                                    {
                                        bAllow = airline.IsActive.Value;
                                    }
                                    else
                                    {
                                        airline = airlinesub.Find(x => x.AirlineCode == "YY" && x.FareBasis == "*" && x.ClassOfService == "*");
                                        if (airline != null)
                                        {
                                            bAllow = airline.IsActive.Value;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return bAllow;
        }


    }
}
