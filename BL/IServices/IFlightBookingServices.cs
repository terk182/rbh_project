using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IFlightBookingServices
    {
        void UpdateBookingStatus(Guid id, int bookingStatus);
        void UpdatePayment(Guid id, int paymentStatus, int paymentMethod, int installmentPlan, decimal installmentMonthly,DateTime paymentDate);
        Guid SaveBooking(Entities.RobinhoodFare.AirFare booking);
        //Guid SaveBookingBundle(Entities.RobinhoodFare.AirFare booking);
        void UpdateFinalPrice(Guid id, decimal finalPrice);
        void UpdateTransaction_Hash(Guid id, string transactionHash);
        List<FlightBooking> GetFlightBookings(string transactionID);
        FlightBooking GetFlightBooking(Guid id);
        void SaveFlightBooking(FlightBooking flightBooking);
    }
}
