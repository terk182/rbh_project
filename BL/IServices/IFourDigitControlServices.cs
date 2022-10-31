using BL.Entities;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IFourDigitControlServices
    {
        List<FourDigitControl> GetAll();
        FourDigitControl GetByID(Guid id);
        void SaveOrUpdate(FourDigitControl fourDigit);
    }
}