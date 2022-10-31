using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.ResendEmail
{
    public class Response
    {
        public bool isError { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }
}
