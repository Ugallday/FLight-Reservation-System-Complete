using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace Assignment6AirlineReservation.DataAccess
{
    /// <summary>
    /// Manages adding passengers and determining available seats.
    /// </summary>
    public static class clsAddPassengerManager
    {
        /// <summary>
        /// Adds a passenger to the Passenger table and links them to a flight with an assigned seat.
        /// </summary>
        /// <param name="firstName">Passenger's first name.</param>
        /// <param name="lastName">Passenger's last name.</param>
        /// <param name="flightID">Flight ID to link passenger to.</param>
        /// <param name="seatNumber">Seat number assigned to passenger.</param>
        /// <exception cref="Exception">Thrown if there is an error adding the passenger.</exception>
        public static void AddPassenger(string firstName, string lastName, int flightID, string seatNumber)
        {
            try
            {
                using (OleDbConnection conn = clsDataAccess.GetConnection())
                {
                    conn.Open();

                    // Insert into Passenger table
                    string insertPassenger = "INSERT INTO Passenger (First_Name, Last_Name) VALUES (?, ?)";
                    using (OleDbCommand cmd = new OleDbCommand(insertPassenger, conn))
                    {
                        cmd.Parameters.AddWithValue("?", firstName);
                        cmd.Parameters.AddWithValue("?", lastName);
                        cmd.ExecuteNonQuery();
                    }

                    // Retrieve the new passenger ID
                    string selectID = "SELECT @@IDENTITY";
                    int newPassengerID;
                    using (OleDbCommand cmd = new OleDbCommand(selectID, conn))
                    {
                        newPassengerID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Insert into Flight_Passenger_Link table
                    string insertLink = "INSERT INTO Flight_Passenger_Link (Flight_ID, Passenger_ID, Seat_Number) VALUES (?, ?, ?)";
                    using (OleDbCommand cmd = new OleDbCommand(insertLink, conn))
                    {
                        cmd.Parameters.AddWithValue("?", flightID);
                        cmd.Parameters.AddWithValue("?", newPassengerID);
                        cmd.Parameters.AddWithValue("?", seatNumber);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding passenger: " + ex.Message);
            }
        }

        /// <summary>
        /// Determines the next available seat number for a flight.
        /// </summary>
        /// <param name="flightID">Flight ID to check for available seats.</param>
        /// <returns>Next available seat number as string.</returns>
        /// <exception cref="Exception">Thrown if there is an error determining the next available seat or if no seats are available.</exception>
        public static string DetermineNextAvailableSeat(int flightID)
        {
            try
            {
                using (OleDbConnection conn = clsDataAccess.GetConnection())
                {
                    conn.Open();

                    // Get all taken seat numbers for this flight
                    string query = "SELECT Seat_Number FROM Flight_Passenger_Link WHERE Flight_ID = ?";
                    List<int> takenSeats = new List<int>();

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", flightID);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                takenSeats.Add(Convert.ToInt32(reader["Seat_Number"]));
                            }
                        }
                    }

                    // Determine next available seat from 1 to max seat count (adjust per aircraft if needed)
                    int maxSeats = 16; // For 767. Adjust dynamically for A380 if implemented later.
                    for (int i = 1; i <= maxSeats; i++)
                    {
                        if (!takenSeats.Contains(i))
                        {
                            return i.ToString();
                        }
                    }

                    throw new Exception("No available seats.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error determining next available seat: " + ex.Message);
            }
        }
    }
}
