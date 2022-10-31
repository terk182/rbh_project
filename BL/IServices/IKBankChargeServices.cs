using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IKBankChargeServices
    {
        KBankCharge GetCharge(string id);
        void Save(KBankCharge data);
    }
}
