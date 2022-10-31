using BL.Entities;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IPassportServices
    {
        List<PassportConfig> GetAll();
        PassportConfig GetByID(Guid id);
        void SaveOrUpdate(PassportConfig passport);
    }
}