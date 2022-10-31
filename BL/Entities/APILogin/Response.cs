using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.APILogin
{
    public class Response
    {
        public string accessToken { get; set; }
        public string tokenType { get; set; }
        public DateTime expires { get; set; }
        public bool isError { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }
}
