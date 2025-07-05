using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6AirlineReservation.Models
{
    public class clsPassenger
    {
        /// <summary>
        /// gets or sets the passenger ID
        /// </summary>
        public int PassengerID { get; set; }

        /// <summary>
        /// gets or sets the first name 
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the last name
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// gets or sets the seat number
        /// </summary>
        public string SeatNumber { get; set; }

        /// <summary>
        /// gets or sets the flight id the passenger is on
        /// </summary>
        public int FlightID { get; set; }

        /// <summary>
        /// Returns full name and seat number.
        /// </summary>
        public string Display => $"{FirstName} {LastName} - Seat {SeatNumber}";
    }
}
