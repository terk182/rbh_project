using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodPassport
{
    public class PassportConfigResponse
    {
        public bool isError { get; set; }
        public string errorMessage { get; set; }
        public List<DataModel.PassportConfig> passportList { get; set; }
    }
}
