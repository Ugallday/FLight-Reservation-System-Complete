using Assignment6AirlineReservation.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Assignment6AirlineReservation
{
    /// <summary>
    /// Interaction logic for wndAddPassenger.xaml
    /// </summary>
    public partial class wndAddPassenger : Window
    {
        /// <summary>
        /// Gets or sets the selected flight ID to which the passenger will be added.
        /// </summary>
        public int SelectedFlightID { get; set; }

        /// <summary>
        /// Constructor for the add passenger window. Initializes the window and stores the flight ID.
        /// </summary>
        /// <param name="flightID">The flight ID passed from the main window.</param>
        public wndAddPassenger(int flightID)
        {
            try
            {
                InitializeComponent();
                SelectedFlightID = flightID; // store the passed FlightID for later use
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Allows only letter input in the textboxes. Blocks all other keys except backspace, delete, tab, and enter.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Key event arguments containing information about the key pressed.</param>
        private void txtLetterInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //Only allow letters to be entered
                if (!(e.Key >= Key.A && e.Key <= Key.Z))
                {
                    //Allow the user to use the backspace, delete, tab and enter
                    if (!(e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Tab || e.Key == Key.Enter))
                    {
                        //No other keys allowed besides letters, backspace, delete, tab, and enter
                        e.Handled = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                            MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Handles exceptions by showing a message box with the error message. Writes to file if displaying the error fails.
        /// </summary>
        /// <param name="sClass">The class where the error occurred.</param>
        /// <param name="sMethod">The method where the error occurred.</param>
        /// <param name="sMessage">The error message to display.</param>
        private void HandleError(string sClass, string sMethod, string sMessage)
        {
            try
            {
                MessageBox.Show(sClass + "." + sMethod + " -> " + sMessage);
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText("C:\\Error.txt", Environment.NewLine + "HandleError Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// Handles the Save button click event. Adds the passenger and calls the main window method to assign a seat.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Routed event arguments.</param>
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string firstName = txtFirstName.Text.Trim();
                string lastName = txtLastName.Text.Trim();

                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                {
                    MessageBox.Show("Please enter both first and last name.");
                    return;
                }

                // Add passenger without seat, retrieve new PassengerID
                int newPassengerID = clsPassengerManager.AddPassengerWithoutSeat(firstName, lastName);

                // Pass newPassengerID back to MainWindow to assign seat
                ((MainWindow)Application.Current.MainWindow).BeginAssignSeat(newPassengerID);

                this.Close();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name, MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Wrapper method to get the next available seat from DataAccess for a given flight ID.
        /// </summary>
        /// <param name="flightID">The flight ID to check for the next available seat.</param>
        /// <returns>Returns the next available seat number as a string.</returns>
        private string DetermineNextAvailableSeat(int flightID)
        {
            return clsAddPassengerManager.DetermineNextAvailableSeat(flightID);
        }
    }
}
