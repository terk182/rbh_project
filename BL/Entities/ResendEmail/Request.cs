using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.ResendEmail
{
    public class Request
    {
        public string bookingKeyReference { get; set; }//TransactionID
        public string Email { get; set; }
    }
}
