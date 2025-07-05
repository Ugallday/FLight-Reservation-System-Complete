using Assignment6AirlineReservation.DataAccess;
using Assignment6AirlineReservation.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Assignment6AirlineReservation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Indicates if Change Seat mode is active.
        /// </summary>
        private bool isChangeSeatMode = false;

        /// <summary>
        /// Stores passenger selected for seat change.
        /// </summary>
        private clsPassenger passengerToChange = null;

        /// <summary>
        /// Stores passenger ID to assign seat for newly added passenger.
        /// </summary>
        private int passengerIDToAssignSeat = -1;

        /// <summary>
        /// Constructor. Initializes the main window and loads flights.
        /// </summary>
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                LoadFlights();
                cbChoosePassenger.IsEnabled = false;
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Loads flights into the flight combo box.
        /// </summary>
        private void LoadFlights()
        {
            try
            {
                List<clsFlight> flights = clsFlightManager.GetFlights();
                cbChooseFlight.ItemsSource = flights;
                cbChooseFlight.DisplayMemberPath = "Display";
                cbChooseFlight.SelectedValuePath = "FlightID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading flights: " + ex.Message);
            }
        }

        /// <summary>
        /// Loads passengers for the selected flight and updates seat map.
        /// </summary>
        /// <param name="flightNumber">Flight number.</param>
        private void LoadPassengers(int flightNumber)
        {
            try
            {
                ResetSeatColors();
                List<clsPassenger> passengers = clsPassengerManager.GetPassengersByFlightID(flightNumber);
                cbChoosePassenger.ItemsSource = passengers;
                cbChoosePassenger.DisplayMemberPath = "Display";
                cbChoosePassenger.SelectedValuePath = "PassengerID";
                cbChoosePassenger.Items.Refresh();

                Canvas activeCanvasSeats = CanvasA380.Visibility == Visibility.Visible ? cA380_Seats :
                                           Canvas767.Visibility == Visibility.Visible ? c767_Seats : null;

                if (activeCanvasSeats != null)
                {
                    foreach (var child in activeCanvasSeats.Children)
                    {
                        if (child is Label seatLabel)
                        {
                            seatLabel.Background = Brushes.Blue;
                        }
                    }

                    foreach (var passenger in passengers)
                    {
                        foreach (var child in activeCanvasSeats.Children)
                        {
                            if (child is Label seatLabel && seatLabel.Content.ToString() == passenger.SeatNumber)
                            {
                                seatLabel.Background = Brushes.Red;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading passengers: " + ex.Message);
            }
        }

        /// <summary>
        /// Displays seat map based on aircraft type.
        /// </summary>
        /// <param name="flight">Selected flight.</param>
        private void DisplaySeatMap(clsFlight flight)
        {
            if (flight.AircraftType == "A380")
            {
                CanvasA380.Visibility = Visibility.Visible;
                Canvas767.Visibility = Visibility.Collapsed;
            }
            else if (flight.AircraftType == "767")
            {
                Canvas767.Visibility = Visibility.Visible;
                CanvasA380.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handles flight selection change to load passengers and display seat map.
        /// </summary>
        private void cbChooseFlight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbChooseFlight.SelectedItem is clsFlight selectedFlight)
                {
                    DisplaySeatMap(selectedFlight);
                    LoadPassengers(selectedFlight.FlightID);
                    cbChoosePassenger.IsEnabled = true;
                    gPassengerCommands.IsEnabled = true;
                    Flight_Title.Content = $" {selectedFlight.FlightNumber} - {selectedFlight.AircraftType}";
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Handles passenger selection change to highlight seat on map.
        /// </summary>
        private void cbChoosePassenger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbChoosePassenger.SelectedItem is clsPassenger selectedPassenger)
            {
                string selectedSeatNumber = selectedPassenger.SeatNumber;

                Canvas activeCanvasSeats = CanvasA380.Visibility == Visibility.Visible ? cA380_Seats :
                                           Canvas767.Visibility == Visibility.Visible ? c767_Seats : null;

                if (activeCanvasSeats != null)
                {
                    int flightID = (int)cbChooseFlight.SelectedValue;
                    List<clsPassenger> passengers = clsPassengerManager.GetPassengersByFlightID(flightID);

                    foreach (var child in activeCanvasSeats.Children)
                    {
                        if (child is Label seatLabel)
                        {
                            seatLabel.Background = Brushes.Blue;
                        }
                    }

                    foreach (var passenger in passengers)
                    {
                        foreach (var child in activeCanvasSeats.Children)
                        {
                            if (child is Label seatLabel && seatLabel.Content.ToString() == passenger.SeatNumber)
                            {
                                seatLabel.Background = Brushes.Red;
                                break;
                            }
                        }
                    }

                    foreach (var child in activeCanvasSeats.Children)
                    {
                        if (child is Label seatLabel && seatLabel.Content.ToString() == selectedSeatNumber)
                        {
                            seatLabel.Background = Brushes.Green;
                            break;
                        }
                    }
                }

                lblPassengersSeatNumber.Content = selectedSeatNumber;
            }
            else
            {
                lblPassengersSeatNumber.Content = "";
            }
        }

        /// <summary>
        /// Adds a new passenger.
        /// </summary>
        private void cmdAddPassenger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbChooseFlight.SelectedItem is clsFlight selectedFlight)
                {
                    wndAddPassenger wndAddPass = new wndAddPassenger(selectedFlight.FlightID);
                    wndAddPass.ShowDialog();
                    LoadPassengers(selectedFlight.FlightID);
                }
                else
                {
                    MessageBox.Show("Please select a flight first.");
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Deletes the selected passenger.
        /// </summary>
        private void cmdDeletePassenger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbChoosePassenger.SelectedItem is clsPassenger selectedPassenger)
                {
                    var result = MessageBox.Show($"Delete passenger {selectedPassenger.FirstName} {selectedPassenger.LastName}?", "Confirm Delete", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        clsPassengerManager.DeletePassenger(selectedPassenger.PassengerID);
                        ResetSeatColor(selectedPassenger.SeatNumber, selectedPassenger.FlightID);
                        LoadPassengers(selectedPassenger.FlightID);
                        cbChoosePassenger.SelectedIndex = -1;
                        gPassengerCommands.IsEnabled = false;
                        MessageBox.Show("Passenger deleted successfully.");
                    }
                }
                else
                {
                    MessageBox.Show("Please select a passenger to delete.");
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Changes the seat of selected passenger.
        /// </summary>
        private void cmdChangeSeat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbChoosePassenger.SelectedItem is clsPassenger selectedPassenger)
                {
                    isChangeSeatMode = true;
                    passengerToChange = selectedPassenger;

                    MessageBox.Show("Change Seat mode activated. Click an empty seat to assign the passenger.");

                    cbChooseFlight.IsEnabled = false;
                    cbChoosePassenger.IsEnabled = false;
                    cmdAddPassenger.IsEnabled = false;
                    cmdDeletePassenger.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("Please select a passenger first.");
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Begins seat assignment for a newly added passenger.
        /// </summary>
        /// <param name="passengerID">Passenger ID to assign seat for.</param>
        public void BeginAssignSeat(int passengerID)
        {
            try
            {
                passengerIDToAssignSeat = passengerID;
                MessageBox.Show("Please click an empty seat to assign to the new passenger.");

                cbChooseFlight.IsEnabled = false;
                cbChoosePassenger.IsEnabled = false;
                cmdAddPassenger.IsEnabled = false;
                cmdChangeSeat.IsEnabled = false;
                cmdDeletePassenger.IsEnabled = false;
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Handles seat click events for assigning or changing seats.
        /// </summary>
        private void Seat_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label clickedSeat)
            {
                string seatNumber = clickedSeat.Content.ToString();

                if (passengerIDToAssignSeat != -1)
                {
                    int flightID = (cbChooseFlight.SelectedItem as clsFlight).FlightID;
                    clsPassengerManager.AssignSeatToPassenger(passengerIDToAssignSeat, flightID, seatNumber);
                    MessageBox.Show("Passenger seat assigned successfully.");
                    passengerIDToAssignSeat = -1;

                    cbChooseFlight.IsEnabled = true;
                    cbChoosePassenger.IsEnabled = true;
                    cmdAddPassenger.IsEnabled = true;
                    cmdChangeSeat.IsEnabled = true;
                    cmdDeletePassenger.IsEnabled = true;

                    LoadPassengers(flightID);
                }
                else if (isChangeSeatMode)
                {
                    if (clickedSeat.Background == Brushes.Blue)
                    {
                        clsPassengerManager.UpdatePassengerSeat(passengerToChange.PassengerID, seatNumber);
                        MessageBox.Show($"Seat changed to {seatNumber}.");
                        LoadPassengers(passengerToChange.FlightID);
                        isChangeSeatMode = false;
                        passengerToChange = null;

                        cbChooseFlight.IsEnabled = true;
                        cbChoosePassenger.IsEnabled = true;
                        cmdAddPassenger.IsEnabled = true;
                        cmdDeletePassenger.IsEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Seat is already taken. Please choose an empty seat.");
                    }
                }
                else if (clickedSeat.Background == Brushes.Red)
                {
                    int flightID = (cbChooseFlight.SelectedItem as clsFlight).FlightID;
                    clsPassenger passenger = clsPassengerManager.GetPassengerBySeat(flightID, seatNumber);

                    if (passenger != null)
                    {
                        cbChoosePassenger.SelectedItem = passenger;
                    }
                }
            }
        }

        /// <summary>
        /// Resets all seat colors to blue (empty) for both aircrafts.
        /// </summary>
        private void ResetSeatColors()
        {
            try
            {
                foreach (var child in c767_Seats.Children)
                {
                    if (child is Label seatLabel)
                    {
                        seatLabel.Background = Brushes.Blue;
                    }
                }

                foreach (var child in cA380_Seats.Children)
                {
                    if (child is Label seatLabel)
                    {
                        seatLabel.Background = Brushes.Blue;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Resets color of a specific seat to blue (empty).
        /// </summary>
        private void ResetSeatColor(string seatNumber, int flightID)
        {
            try
            {
                Canvas seatCanvas = flightID == 1 ? cA380_Seats : c767_Seats;

                foreach (var child in seatCanvas.Children)
                {
                    if (child is Label seatLabel && seatLabel.Content.ToString() == seatNumber)
                    {
                        seatLabel.Background = Brushes.Blue;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Handles errors by showing a message box or writing to file if displaying fails.
        /// </summary>
        private void HandleError(string sClass, string sMethod, string sMessage)
        {
            try
            {
                MessageBox.Show(sClass + "." + sMethod + " -> " + sMessage);
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\Error.txt", Environment.NewLine + "HandleError Exception: " + ex.Message);
            }
        }
    }
}
