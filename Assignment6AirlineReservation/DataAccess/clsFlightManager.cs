using Assignment6AirlineReservation.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace Assignment6AirlineReservation.DataAccess
{
    /// <summary>
    /// Manages flight data retrieval.
    /// </summary>
    public class clsFlightManager
    {
        /// <summary>
        /// Gets all flights from the database.
        /// </summary>
        /// <returns>List of flights</returns>
        public static List<clsFlight> GetFlights()
        {
            List<clsFlight> flights = new List<clsFlight>();

            try
            {
                using (OleDbConnection conn = clsDataAccess.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT Flight_ID, Flight_Number, Aircraft_Type FROM Flight";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            flights.Add(new clsFlight
                            {

                                FlightID = Convert.ToInt32(reader["Flight_ID"]),
                                FlightNumber = Convert.ToInt32(reader["Flight_Number"]),
                                AircraftType = reader["Aircraft_Type"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving flights: " + ex.Message);
            }

            return flights;
        }
    }
}
