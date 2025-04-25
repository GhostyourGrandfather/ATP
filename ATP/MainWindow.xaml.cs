using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace ATP
{
    public partial class MainWindow : Window
    {
        private List<Vehicle> _vehicles = new List<Vehicle>
        {
            new Vehicle { Id = 1, Brand = "ГАЗ", Model = "ГАЗель NEXT", Year = 2022, Number = "А123БВ777" },
            new Vehicle { Id = 2, Brand = "КАМАЗ", Model = "65201", Year = 2021, Number = "В456ГД888" }
        };

        private List<Driver> _drivers = new List<Driver>
        {
            new Driver { Id = 1, Name = "Иванов Иван Иванович", Experience = 5, LicenseNumber = "AB123456" },
            new Driver { Id = 2, Name = "Петров Петр Петрович", Experience = 3, LicenseNumber = "CD654321" }
        };

        private List<Route> _routes = new List<Route>();

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            VehiclesDataGrid.ItemsSource = _vehicles;
            DriversDataGrid.ItemsSource = _drivers;
            RoutesDataGrid.ItemsSource = _routes;
        }

        private void AddVehicle_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BrandTextBox.Text) )return;

            var newVehicle = new Vehicle
            {
                Id = _vehicles.Count + 1,
                Brand = BrandTextBox.Text,
                Model = ModelTextBox.Text,
                Year = int.TryParse(YearTextBox.Text, out int year) ? year : 0,
                Number = NumberTextBox.Text
            };

            _vehicles.Add(newVehicle);
            VehiclesDataGrid.Items.Refresh();
        }

        private void DeleteVehicle_Click(object sender, RoutedEventArgs e)
        {
            if (VehiclesDataGrid.SelectedItem is Vehicle selected)
            {
                _vehicles.Remove(selected);
                VehiclesDataGrid.Items.Refresh();
            }
        }

        private void AddDriver_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text)) return;

            var newDriver = new Driver
            {
                Id = _drivers.Count + 1,
                Name = NameTextBox.Text,
                Experience = int.TryParse(ExperienceTextBox.Text, out int exp) ? exp : 0,
                LicenseNumber = LicenseNumberTextBox.Text
            };

            _drivers.Add(newDriver);
            DriversDataGrid.Items.Refresh();
        }

        private void DeleteDriver_Click(object sender, RoutedEventArgs e)
        {
            if (DriversDataGrid.SelectedItem is Driver selected)
            {
                _drivers.Remove(selected);
                DriversDataGrid.Items.Refresh();
            }
        }

        private void AddRoute_Click(object sender, RoutedEventArgs e)
        {
            if (DriverComboBox.SelectedItem == null || VehicleComboBox.SelectedItem == null) return;

            var newRoute = new Route
            {
                Id = _routes.Count + 1,
                StartPoint = StartPointTextBox.Text,
                EndPoint = EndPointTextBox.Text,
                Distance = decimal.TryParse(DistanceTextBox.Text, out decimal dist) ? dist : 0,
                TravelTime = decimal.TryParse(TravelTimeTextBox.Text, out decimal time) ? time : 0,
                Driver = (Driver)DriverComboBox.SelectedItem,
                Vehicle = (Vehicle)VehicleComboBox.SelectedItem
            };

            _routes.Add(newRoute);
            RoutesDataGrid.Items.Refresh();
            UpdateRouteStatistics();
        }

        private void UpdateRouteStatistics()
        {
            TotalRoutesText.Text = _routes.Count.ToString();
            // Дополнительная статистика...
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}