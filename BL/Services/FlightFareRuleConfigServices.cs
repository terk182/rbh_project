using BL.Entities.FareRuleConfig;
using BL.Entities.RobinhoodFare;
using BL.Entities.RobinhoodFlight;
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
    public class FlightFareRuleConfigServices : IFlightFareRuleConfigServices
    {
        private readonly UnitOfWork _unitOfWork;

        public FlightFareRuleConfigServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<FareRuleEntities> GetAll()
        {
            List<FareRuleEntities> fareRules = null;
            var fareruleList = _unitOfWork.FlightFareRuleRepository.GetMany(x => x.IsDelete == false).ToList();
            if(fareruleList!=null && fareruleList.Count > 0)
            {
                fareRules = new List<FareRuleEntities>();
                FareRuleEntities fareRule = new FareRuleEntities();
                foreach(var rule in fareruleList)
                {
                    fareRule = new FareRuleEntities();
                    fareRule.fareRule = rule;
                    fareRule.fareRuleDetails = _unitOfWork.FlightFareRuleDetailRepository.GetMany(x => x.FareRuleOID == rule.FareRuleOID).ToList();
                    fareRule.fareRuleConfig = _unitOfWork.FlightFareRuleConfigRepository.GetMany(x => x.FareRuleOID == rule.FareRuleOID).ToList();
                    fareRules.Add(fareRule);
                }
            }

            return fareRules;
        }

        public FareRuleEntities GetByID(Guid FareRuleOID)
        {
            FareRuleEntities fareRules = null;
            var farerule = _unitOfWork.FlightFareRuleRepository.GetFirstOrDefault(x => x.FareRuleOID == FareRuleOID);
            if (farerule != null)
            {
                fareRules = new FareRuleEntities();
                fareRules.fareRule = farerule;
                fareRules.fareRuleDetails = _unitOfWork.FlightFareRuleDetailRepository.GetMany(x => x.FareRuleOID == farerule.FareRuleOID).ToList();
                fareRules.fareRuleConfig = _unitOfWork.FlightFareRuleConfigRepository.GetMany(x => x.FareRuleOID == farerule.FareRuleOID).ToList();
            }
            return fareRules;
        }

        public void SaveOrUpdate(FareRuleEntities fareRules)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.FlightFareRuleRepository.GetFirstOrDefault(x => x.FareRuleOID == fareRules.fareRule.FareRuleOID);
                if (check == null)
                {
                    _unitOfWork.FlightFareRuleRepository.Insert(fareRules.fareRule);
                    foreach (var detail in fareRules.fareRuleDetails)
                    {
                        detail.FareRuleOID = fareRules.fareRule.FareRuleOID;
                        _unitOfWork.FlightFareRuleDetailRepository.Insert(detail);
                    }
                    if (fareRules.fareRuleConfig != null && fareRules.fareRuleConfig.Count>0)
                    {
                        foreach (var fareBasis in fareRules.fareRuleConfig)
                        {
                            fareBasis.FareRuleOID = fareRules.fareRule.FareRuleOID;
                            _unitOfWork.FlightFareRuleConfigRepository.Insert(fareBasis);
                        }
                    }
                }
                else
                {
                    _unitOfWork.FlightFareRuleDetailRepository.DetachAll();
                    _unitOfWork.FlightFareRuleRepository.Update(fareRules.fareRule);
                    foreach (var detail in fareRules.fareRuleDetails)
                    {
                        detail.FareRuleOID = fareRules.fareRule.FareRuleOID;
                        var _check = _unitOfWork.FlightFareRuleDetailRepository.GetFirstOrDefault(x => x.FareRuleOID == fareRules.fareRule.FareRuleOID && x.LanguageCode.ToLower() == detail.LanguageCode.ToLower());
                        if (_check == null)
                        {
                            _unitOfWork.FlightFareRuleDetailRepository.Insert(detail);
                        }
                        else
                        {

                            _unitOfWork.FlightFareRuleDetailRepository.Update(detail);
                        }
                    }
                    if (fareRules.fareRuleConfig != null)
                    {
                        foreach (var fareBasis in fareRules.fareRuleConfig)
                        {
                            fareBasis.FareRuleOID = fareRules.fareRule.FareRuleOID;
                            var _check = _unitOfWork.FlightFareRuleConfigRepository.GetFirstOrDefault(x => x.FareRuleOID == fareRules.fareRule.FareRuleOID);
                            if (_check == null)
                            {
                                _unitOfWork.FlightFareRuleConfigRepository.Insert(fareBasis);
                            }
                            else
                            {

                                _unitOfWork.FlightFareRuleConfigRepository.Update(fareBasis);
                            }
                        }
                    }
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }

        public Entities.RobinhoodFare.FareRule GetFareRuleConfig(SegmentForGetFareRule segment_Data, string languageCode)
        {
            Entities.RobinhoodFare.FareRule fareRule = null;
            NamingServices namingServices = new NamingServices(_unitOfWork);
            string depCountry = namingServices.GetCountryCode(segment_Data.boardPoint);
            string arrCountry = namingServices.GetCountryCode(segment_Data.offPoint);
            List<FareRuleEntities> fareRuleAll = this.GetAll();
            fareRuleAll = fareRuleAll.FindAll(x => x.fareRule.AirlineCodes == segment_Data.marketingCompany).ToList();
            List<FlightFareRuleConfig> ruleConfigs = new List<FlightFareRuleConfig>();
            foreach (Entities.FareRuleConfig.FareRuleEntities ruleEntities in fareRuleAll)
            {

                ruleConfigs = this.GetRuleConfigs(segment_Data, ruleEntities, depCountry, arrCountry);
                if (ruleConfigs != null && ruleConfigs.Count > 0)
                {
                    var fareruleDetail = ruleConfigs.Find(x => x.RBD.IndexOf(segment_Data.rbd) != -1 && x.FareBasis.IndexOf(segment_Data.fareBasis) != -1);
                    if (fareruleDetail == null)
                    {
                        fareruleDetail = ruleConfigs.Find(x => x.RBD.IndexOf(segment_Data.rbd) != -1 && x.FareBasis=="*");
                    }
                    if (fareruleDetail == null)
                    {
                        fareruleDetail = ruleConfigs.Find(x => x.RBD=="*" && x.FareBasis.IndexOf(segment_Data.fareBasis) != -1);
                    }
                    if (fareruleDetail == null)
                    {
                        fareruleDetail = ruleConfigs.Find(x => x.RBD=="*" && x.FareBasis == "*");
                    }

                    if (fareruleDetail != null)
                    {
                        fareRule = new Entities.RobinhoodFare.FareRule();
                        fareRule.origin = new City(namingServices, languageCode);
                        fareRule.origin.code = segment_Data.boardPoint;
                        fareRule.destination = new City(namingServices, languageCode);
                        fareRule.destination.code = segment_Data.offPoint;
                        fareRule.fareBasis = segment_Data.fareBasis;
                        fareRule.rules = new List<FareRuleDatail>();
                        FareRuleDatail r = new FareRuleDatail();
                        string[] arrRule = ruleEntities.fareRuleDetails.Find(x => x.LanguageCode == languageCode).FareRule.Replace("\r\n", "|").Split('|');
                        r.fareRuleText = new List<string>();
                        foreach (var _text in arrRule)
                        {
                            r.fareRuleText.Add(_text);
                        }
                        fareRule.rules.Add(r);
                    }
                }
            }

            return fareRule;
        }

        public List<FlightFareRuleConfig> GetRuleConfigs(SegmentForGetFareRule segment_Data, FareRuleEntities ruleEntities, string depCountry,string arrCountry)
        {
            if (ruleEntities == null)
                return null;

            List<FlightFareRuleConfig> ruleConfigs = null;
            ruleConfigs = ruleEntities.fareRuleConfig.FindAll(x => x.IsActive == true && x.ZoneFrom == segment_Data.boardPoint && x.ZoneTo == segment_Data.offPoint);
            if (ruleConfigs == null || (ruleConfigs!=null && ruleConfigs.Count==0))
            {
                ruleConfigs = ruleEntities.fareRuleConfig.FindAll(x => x.IsActive == true && x.ZoneFrom == segment_Data.boardPoint && x.ZoneTo == arrCountry);
            }
            if (ruleConfigs == null || (ruleConfigs != null && ruleConfigs.Count == 0))
            {
                ruleConfigs = ruleEntities.fareRuleConfig.FindAll(x => x.IsActive == true && x.ZoneFrom == segment_Data.boardPoint && x.ZoneTo == "*");
            }
            if (ruleConfigs == null || (ruleConfigs != null && ruleConfigs.Count == 0))
            {
                ruleConfigs = ruleEntities.fareRuleConfig.FindAll(x => x.IsActive == true && x.ZoneFrom == depCountry && x.ZoneTo == segment_Data.offPoint);
            }
            if (ruleConfigs == null || (ruleConfigs != null && ruleConfigs.Count == 0))
            {
                ruleConfigs = ruleEntities.fareRuleConfig.FindAll(x => x.IsActive == true && x.ZoneFrom == depCountry && x.ZoneTo == arrCountry);
            }
            if (ruleConfigs == null || (ruleConfigs != null && ruleConfigs.Count == 0))
            {
                ruleConfigs = ruleEntities.fareRuleConfig.FindAll(x => x.IsActive == true && x.ZoneFrom == depCountry && x.ZoneTo == "*");
            }
            if (ruleConfigs == null || (ruleConfigs != null && ruleConfigs.Count == 0))
            {
                ruleConfigs = ruleEntities.fareRuleConfig.FindAll(x => x.IsActive == true && x.ZoneFrom == "*" && x.ZoneTo == segment_Data.offPoint);
            }
            if (ruleConfigs == null || (ruleConfigs != null && ruleConfigs.Count == 0))
            {
                ruleConfigs = ruleEntities.fareRuleConfig.FindAll(x => x.IsActive == true && x.ZoneFrom == "*" && x.ZoneTo == arrCountry);
            }
            if (ruleConfigs == null || (ruleConfigs != null && ruleConfigs.Count == 0))
            {
                ruleConfigs = ruleEntities.fareRuleConfig.FindAll(x => x.IsActive == true && x.ZoneFrom == "*" && x.ZoneTo == "*");
            }
            if(ruleConfigs != null && ruleConfigs.Count == 0)
            {
                ruleConfigs = null;
            }
            return ruleConfigs;
        }

        public List<FareRuleConfig> GetAllConfig()
        {
            List<FareRuleConfig> fareRules = null;
            var fareruleList = _unitOfWork.FlightFareRuleRepository.GetMany(x => x.IsDelete == false).ToList();
            if (fareruleList != null && fareruleList.Count > 0)
            {
                fareRules = new List<FareRuleConfig>();
                FareRuleConfig fareRule = new FareRuleConfig();
                FareRuleDetails details = new FareRuleDetails();
                ConditionConfig condition = new ConditionConfig();
                foreach (var rule in fareruleList)
                {
                    fareRule = new FareRuleConfig();
                    fareRule.airlineCode = rule.AirlineCodes;
                    fareRule.fareFamilyCode = rule.FareFamilyCode;
                    var FlightFareRuleDetail = _unitOfWork.FlightFareRuleDetailRepository.GetMany(x => x.FareRuleOID == rule.FareRuleOID).ToList();
                    fareRule.fareRuleDetails = new List<FareRuleDetails>();
                    
                    foreach (var _detail in FlightFareRuleDetail)
                    {
                        details = new FareRuleDetails();
                        details.languageCode = _detail.LanguageCode;
                        details.fareFamilyName = _detail.FareFamilyName;
                        details.fareRuleText = new List<string>();
                        string[] arrRule = _detail.FareRule.Replace("\r\n", "|").Split('|');
                        foreach (var _text in arrRule)
                        {
                            details.fareRuleText.Add(_text);
                        }
                        fareRule.fareRuleDetails.Add(details);
                    }
                    var FlightFareRuleConfig = _unitOfWork.FlightFareRuleConfigRepository.GetMany(x => x.FareRuleOID == rule.FareRuleOID).ToList();
                    fareRule.conditionConfigs = new List<ConditionConfig>();
                    foreach (var _config in FlightFareRuleConfig)
                    {
                        condition = new ConditionConfig();
                        condition.zoneFrom = _config.ZoneFrom;
                        condition.zoneTo = _config.ZoneTo;
                        condition.rbd = _config.RBD;
                        condition.fareBasis = _config.FareBasis;
                        fareRule.conditionConfigs.Add(condition);
                    }
                    fareRules.Add(fareRule);
                }
            }
            return fareRules;
        }
    }
}
