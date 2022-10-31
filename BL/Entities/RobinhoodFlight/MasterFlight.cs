using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodFlight
{
    public class MasterFlight
    {
        public Guid id { get; set; }
        public Pricing fare { get; set; }
        public decimal addOnPrice { get; set; }
        public List<Flight> departureFlight { get; set; }
        public List<Flight> returnFlight { get; set; }
        //multi destination
        public List<Flight> Flight_SegRef1 { get; set; }
        public List<Flight> Flight_SegRef2 { get; set; }
        public List<Flight> Flight_SegRef3 { get; set; }
        public List<Flight> Flight_SegRef4 { get; set; }
        public List<Flight> Flight_SegRef5 { get; set; }
        public List<Flight> Flight_SegRef6 { get; set; }

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
                    if (this.Flight_SegRef1 != null && this.Flight_SegRef1.Count > 0)
                    {
                        return this.Flight_SegRef1[0].flightDetails[0].airline;
                    }
                    else
                    {
                        return null;
                    }
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
        public int minSegRef3Time //maxDepTime
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef3 != null)
                {
                    time = int.Parse(this.Flight_SegRef3.Min(x => x.totalTime));
                }
                return time;
            }
        }
        public int maxSegRef3Time
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef3 != null)
                {
                    time = int.Parse(this.Flight_SegRef3.Max(x => x.totalTime));
                }
                return time;
            }
        }
        public int minSegRef4Time //maxDepTime
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef4 != null)
                {
                    time = int.Parse(this.Flight_SegRef4.Min(x => x.totalTime));
                }
                return time;
            }
        }
        public int maxSegRef4Time //maxDepTime
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef4 != null)
                {
                    time = int.Parse(this.Flight_SegRef4.Max(x => x.totalTime));
                }
                return time;
            }
        }
        public int minSegRef5Time //maxDepTime
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef5 != null)
                {
                    time = int.Parse(this.Flight_SegRef5.Min(x => x.totalTime));
                }
                return time;
            }
        }
        public int maxSegRef5Time //maxDepTime
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef5 != null)
                {
                    time = int.Parse(this.Flight_SegRef5.Max(x => x.totalTime));
                }
                return time;
            }
        }
        public int minSegRef6Time //maxDepTime
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef6 != null)
                {
                    time = int.Parse(this.Flight_SegRef6.Min(x => x.totalTime));
                }
                return time;
            }
        }
        public int maxSegRef6Time //maxDepTime
        {
            get
            {
                int time = 0;
                if (this.Flight_SegRef6 != null)
                {
                    time = int.Parse(this.Flight_SegRef6.Max(x => x.totalTime));
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
            //Route 1
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
            //Route 2
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
            //Route 3
            if (this.Flight_SegRef3 != null)
            {
                foreach (var flight in Flight_SegRef3)
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
                    int totalTime = int.Parse(flight.totalTime);
                    int percentage = (totalTime * 100) / allMaxDepTime;
                    int totalConnectionTime = flight.flightDetails.Sum(x => int.Parse(String.IsNullOrEmpty(x.connectingTime) ? "0" : x.connectingTime));
                    int flightTime = (totalTime - totalConnectionTime) / flight.flightDetails.Count;
                    //set last stop
                    flight.flightDetails[flight.flightDetails.Count - 1].connectingTime = "0";
                }

            }
            //RET
            if (this.returnFlight != null)
            {

                //set up scale
                foreach (var flight in returnFlight)
                {
                    int totalTime = int.Parse(flight.totalTime);
                    int percentage = (totalTime * 100) / allMaxRetTime;
                    int totalConnectionTime = flight.flightDetails.Sum(x => int.Parse(String.IsNullOrEmpty(x.connectingTime) ? "0" : x.connectingTime));
                    int flightTime = (totalTime - totalConnectionTime) / flight.flightDetails.Count;
                    foreach (var detail in flight.flightDetails)
                    //set last stop
                    flight.flightDetails[flight.flightDetails.Count - 1].connectingTime = "0";
                }
            }
        }
        public void UIScale(int allMaxSegRef1Time, int allMaxSegRef2Time, int allMaxSegRef3Time,int allMaxSegRef4Time, int allMaxSegRef5Time, int allMaxSegRef6Time)
        {
            //SegRef1
            if (this.Flight_SegRef1 != null)
            {
                //set up scale
                foreach (var flight in Flight_SegRef1)
                {
                    int totalTime = int.Parse(flight.totalTime);
                    int percentage = (totalTime * 100) / allMaxSegRef1Time;
                    int totalConnectionTime = flight.flightDetails.Sum(x => int.Parse(String.IsNullOrEmpty(x.connectingTime) ? "0" : x.connectingTime));
                    int flightTime = (totalTime - totalConnectionTime) / flight.flightDetails.Count;
                 
                    //set last stop
                    flight.flightDetails[flight.flightDetails.Count - 1].connectingTime = "0";
                }

            }
            //SegRef2
            if (this.Flight_SegRef2 != null)
            {

                //set up scale
                foreach (var flight in Flight_SegRef2)
                {
                    int totalTime = int.Parse(flight.totalTime);
                    int percentage = (totalTime * 100) / allMaxSegRef2Time;
                    int totalConnectionTime = flight.flightDetails.Sum(x => int.Parse(String.IsNullOrEmpty(x.connectingTime) ? "0" : x.connectingTime));
                    int flightTime = (totalTime - totalConnectionTime) / flight.flightDetails.Count;
                   
                    //set last stop
                    flight.flightDetails[flight.flightDetails.Count - 1].connectingTime = "0";
                }
            }
            //SegRef3
            if (this.Flight_SegRef3 != null)
            {

                //set up scale
                foreach (var flight in Flight_SegRef3)
                {
                    int totalTime = int.Parse(flight.totalTime);
                    int percentage = (totalTime * 100) / allMaxSegRef3Time;
                    int totalConnectionTime = flight.flightDetails.Sum(x => int.Parse(String.IsNullOrEmpty(x.connectingTime) ? "0" : x.connectingTime));
                    int flightTime = (totalTime - totalConnectionTime) / flight.flightDetails.Count;
                    
                    //set last stop
                    flight.flightDetails[flight.flightDetails.Count - 1].connectingTime = "0";
                }
            }
            //SegRef4
            if (this.Flight_SegRef4 != null)
            {

                //set up scale
                foreach (var flight in Flight_SegRef4)
                {
                    int totalTime = int.Parse(flight.totalTime);
                    int percentage = (totalTime * 100) / allMaxSegRef4Time;
                    int totalConnectionTime = flight.flightDetails.Sum(x => int.Parse(String.IsNullOrEmpty(x.connectingTime) ? "0" : x.connectingTime));
                    int flightTime = (totalTime - totalConnectionTime) / flight.flightDetails.Count;

                    //set last stop
                    flight.flightDetails[flight.flightDetails.Count - 1].connectingTime = "0";
                }
            }
            //SegRef5
            if (this.Flight_SegRef5 != null)
            {

                //set up scale
                foreach (var flight in Flight_SegRef5)
                {
                    int totalTime = int.Parse(flight.totalTime);
                    int percentage = (totalTime * 100) / allMaxSegRef5Time;
                    int totalConnectionTime = flight.flightDetails.Sum(x => int.Parse(String.IsNullOrEmpty(x.connectingTime) ? "0" : x.connectingTime));
                    int flightTime = (totalTime - totalConnectionTime) / flight.flightDetails.Count;

                    //set last stop
                    flight.flightDetails[flight.flightDetails.Count - 1].connectingTime = "0";
                }
            }
            //SegRef6
            if (this.Flight_SegRef6 != null)
            {

                //set up scale
                foreach (var flight in Flight_SegRef6)
                {
                    int totalTime = int.Parse(flight.totalTime);
                    int percentage = (totalTime * 100) / allMaxSegRef6Time;
                    int totalConnectionTime = flight.flightDetails.Sum(x => int.Parse(String.IsNullOrEmpty(x.connectingTime) ? "0" : x.connectingTime));
                    int flightTime = (totalTime - totalConnectionTime) / flight.flightDetails.Count;

                    //set last stop
                    flight.flightDetails[flight.flightDetails.Count - 1].connectingTime = "0";
                }
            }
        }

        public string type { get; set; }//A=Amadeus,K=Kiwi
    }

}
