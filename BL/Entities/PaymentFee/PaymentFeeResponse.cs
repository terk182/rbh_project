using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.PaymentFee
{
    public class PaymentFeeResponse
    {
        public bool isError { get; set; }
        public string errorMessage { get; set; }
        public Payment paymentFeeList { get; set; }
    }
}
