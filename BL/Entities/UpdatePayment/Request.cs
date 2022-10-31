using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.UpdatePayment
{
    public class Request
    {
        public string bookingKeyReference { get; set; }//TransactionID
        public string bookingURN { get; set; }
        public int paymentStatus { get; set; }
        public string paymentReference { get; set; }
        public List<string> remark { get; set; }
    }
}
