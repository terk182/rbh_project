using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodFlight
{
    public class FamilyInformation
    {
        public int RefNumber { get; set; }
        public string FareFamilyname { get; set; }
        public string Description { get; set; }
        public string Carrier { get; set; }
        public List<Service> Services { get; set; }
    }
    public class Service
    {
        public int Reference { get; set; }
        public string Status { get; set; }
        public string ServiceGroup { get; set; }
        public string ServiceSubGroup { get; set; }
        public string CommercialName { get; set; }
    }
}