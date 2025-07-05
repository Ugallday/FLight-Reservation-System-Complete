using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6AirlineReservation.Models
{
    public class clsFlight
    {
        /// <summary>Flight number</summary>
        public int FlightNumber { get; set; }

        /// <summary>Aircraft type</summary>
        public string AircraftType { get; set; }

        public int FlightID { get; set; }

        /// <summary>
        /// Returns formatted display string.
        /// </summary>
        public string Display => $"{FlightNumber} - {AircraftType}";
    }
}
