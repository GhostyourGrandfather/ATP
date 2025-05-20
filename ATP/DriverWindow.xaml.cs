using System;
using System.Collections.Generic;
using System.Windows;

namespace ATP
{
    public partial class DriverWindow : Window
    {
        private int _driverId;
        private List<Vehicle> _vehicles;
        private List<Route> _routes;
        private Driver _currentDriver;

        public class Vehicle
        {
            public int Id { get; set; }
            public string Brand { get; set; }
            public string Model { get; set; }
            public string Number { get; set; }
        }

        public class Driver
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string LicenseNumber { get; set; }
        }

        public class Route
        {
            public int Id { get; set; }
            public string StartPoint { get; set; }
            public string EndPoint { get; set; }
            public decimal Distance { get; set; }
            public string Status { get; set; }
            public Driver Driver { get; set; }
            public Vehicle Vehicle { get; set; }
        }

        public DriverWindow(int driverId)
        {
            // Важно: InitializeComponent() должен быть первым!
            InitializeComponent();

            _driverId = driverId;
            InitializeMockData();
            LoadDriverData();

            this.Title = $"АТП - Водитель: {_currentDriver?.Name}";
        }

        private void InitializeMockData()
        {
            // Мок-данные для водителя
            _currentDriver = new Driver
            {
                Id = _driverId,
                Name = "Иванов И.И.",
                LicenseNumber = "AB123456"
            };

            // Мок-данные для транспорта
            _vehicles = new List<Vehicle>
            {
                new Vehicle { Id = 1, Brand = "ГАЗ", Model = "ГАЗель NEXT", Number = "А123БВ777" }
            };

            // Мок-данные для маршрутов
            _routes = new List<Route>
            {
                new Route
                {
                    Id = 1,
                    StartPoint = "Москва",
                    EndPoint = "Санкт-Петербург",
                    Distance = 700,
                    Status = "Назначен",
                    Driver = _currentDriver,
                    Vehicle = _vehicles[0]
                }
            };
        }

        private void LoadDriverData()
        {
            try
            {
                // Загрузка данных в интерфейс
                DriverNameText.Text = _currentDriver.Name;
                CurrentRoutesDataGrid.ItemsSource = _routes;

                // Статистика
                TotalRoutesText.Text = _routes.Count.ToString();
                TotalDistanceText.Text = CalculateTotalDistance().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private decimal CalculateTotalDistance()
        {
            decimal total = 0;
            foreach (var route in _routes)
            {
                total += route.Distance;
            }
            return total;
        }

        private void StartRoute_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentRoutesDataGrid.SelectedItem is Route selectedRoute)
            {
                selectedRoute.Status = "В пути";
                CurrentRoutesDataGrid.Items.Refresh();
            }
        }

        private void CompleteRoute_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentRoutesDataGrid.SelectedItem is Route selectedRoute)
            {
                selectedRoute.Status = "Завершен";
                CurrentRoutesDataGrid.Items.Refresh();
                LoadDriverData(); // Обновляем статистику
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadDriverData();
        }
    }
}