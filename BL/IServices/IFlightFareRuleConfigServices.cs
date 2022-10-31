using BL.Entities.FareRuleConfig;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IFlightFareRuleConfigServices
    {
        List<FareRuleEntities> GetAll();
        FareRuleEntities GetByID(Guid FareRuleOID);
        void SaveOrUpdate(FareRuleEntities fareRules);
        Entities.RobinhoodFare.FareRule GetFareRuleConfig(SegmentForGetFareRule segment_Data, string languageCode);
        List<FlightFareRuleConfig> GetRuleConfigs(SegmentForGetFareRule segment_Data, FareRuleEntities ruleEntities, string depCountry, string arrCountry);
        List<FareRuleConfig> GetAllConfig();
    }
}
