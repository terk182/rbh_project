using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IAPIConfigServices
    {
        List<APIUser> GetAll();
        APIUser GetById(Guid id);
        void SaveOrUpdate(APIUser user);
        void Delete(Guid id);
        Entities.APILogin.Response Login(Entities.APILogin.Request request, string webmode);
        APIUser GetByToken(string token);
    }
}
