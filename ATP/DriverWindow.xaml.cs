using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.IO;

namespace ATP
{
    public partial class DriverWindow : Window
    {
        private int _driverId;
        private string _driverName = "Иванов И.И.";
        private List<Route> _routes;
        private Vehicle _assignedVehicle;

        public DriverWindow(int driverId)
        {
            Console.WriteLine($"Создание окна водителя для ID: {driverId}");

            _driverId = driverId;
            InitializeMockData();

            // Важно: InitializeComponent() должен быть перед загрузкой данных
            InitializeComponent();

            LoadData();

            Console.WriteLine("Окно водителя инициализировано");
        }

        private void InitializeMockData()
        {
            _driverName = "Иванов И.И."; // Или получайте из "базы данных"

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
            TotalDistanceText.Text = _routes.Where(r => r.Distance > 0).Sum(r => r.Distance).ToString("N0");
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

        private void ReportProblem_Click(object sender, RoutedEventArgs e)
        {
            var problemDialog = new ProblemReportWindow(_assignedVehicle);
            if (problemDialog.ShowDialog() == true)
            {
                // Обработка отправленного отчета
                string problemDescription = problemDialog.ProblemDescription;
                string problemType = problemDialog.ProblemType;

                // Сохранение отчета о проблеме (в реальном приложении - в базу данных)
                string report = $"{DateTime.Now:yyyy-MM-dd HH:mm}|{_driverName}|{problemType}|{problemDescription}";

                try
                {
                    // Сохранение в файл (временное решение)
                    File.AppendAllText("problem_reports.txt", report + Environment.NewLine);

                    MessageBox.Show("Проблема успешно зарегистрирована", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении отчета: {ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DriverWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Загрузка данных при открытии окна
                LoadData();

                // Установка заголовка с именем водителя
                this.Title = $"АТП - Водитель: {_driverName}";

                // Обновление статуса
                StatusText.Text = "Данные успешно загружены";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public class ProblemReportWindow : Window
        {
            public string ProblemDescription { get; private set; }
            public string ProblemType { get; private set; }

            public ProblemReportWindow(Vehicle vehicle)
            {
                this.Width = 400;
                this.Height = 300;
                this.Title = "Сообщить о проблеме";
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                var grid = new Grid();

                // Компоненты интерфейса
                var vehicleInfo = new TextBlock
                {
                    Text = $"Транспорт: {vehicle.Brand} {vehicle.Model} ({vehicle.Number})",
                    Margin = new Thickness(10),
                    FontWeight = FontWeights.Bold
                };

                var problemTypeLabel = new Label { Content = "Тип проблемы:", Margin = new Thickness(10, 40, 10, 0) };
                var problemTypeCombo = new ComboBox
                {
                    Margin = new Thickness(10, 65, 10, 0),
                    ItemsSource = new[] { "Техническая", "Дорожная", "Другая" },
                    SelectedIndex = 0
                };

                var descriptionLabel = new Label { Content = "Описание:", Margin = new Thickness(10, 100, 10, 0) };
                var descriptionBox = new TextBox
                {
                    Margin = new Thickness(10, 125, 10, 0),
                    Height = 80,
                    AcceptsReturn = true,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                };

                var submitButton = new Button
                {
                    Content = "Отправить",
                    Margin = new Thickness(10, 210, 10, 10),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Width = 100
                };

                submitButton.Click += (s, e) =>
                {
                    ProblemType = problemTypeCombo.SelectedItem.ToString();
                    ProblemDescription = descriptionBox.Text;

                    if (string.IsNullOrWhiteSpace(ProblemDescription))
                    {
                        MessageBox.Show("Введите описание проблемы", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    this.DialogResult = true;
                    this.Close();
                };

                // Добавление элементов в сетку
                grid.Children.Add(vehicleInfo);
                grid.Children.Add(problemTypeLabel);
                grid.Children.Add(problemTypeCombo);
                grid.Children.Add(descriptionLabel);
                grid.Children.Add(descriptionBox);
                grid.Children.Add(submitButton);

                this.Content = grid;
            }
        }
    }

}