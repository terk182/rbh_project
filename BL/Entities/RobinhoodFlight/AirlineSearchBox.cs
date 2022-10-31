using BL.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BL.Entities.RobinhoodFlight
{
    public class AirlineSearchBox
    {
        public AirlineSearch airlineSearch;
        public AirlineSearchBox()
        {
            if (System.Web.HttpContext.Current.Cache["C_AIRLINE_SEARCH"] != null)
            {
                airlineSearch = (AirlineSearch)System.Web.HttpContext.Current.Cache["C_AIRLINE_SEARCH"];
            }
            else
            {
                string furl = "http://www.fareok.com/fareok_adminDev/api/AirlineWithImageAPI/100/1";
                string json = HttpUtility.getHttp(furl);
                if (String.IsNullOrEmpty(json) == false)
                {
                    airlineSearch = JsonConvert.DeserializeObject<AirlineSearch>(json);
                    System.Web.HttpContext.Current.Cache.Insert("C_AIRLINE_SEARCH", airlineSearch, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
                }
            }
        }

        public SelectList GetList(string allAirlineText)
        {

            var cList = new List<object>();
            cList.Add(new
            {
                ID = "",
                Name = allAirlineText
            });

            foreach (var air in this.airlineSearch.AirlineWithImages)
            {
                cList.Add(new
                {
                    ID = air.AirlineCode,
                    Name = air.AirlineName
                });
            }

            return new SelectList(cList, "ID", "Name", "");
        }
        
    }


    public class AirlineSearch
    {
        public List<AirlineWithImage> AirlineWithImages { get; set; }
    }

    public class AirlineWithImage
    {
        public int ailId { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public string ImageUrl { get; set; }
    }
}
