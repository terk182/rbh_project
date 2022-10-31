using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.GogojiiFlight
{
    public class MasterFlight
    {
        public Guid id { get; set; }
        public Pricing fare { get; set; }
        public List<Flight> departureFlight { get; set; }
        public List<Flight> returnFlight { get; set; }
        public List<string> fareRule { get; set; }
        public Airline mainAirline
        {
            get
            {
                if (this.departureFlight != null && this.departureFlight.Count > 0)
                {
                    return this.departureFlight[0].flightDetails[0].airline;
                }
                else
                {
                    return null;
                }
            }
        }

        public int maxDepTime
        {
            get
            {
                int time = 0;
                if (this.departureFlight != null)
                {
                    time = int.Parse(this.departureFlight.Max(x => x.totalTime));
                }
                return time;
            }
        }

        public int maxRetTime
        {
            get
            {
                int time = 0;
                if (this.returnFlight != null)
                {
                    time = int.Parse(this.returnFlight.Max(x => x.totalTime));
                }
                return time;
            }
        }

        public void ComputeConnectionTime()
        {
            //DEP
            if (this.departureFlight != null)
            {
                foreach(var flight in departureFlight)
                {
                    
                    if (flight.flightDetails.Count > 1)
                    {
                        for (int i = 0; i < flight.flightDetails.Count - 1; i++)
                        {
                            TimeSpan ts = flight.flightDetails[i + 1].departureDateTime - flight.flightDetails[i].arrivalDateTime;
                            int hour = (ts.Days * 24) + ts.Hours;
                            flight.flightDetails[i].connectingTime = hour.ToString().PadLeft(2, '0') + ts.Minutes.ToString().PadLeft(2, '0');
                        }
                    }
                }
                

            }
            //RET
            if (this.returnFlight != null)
            {
                foreach (var flight in returnFlight)
                {
                    if (flight.flightDetails.Count > 1)
                    {
                        for (int i = 0; i < flight.flightDetails.Count - 1; i++)
                        {
                            TimeSpan ts = flight.flightDetails[i + 1].departureDateTime - flight.flightDetails[i].arrivalDateTime;
                            int hour = (ts.Days * 24) + ts.Hours;
                            flight.flightDetails[i].connectingTime = hour.ToString().PadLeft(2, '0') + ts.Minutes.ToString().PadLeft(2, '0');
                        }
                    }
                }
                
            }
        }


        public void UIScale(int allMaxDepTime, int allMaxRetTime)
        {
            //DEP
            if (this.departureFlight != null)
            {
                //set up scale
                foreach (var flight in departureFlight)
                {
                    int allScale = 0;
                    int totalTime = int.Parse(flight.totalTime);
                    int percentage = (totalTime * 100) / allMaxDepTime;
                    int totalConnectionTime = flight.flightDetails.Sum(x => int.Parse(String.IsNullOrEmpty(x.connectingTime) ? "0" : x.connectingTime));
                    int flightTime = (totalTime - totalConnectionTime) / flight.flightDetails.Count;
                    foreach (var detail in flight.flightDetails)
                    {
                        detail.scaleFlight = (flightTime * percentage) / totalTime;
                        detail.scaleConnection = ((int.Parse(String.IsNullOrEmpty(detail.connectingTime) ? "0" : detail.connectingTime)) * percentage) / int.Parse(flight.totalTime);
                        allScale += (detail.scaleFlight + detail.scaleConnection);
                    }
                    //set last stop
                    flight.flightDetails[flight.flightDetails.Count - 1].connectingTime = "0";
                    flight.flightDetails[flight.flightDetails.Count - 1].scaleConnection = 100 - allScale;
                }

            }
            //RET
            if (this.returnFlight != null)
            {

                //set up scale
                foreach (var flight in returnFlight)
                {
                    int allScale = 0;
                    int totalTime = int.Parse(flight.totalTime);
                    int percentage = (totalTime * 100) / allMaxRetTime;
                    int totalConnectionTime = flight.flightDetails.Sum(x => int.Parse(String.IsNullOrEmpty(x.connectingTime) ? "0" : x.connectingTime));
                    int flightTime = (totalTime - totalConnectionTime) / flight.flightDetails.Count;
                    foreach (var detail in flight.flightDetails)
                    {
                        detail.scaleFlight = (flightTime * percentage) / totalTime;
                        detail.scaleConnection = ((int.Parse(String.IsNullOrEmpty(detail.connectingTime) ? "0" : detail.connectingTime)) * percentage) / int.Parse(flight.totalTime);
                        allScale += (detail.scaleFlight + detail.scaleConnection);
                    }
                    //set last stop
                    flight.flightDetails[flight.flightDetails.Count - 1].connectingTime = "0";
                    flight.flightDetails[flight.flightDetails.Count - 1].scaleConnection = 100 - allScale;
                }
            }
        }
    }

}
