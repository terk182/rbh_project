using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.Invoice
{
    public class Request
    {
        public string Ref { get; set; }
        public string NameLastname { get; set; }
        public string Email { get; set; }
        public string Telephonenumber { get; set; }
    }
}
