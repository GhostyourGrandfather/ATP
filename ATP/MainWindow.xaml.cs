using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;

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
                Vehicle = (Vehicle)VehicleComboBox.SelectedItem,
                CreatedDate = DateTime.Now // Добавлено
            };

            _routes.Add(newRoute);
            RoutesDataGrid.Items.Refresh();
            UpdateRouteStatistics();

            // Очистка полей
            StartPointTextBox.Clear();
            EndPointTextBox.Clear();
            DistanceTextBox.Clear();
            TravelTimeTextBox.Clear();
        }

        private void UpdateRouteStatistics()
        {
            TotalRoutesText.Text = _routes.Count.ToString();

            if (_routes.Count > 0)
            {
                decimal totalDistance = 0;
                decimal totalTime = 0;

                foreach (var route in _routes)
                {
                    totalDistance += route.Distance;
                    totalTime += route.TravelTime;
                }

                TotalDistanceText.Text = totalDistance.ToString("N0");
                AvgTimeText.Text = (totalTime / _routes.Count).ToString("N1");
            }
            else
            {
                TotalDistanceText.Text = "0";
                AvgTimeText.Text = "0";
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void ShowRouteOnMap_Click(object sender, RoutedEventArgs e)
        {
            if (RoutesDataGrid.SelectedItem is Route selectedRoute)
            {
                string mapUrl = $"https://www.google.com/maps/dir/{selectedRoute.StartPoint}/{selectedRoute.EndPoint}";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = mapUrl,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("Выберите маршрут для отображения на карте", "Информация",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CalculateOptimalRoute_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StartPointTextBox.Text) ||
                string.IsNullOrWhiteSpace(EndPointTextBox.Text))
            {
                MessageBox.Show("Укажите начальную и конечную точки", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Упрощенный расчет "оптимального" маршрута
            DistanceTextBox.Text = "500"; // Примерное расстояние
            TravelTimeTextBox.Text = "8.5"; // Примерное время

            MessageBox.Show("Оптимальный маршрут рассчитан", "Информация",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteRoute_Click(object sender, RoutedEventArgs e)
        {
            if (RoutesDataGrid.SelectedItem is Route selectedRoute)
            {
                if (MessageBox.Show("Удалить выбранный маршрут?", "Подтверждение",
                                   MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _routes.Remove(selectedRoute);
                    RoutesDataGrid.Items.Refresh();
                    UpdateRouteStatistics();
                }
            }
            else
            {
                MessageBox.Show("Выберите маршрут для удаления", "Информация",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.csv)|*.csv",
                FileName = $"Отчет_маршрутов_{DateTime.Now:yyyyMMdd}.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        writer.WriteLine("Маршрут;Расстояние;Время;Водитель;Транспорт");

                        foreach (Route route in _routes)
                        {
                            writer.WriteLine(
                                $"{route.StartPoint} → {route.EndPoint};" +
                                $"{route.Distance};" +
                                $"{route.TravelTime};" +
                                $"{route.Driver?.Name};" +
                                $"{route.Vehicle?.Brand} {route.Vehicle?.Model}"
                            );
                        }
                    }

                    MessageBox.Show("Данные успешно экспортированы", "Успех",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип отчета", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string reportType = ((ComboBoxItem)ReportTypeComboBox.SelectedItem).Content.ToString();
            string period = $"{StartDatePicker.SelectedDate:dd.MM.yyyy} - {EndDatePicker.SelectedDate:dd.MM.yyyy}";

            // Фильтрация данных по дате
            var filteredRoutes = _routes.FindAll(r =>
                r.CreatedDate >= StartDatePicker.SelectedDate &&
                r.CreatedDate <= EndDatePicker.SelectedDate);

            ReportsDataGrid.ItemsSource = filteredRoutes;

            StatusText.Text = $"Сформирован отчет: {reportType} за период {period}";
        }

        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Печать отчета", "Информация",
                          MessageBoxButton.OK, MessageBoxImage.Information);
            // В реальном приложении здесь будет код для печати
        }

        private void RoutingTab_Loaded(object sender, RoutedEventArgs e)
        {
            // Инициализация ComboBox при загрузке вкладки
            DriverComboBox.ItemsSource = _drivers;
            VehicleComboBox.ItemsSource = _vehicles;

            // Выбор первого элемента по умолчанию
            if (_drivers.Count > 0) DriverComboBox.SelectedIndex = 0;
            if (_vehicles.Count > 0) VehicleComboBox.SelectedIndex = 0;
        }
    }
}