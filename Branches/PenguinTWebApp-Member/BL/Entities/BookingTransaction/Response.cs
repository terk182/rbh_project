using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.BookingTransaction
{
    public class Response
    {
        public string BookingKeyReference { get; set; }
        public bool isError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
