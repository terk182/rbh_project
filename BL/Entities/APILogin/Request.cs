using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.APILogin
{
    public class Request
    {
        public string username { get; set; }
        public string password { get; set; }
        public string grantType { get; set; }
    }
}
