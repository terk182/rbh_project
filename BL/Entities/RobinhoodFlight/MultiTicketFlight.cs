using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodFlight
{
    public class MultiTicketFlight
    {
        public Guid id { get; set; }
        public string recommendation_number { get; set; }
        public bool isMultiTicket { get; set; }//true from MultiTicket
        public Pricing fare { get; set; }
        public RobinhoodFare.DiscountTag discountTag { get; set; }
        public List<Flight> Flight_SegRef1 { get; set; }//departureFlight
        public List<Flight> Flight_SegRef2 { get; set; }//returnFlight
        public List<string> fareRule { get; set; }
        public Airline mainAirline
        {
            get
            {
                if (this.Flight_SegRef1 != null && this.Flight_SegRef1.Count > 0)
                {
                    return this.Flight_SegRef1[0].flightDetails[0].airline;
                }
                else
                {
                    if (this.Flight_SegRef1 == null && this.Flight_SegRef2 != null && this.Flight_SegRef2.Count > 0)
                    {
                        return this.Flight_SegRef2[0].flightDetails[0].airline;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        public List<string> upsell { get; set; }
        public List<FamilyInformation> FamilyInformation { get; set; }

        public int minSegRef1Time //maxDepTime
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef1 != null)
                {
                    time = int.Parse(this.Flight_SegRef1.Min(x => x.totalTime));
                }
                return time;
            }
        }
        public int maxSegRef1Time //maxDepTime
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef1 != null)
                {
                    time = int.Parse(this.Flight_SegRef1.Max(x => x.totalTime));
                }
                return time;
            }
        }
        public int minSegRef2Time //maxDepTime
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef2 != null)
                {
                    time = int.Parse(this.Flight_SegRef2.Min(x => x.totalTime));
                }
                return time;
            }
        }
        public int maxSegRef2Time//maxRetTime
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef2 != null)
                {
                    time = int.Parse(this.Flight_SegRef2.Max(x => x.totalTime));
                }
                return time;
            }
        }
        

        public int minSegRef1DepTime //maxDepTime
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef1 != null)
                {
                    //time = this.Flight_SegRef1.Min(x => x.flightDetails.Min(y => y.depDisplayDateTime.time));
                    time = this.Flight_SegRef1.Min(x => x.flightDetails[0].depDisplayDateTime.time);
                }
                return time;
            }
        }
        public int maxSegRef1DepTime //maxDepTime
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef1 != null)
                {
                    //time = this.Flight_SegRef1.Max(x => x.flightDetails.Max(y=>y.depDisplayDateTime.time));
                    time = this.Flight_SegRef1.Max(x => x.flightDetails[0].depDisplayDateTime.time);
                }
                return time;
            }
        }

        public void ComputeConnectionTime()
        {
            //DEP
            if (this.Flight_SegRef1 != null)
            {
                foreach (var flight in Flight_SegRef1)
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
            if (this.Flight_SegRef2 != null)
            {
                foreach (var flight in Flight_SegRef2)
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

        public void UIScale(int allMaxSegRef1Time, int allMaxSegRef2Time)
        {
            //DEP
            if (this.Flight_SegRef1 != null)
            {
                //set up scale
                foreach (var flight in Flight_SegRef1)
                {
                    int allScale = 0;
                    int totalTime = int.Parse(flight.totalTime);
                    int percentage = (totalTime * 100) / allMaxSegRef1Time;
                    int totalConnectionTime = flight.flightDetails.Sum(x => int.Parse(String.IsNullOrEmpty(x.connectingTime) ? "0" : x.connectingTime));
                    int flightTime = (totalTime - totalConnectionTime) / flight.flightDetails.Count;
             
                    //set last stop
                    flight.flightDetails[flight.flightDetails.Count - 1].connectingTime = "0";
                }

            }
            //RET
            if (this.Flight_SegRef2 != null)
            {

                //set up scale
                foreach (var flight in Flight_SegRef2)
                {
                    int allScale = 0;
                    int totalTime = int.Parse(flight.totalTime);
                    int percentage = (totalTime * 100) / allMaxSegRef2Time;
                    int totalConnectionTime = flight.flightDetails.Sum(x => int.Parse(String.IsNullOrEmpty(x.connectingTime) ? "0" : x.connectingTime));
                    int flightTime = (totalTime - totalConnectionTime) / flight.flightDetails.Count;
                    //set last stop
                    flight.flightDetails[flight.flightDetails.Count - 1].connectingTime = "0";
                }
            }
           
        }

        public string Corp_Code { get; set; }
        public string type { get; set; }//A=Amadeus,K=Kiwi
        public decimal addOnPrice { get; set; }
    }
}
