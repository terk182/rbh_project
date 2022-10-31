using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.PNRCancel
{
    public class Response
    {
        public string bookingKeyReference { get; set; }
        public List<string> recordLocator { get; set; }
        public bool isError { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }
}
