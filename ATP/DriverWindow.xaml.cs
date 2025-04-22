using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace ATP
{
    public partial class DriverWindow : Window
    {
        private int _driverId;
        private List<Route> _routes;
        private Vehicle _assignedVehicle;

        public DriverWindow(int driverId)
        {
            _driverId = driverId;
            InitializeMockData();
            InitializeComponent();
            LoadData();
        }

        private void InitializeMockData()
        {
            _assignedVehicle = new Vehicle
            {
                Id = 1,
                Brand = "ГАЗ",
                Model = "ГАЗель NEXT",
                Year = 2022,
                Number = "А123БВ777"
            };

            _routes = new List<Route>
            {
                new Route
                {
                    Id = 1,
                    StartPoint = "Москва",
                    EndPoint = "Санкт-Петербург",
                    Distance = 700,
                    Status = "В процессе",
                    Vehicle = _assignedVehicle
                },
                new Route
                {
                    Id = 2,
                    StartPoint = "Москва",
                    EndPoint = "Казань",
                    Distance = 800,
                    Status = "Завершен",
                    Vehicle = _assignedVehicle
                }
            };
        }

        private void LoadData()
        {
            CurrentRoutesDataGrid.ItemsSource = _routes;
            RouteHistoryDataGrid.ItemsSource = _routes.FindAll(r => r.Status == "Завершен");

            VehicleBrandText.Text = _assignedVehicle.Brand;
            VehicleModelText.Text = _assignedVehicle.Model;
            VehicleYearText.Text = _assignedVehicle.Year.ToString();
            VehicleNumberText.Text = _assignedVehicle.Number;

            TotalRoutesText.Text = _routes.Count.ToString();
            TotalDistanceText.Text = _routes.Sum(r => r.Distance).ToString();
        }

        private void StartRoute_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentRoutesDataGrid.SelectedItem is Route route)
            {
                route.Status = "В процессе";
                CurrentRoutesDataGrid.Items.Refresh();
            }
        }

        private void CompleteRoute_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentRoutesDataGrid.SelectedItem is Route route)
            {
                route.Status = "Завершен";
                CurrentRoutesDataGrid.Items.Refresh();
                RouteHistoryDataGrid.Items.Refresh();
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}