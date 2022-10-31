using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IBackofficeAdminServices
    {
        List<BackOfficeAdmin> GetAll();
        BackOfficeAdmin GetByID(Guid BackOfficeAdminOID);
        BackOfficeAdmin GetByUsermame(string username);
        BackOfficeAdmin GetByLogin(string username, string password);
        void SaveOrUpdate(BackOfficeAdmin BackOfficeAdmin);
    }
}
