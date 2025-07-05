using Assignment6AirlineReservation.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6AirlineReservation.DataAccess
{
    /// <summary>
    /// Manages passenger data retrieval and database operations.
    /// </summary>
    public class clsPassengerManager
    {
        /// <summary>
        /// Gets passengers for a specific flight from the database.
        /// </summary>
        /// <param name="flightID">Flight ID to get passengers for.</param>
        /// <returns>List of passengers assigned to the flight.</returns>
        public static List<clsPassenger> GetPassengersByFlightID(int flightID)
        {
            List<clsPassenger> passengers = new List<clsPassenger>();

            try
            {
                using (OleDbConnection conn = clsDataAccess.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT
                                        p.Passenger_ID,
                                        p.First_Name, 
                                        p.Last_Name, fpl.Flight_ID,
                                        fpl.Seat_Number FROM Passenger p 
                                        INNER JOIN Flight_Passenger_Link fpl
                                        ON p.Passenger_ID = fpl.Passenger_ID
                                        WHERE fpl.Flight_ID = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.Add("?", OleDbType.Integer).Value = flightID;

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                passengers.Add(new clsPassenger
                                {
                                    PassengerID = Convert.ToInt32(reader["Passenger_ID"]),
                                    FirstName = reader["First_Name"].ToString(),
                                    LastName = reader["Last_Name"].ToString(),
                                    SeatNumber = reader["Seat_Number"].ToString(),
                                    FlightID = Convert.ToInt32(reader["Flight_ID"])
                                });
                            }
                        }
                    }
                }
                return passengers;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving passengers: " + ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a passenger by their seat number for a specific flight.
        /// </summary>
        /// <param name="flightID">Flight ID.</param>
        /// <param name="seatNumber">Seat number.</param>
        /// <returns>Passenger object if found; otherwise null.</returns>
        public static clsPassenger GetPassengerBySeat(int flightID, string seatNumber)
        {
            try
            {
                using (OleDbConnection conn = clsDataAccess.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT p.Passenger_ID, p.First_Name, p.Last_Name, fpl.Seat_Number
                                     FROM Passenger p
                                     INNER JOIN Flight_Passenger_Link fpl ON p.Passenger_ID = fpl.Passenger_ID
                                     WHERE fpl.Flight_ID = ? AND fpl.Seat_Number = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", flightID);
                        cmd.Parameters.AddWithValue("?", seatNumber);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new clsPassenger
                                {
                                    PassengerID = Convert.ToInt32(reader["Passenger_ID"]),
                                    FirstName = reader["First_Name"].ToString(),
                                    LastName = reader["Last_Name"].ToString(),
                                    SeatNumber = reader["Seat_Number"].ToString(),
                                    FlightID = flightID
                                };
                            }
                        }
                    }

                    return null; // No passenger found for this seat
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving passenger by seat: " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes a passenger from the database, including their seat assignment.
        /// </summary>
        /// <param name="passengerID">Passenger ID to delete.</param>
        public static void DeletePassenger(int passengerID)
        {
            try
            {
                using (OleDbConnection conn = clsDataAccess.GetConnection())
                {
                    conn.Open();

                    // Delete link first due to FK constraint
                    string deleteLink = "DELETE FROM Flight_Passenger_Link WHERE Passenger_ID = ?";
                    using (OleDbCommand cmd = new OleDbCommand(deleteLink, conn))
                    {
                        cmd.Parameters.AddWithValue("?", passengerID);
                        cmd.ExecuteNonQuery();
                    }

                    // Then delete passenger
                    string deletePassenger = "DELETE FROM Passenger WHERE Passenger_ID = ?";
                    using (OleDbCommand cmd = new OleDbCommand(deletePassenger, conn))
                    {
                        cmd.Parameters.AddWithValue("?", passengerID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting passenger: " + ex.Message);
            }
        }

        /// <summary>
        /// Updates a passenger's seat number in the database.
        /// </summary>
        /// <param name="passengerID">Passenger ID to update.</param>
        /// <param name="newSeatNumber">New seat number to assign.</param>
        public static void UpdatePassengerSeat(int passengerID, string newSeatNumber)
        {
            try
            {
                using (OleDbConnection conn = clsDataAccess.GetConnection())
                {
                    conn.Open();

                    string query = "UPDATE Flight_Passenger_Link SET Seat_Number = ? WHERE Passenger_ID = ?";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", newSeatNumber);
                        cmd.Parameters.AddWithValue("?", passengerID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating passenger seat: " + ex.Message);
            }
        }

        /// <summary>
        /// Adds a passenger to the database without assigning a seat.
        /// </summary>
        /// <param name="firstName">Passenger first name.</param>
        /// <param name="lastName">Passenger last name.</param>
        /// <returns>Newly created passenger ID.</returns>
        public static int AddPassengerWithoutSeat(string firstName, string lastName)
        {
            try
            {
                using (OleDbConnection conn = clsDataAccess.GetConnection())
                {
                    conn.Open();

                    string insertPassenger = "INSERT INTO Passenger (First_Name, Last_Name) VALUES (?, ?)";
                    using (OleDbCommand cmd = new OleDbCommand(insertPassenger, conn))
                    {
                        cmd.Parameters.AddWithValue("?", firstName);
                        cmd.Parameters.AddWithValue("?", lastName);
                        cmd.ExecuteNonQuery();
                    }

                    string selectID = "SELECT @@IDENTITY";
                    using (OleDbCommand cmd = new OleDbCommand(selectID, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding passenger without seat: " + ex.Message);
            }
        }

        /// <summary>
        /// Assigns a seat to an existing passenger by inserting into the Flight_Passenger_Link table.
        /// </summary>
        /// <param name="passengerID">Passenger ID.</param>
        /// <param name="flightID">Flight ID.</param>
        /// <param name="seatNumber">Seat number to assign.</param>
        public static void AssignSeatToPassenger(int passengerID, int flightID, string seatNumber)
        {
            try
            {
                using (OleDbConnection conn = clsDataAccess.GetConnection())
                {
                    conn.Open();

                    string insertLink = "INSERT INTO Flight_Passenger_Link (Flight_ID, Passenger_ID, Seat_Number) VALUES (?, ?, ?)";
                    using (OleDbCommand cmd = new OleDbCommand(insertLink, conn))
                    {
                        cmd.Parameters.AddWithValue("?", flightID);
                        cmd.Parameters.AddWithValue("?", passengerID);
                        cmd.Parameters.AddWithValue("?", seatNumber);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error assigning seat to passenger: " + ex.Message);
            }
        }
    }
}
