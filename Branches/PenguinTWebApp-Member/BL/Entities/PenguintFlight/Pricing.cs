using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.GogojiiFlight
{
    public class Pricing
    {
        public string currencyCode { get; set; }
        public PaxPricing adtFare { get; set; }
        public PaxPricing chdFare { get; set; }
        public PaxPricing infFare { get; set; }

        private int _adtNo { get; set; }
        private int _chdNo { get; set; }
        private int _infNo { get; set; }

        public Pricing(int adtNo, int chdNo, int infNo)
        {
            this._adtNo = adtNo;
            this._chdNo = chdNo;
            this._infNo = infNo;
        }

        public decimal total
        {
            get
            {
                decimal t = this._adtNo * adtFare.net;
                if (this._chdNo > 0)
                {
                    t += (this._chdNo * chdFare.net);
                }
                if (this._infNo > 0)
                {
                    t += (this._infNo * infFare.net);
                }
                return t;
            }
        }
    }

    public class PaxPricing
    {
        public decimal fare { get; set; }
        public decimal fareBeforeMarkup { get; set; }
        public decimal tax { get; set; }
        public decimal lessFare { get; set; }
        public decimal net {
            get
            {
                return this.fare + this.tax;
            }
        }
    }
}
