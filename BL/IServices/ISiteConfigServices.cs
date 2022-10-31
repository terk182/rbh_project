using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface ISiteConfigServices
    {
        List<DataModel.SiteConfig> GetAll();
        DataModel.SiteConfig GetByID(Guid ConfigOID);
        DataModel.SiteConfig GetByKey(string ConfigKey);
        void SaveOrUpdate(DataModel.SiteConfig airline);
    }
}
