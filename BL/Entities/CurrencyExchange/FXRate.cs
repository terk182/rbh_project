using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.CurrencyExchange
{
    public class FXRate
    {
        public string currencyCode { get; set; }
        public string currencyName { get; set; }
        public decimal rateToTHB { get; set; }
    }
}
