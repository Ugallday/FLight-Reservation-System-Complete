using System;
using System.Data.OleDb;

namespace Assignment6AirlineReservation
{
    /// <summary>
    /// Provides database connection functionality.
    /// </summary>
    public static class clsDataAccess
    {
        /// <summary>
        /// Connection string to the Access database.
        /// Adjust Data Source path to your .accdb file location.
        /// </summary>
        private static string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=ReservationSystem.accdb";

        /// <summary>
        /// Gets a new OleDbConnection using the connection string.
        /// </summary>
        /// <returns>OleDbConnection object</returns>
        public static OleDbConnection GetConnection()
        {
            try
            {
                return new OleDbConnection(connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating database connection: " + ex.Message);
            }
        }
    }
}
