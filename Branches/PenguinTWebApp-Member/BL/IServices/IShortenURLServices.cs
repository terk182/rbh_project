using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IShortenURLServices
    {
        List<ShortenURL> GetAll();
        ShortenURL GetById(string code);
        void SaveOrUpdate(ShortenURL airPromotion);
        void Delete(string code);
    }
}
