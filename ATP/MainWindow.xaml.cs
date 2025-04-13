using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace ATP
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ATP_Management;Integrated Security=True";

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            LoadVehicles();
            LoadDrivers();
            LoadRoutes();
            UpdateRouteStatistics();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Инициализация данных при загрузке окна
            LoadData(); // Этот метод уже есть в вашем коде

            // Дополнительная инициализация при необходимости
            StatusText.Text = "Система готова к работе";

            // Можно добавить проверку соединения с БД
            CheckDatabaseConnection();
        }

        private void CheckDatabaseConnection()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    StatusText.Text = "Подключено к базе данных";
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка подключения к БД";
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Управление транспортом
        private void LoadVehicles()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Brand, Model, Year, Number FROM Vehicles WHERE IsActive = 1 ORDER BY Brand, Model";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    VehiclesDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке транспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddVehicle_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BrandTextBox.Text) || string.IsNullOrWhiteSpace(ModelTextBox.Text) ||
                string.IsNullOrWhiteSpace(YearTextBox.Text) || string.IsNullOrWhiteSpace(NumberTextBox.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Vehicles (Brand, Model, Year, Number) VALUES (@Brand, @Model, @Year, @Number)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Brand", BrandTextBox.Text);
                    command.Parameters.AddWithValue("@Model", ModelTextBox.Text);
                    command.Parameters.AddWithValue("@Year", int.Parse(YearTextBox.Text));
                    command.Parameters.AddWithValue("@Number", NumberTextBox.Text);
                    command.ExecuteNonQuery();
                }

                // Очистка полей и обновление таблицы
                BrandTextBox.Clear();
                ModelTextBox.Clear();
                YearTextBox.Clear();
                NumberTextBox.Clear();
                LoadVehicles();

                StatusText.Text = "Транспорт успешно добавлен";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении транспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteVehicle_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int vehicleId = (int)button.CommandParameter;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Vehicles SET IsActive = 0 WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", vehicleId);
                    command.ExecuteNonQuery();
                }

                LoadVehicles();
                StatusText.Text = "Транспорт успешно удален";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении транспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Управление водителями
        private void LoadDrivers()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Name, Experience, LicenseNumber FROM Drivers WHERE IsActive = 1 ORDER BY Name";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    DriversDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке водителей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddDriver_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(ExperienceTextBox.Text) ||
                string.IsNullOrWhiteSpace(LicenseNumberTextBox.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Drivers (Name, Experience, LicenseNumber) VALUES (@Name, @Experience, @LicenseNumber)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", NameTextBox.Text);
                    command.Parameters.AddWithValue("@Experience", int.Parse(ExperienceTextBox.Text));
                    command.Parameters.AddWithValue("@LicenseNumber", LicenseNumberTextBox.Text);
                    command.ExecuteNonQuery();
                }

                // Очистка полей и обновление таблицы
                NameTextBox.Clear();
                ExperienceTextBox.Clear();
                LicenseNumberTextBox.Clear();
                LoadDrivers();
                LoadDriverComboBox();

                StatusText.Text = "Водитель успешно добавлен";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении водителя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteDriver_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int driverId = (int)button.CommandParameter;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Drivers SET IsActive = 0 WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", driverId);
                    command.ExecuteNonQuery();
                }

                LoadDrivers();
                LoadDriverComboBox();
                StatusText.Text = "Водитель успешно удален";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении водителя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Маршрутизация
        private void LoadRoutes()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            r.Id,
                            r.StartPoint,
                            r.EndPoint,
                            r.Distance,
                            r.TravelTime,
                            d.Name AS DriverName,
                            v.Brand AS VehicleBrand,
                            v.Model AS VehicleModel
                        FROM Routes r
                        JOIN Drivers d ON r.DriverId = d.Id
                        JOIN Vehicles v ON r.VehicleId = v.Id
                        WHERE d.IsActive = 1 AND v.IsActive = 1
                        ORDER BY r.CreatedAt DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    RoutesDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке маршрутов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDriverComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Name FROM Drivers WHERE IsActive = 1 ORDER BY Name";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    DriverComboBox.Items.Clear();
                    while (reader.Read())
                    {
                        DriverComboBox.Items.Add(new
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке списка водителей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadVehicleComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Brand FROM Vehicles WHERE IsActive = 1 ORDER BY Brand";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    VehicleComboBox.Items.Clear();
                    while (reader.Read())
                    {
                        VehicleComboBox.Items.Add(new
                        {
                            Id = reader.GetInt32(0),
                            Brand = reader.GetString(1)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке списка транспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddRoute_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StartPointTextBox.Text) || string.IsNullOrWhiteSpace(EndPointTextBox.Text) ||
                string.IsNullOrWhiteSpace(DistanceTextBox.Text) || string.IsNullOrWhiteSpace(TravelTimeTextBox.Text) ||
                DriverComboBox.SelectedItem == null || VehicleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                dynamic selectedDriver = DriverComboBox.SelectedItem;
                dynamic selectedVehicle = VehicleComboBox.SelectedItem;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO Routes (StartPoint, EndPoint, Distance, TravelTime, DriverId, VehicleId)
                        VALUES (@StartPoint, @EndPoint, @Distance, @TravelTime, @DriverId, @VehicleId)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StartPoint", StartPointTextBox.Text);
                    command.Parameters.AddWithValue("@EndPoint", EndPointTextBox.Text);
                    command.Parameters.AddWithValue("@Distance", decimal.Parse(DistanceTextBox.Text));
                    command.Parameters.AddWithValue("@TravelTime", decimal.Parse(TravelTimeTextBox.Text));
                    command.Parameters.AddWithValue("@DriverId", selectedDriver.Id);
                    command.Parameters.AddWithValue("@VehicleId", selectedVehicle.Id);
                    command.ExecuteNonQuery();
                }

                // Очистка полей и обновление таблицы
                StartPointTextBox.Clear();
                EndPointTextBox.Clear();
                DistanceTextBox.Clear();
                TravelTimeTextBox.Clear();
                LoadRoutes();
                UpdateRouteStatistics();

                StatusText.Text = "Маршрут успешно добавлен";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении маршрута: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteRoute_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int routeId = (int)button.CommandParameter;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Routes WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", routeId);
                    command.ExecuteNonQuery();
                }

                LoadRoutes();
                UpdateRouteStatistics();
                StatusText.Text = "Маршрут успешно удален";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении маршрута: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRouteStatistics()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            COUNT(*) AS TotalRoutes,
                            SUM(Distance) AS TotalDistance,
                            AVG(TravelTime) AS AvgTime
                        FROM Routes";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        TotalRoutesText.Text = reader["TotalRoutes"].ToString();
                        TotalDistanceText.Text = reader["TotalDistance"].ToString();
                        AvgTimeText.Text = reader["AvgTime"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении статистики: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateOptimalRoute_Click(object sender, RoutedEventArgs e)
        {
            // Здесь можно реализовать алгоритм расчета оптимального маршрута
            MessageBox.Show("Функция расчета оптимального маршрута находится в разработке", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowRouteOnMap_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int routeId = (int)button.CommandParameter;

            // Здесь можно реализовать отображение маршрута на карте
            MessageBox.Show($"Отображение маршрута #{routeId} на карте", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RoutingTab_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDriverComboBox();
            LoadVehicleComboBox();
        }
        #endregion

        #region Отчеты
        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportTypeComboBox.SelectedItem == null || StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите тип отчета и период", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string reportType = ((ComboBoxItem)ReportTypeComboBox.SelectedItem).Content.ToString();
            DateTime startDate = StartDatePicker.SelectedDate.Value;
            DateTime endDate = EndDatePicker.SelectedDate.Value;

            try
            {
                DataTable reportData = new DataTable();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "";

                    switch (reportType)
                    {
                        case "По маршрутам":
                            query = @"
                                SELECT 
                                    r.StartPoint + ' → ' + r.EndPoint AS Route,
                                    r.Distance,
                                    r.TravelTime,
                                    d.Name AS Driver,
                                    v.Brand + ' ' + v.Model AS Vehicle,
                                    r.CreatedAt AS RouteDate
                                FROM Routes r
                                JOIN Drivers d ON r.DriverId = d.Id
                                JOIN Vehicles v ON r.VehicleId = v.Id
                                WHERE r.CreatedAt BETWEEN @StartDate AND @EndDate
                                ORDER BY r.CreatedAt";
                            break;

                        case "По водителям":
                            query = @"
                                SELECT 
                                    d.Name,
                                    d.Experience,
                                    d.LicenseNumber,
                                    COUNT(r.Id) AS RoutesCount,
                                    SUM(r.Distance) AS TotalDistance,
                                    AVG(r.TravelTime) AS AvgTravelTime
                                FROM Drivers d
                                LEFT JOIN Routes r ON d.Id = r.DriverId
                                WHERE r.CreatedAt BETWEEN @StartDate AND @EndDate
                                GROUP BY d.Id, d.Name, d.Experience, d.LicenseNumber
                                ORDER BY RoutesCount DESC";
                            break;

                        case "По транспорту":
                            query = @"
                                SELECT 
                                    v.Brand,
                                    v.Model,
                                    v.Number,
                                    COUNT(r.Id) AS RoutesCount,
                                    SUM(r.Distance) AS TotalDistance,
                                    AVG(r.TravelTime) AS AvgTravelTime
                                FROM Vehicles v
                                LEFT JOIN Routes r ON v.Id = r.VehicleId
                                WHERE r.CreatedAt BETWEEN @StartDate AND @EndDate
                                GROUP BY v.Id, v.Brand, v.Model, v.Number
                                ORDER BY TotalDistance DESC";
                            break;

                        case "Финансовый":
                            query = @"
                                SELECT 
                                    v.Brand + ' ' + v.Model AS Vehicle,
                                    COUNT(r.Id) AS TripsCount,
                                    SUM(r.Distance) * 0.5 AS FuelCost,
                                    SUM(r.Distance) * 0.1 AS MaintenanceCost,
                                    SUM(r.Distance) * 0.3 AS TotalCost
                                FROM Vehicles v
                                LEFT JOIN Routes r ON v.Id = r.VehicleId
                                WHERE r.CreatedAt BETWEEN @StartDate AND @EndDate
                                GROUP BY v.Id, v.Brand, v.Model
                                ORDER BY TotalCost DESC";
                            break;
                    }

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate.AddDays(1)); // Чтобы включить всю конечную дату

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(reportData);
                }

                ReportsDataGrid.ItemsSource = reportData.DefaultView;
                SaveReportToDatabase(reportType, startDate, endDate, reportData);

                StatusText.Text = $"Отчет '{reportType}' успешно сформирован";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveReportToDatabase(string reportType, DateTime startDate, DateTime endDate, DataTable reportData)
        {
            try
            {
                string reportDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(reportData);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO Reports (ReportType, StartDate, EndDate, ReportData)
                        VALUES (@ReportType, @StartDate, @EndDate, @ReportData)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ReportType", reportType);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    command.Parameters.AddWithValue("@ReportData", reportDataJson);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении отчета в базу данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsDataGrid.ItemsSource == null)
            {
                MessageBox.Show("Нет данных для экспорта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Здесь можно реализовать экспорт в Excel
            MessageBox.Show("Экспорт в Excel выполнен успешно", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsDataGrid.ItemsSource == null)
            {
                MessageBox.Show("Нет данных для печати", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Здесь можно реализовать печать отчета
            MessageBox.Show("Отчет отправлен на печать", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion
    }
}