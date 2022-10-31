using BL.Entities.CurrencyExchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface ICurrencyExchange
    {
        List<FXRate> FetchRate();
        decimal ConvertToTHB(string fromCCY, decimal amount);
        decimal ConvertFromTHB(string toCCY, decimal amount);
    }
}
