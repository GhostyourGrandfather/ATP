using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace ATP
{
    public partial class DriverWindow : Window
    {
        private string connectionString = "Data Source=(local);Initial Catalog=ATP_Management;Integrated Security=True";
        private int currentDriverId;

        public DriverWindow(int driverId)
        {
            InitializeComponent();
            currentDriverId = driverId;
            LoadDriverData();
            LoadRoutes();
            LoadVehicleInfo();
        }

        private void DriverWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadDriverData();
                LoadRoutes();
                LoadVehicleInfo();
                StatusText.Text = "Данные загружены";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка загрузки данных";
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDriverStatistics()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                SELECT 
                    COUNT(*) AS TotalCompletedRoutes,
                    SUM(Distance) AS TotalDistance,
                    AVG(Rating) AS AvgRating
                FROM Routes
                WHERE DriverId = @DriverId AND IsCompleted = 1";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DriverId", currentDriverId);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        TotalRoutesText.Text = reader["TotalCompletedRoutes"].ToString();
                        TotalDistanceText.Text = reader["TotalDistance"].ToString();
                        AvgRatingText.Text = reader["AvgRating"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статистики: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRecentRoutes()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                SELECT TOP 10
                    r.StartPoint + ' → ' + r.EndPoint AS Route,
                    r.Distance,
                    FORMAT(r.CreatedAt, 'dd.MM.yyyy') AS RouteDate,
                    v.Brand + ' ' + v.Model AS Vehicle,
                    CASE WHEN r.IsCompleted = 1 THEN 'Завершен' ELSE 'В процессе' END AS Status
                FROM Routes r
                JOIN Vehicles v ON r.VehicleId = v.Id
                WHERE r.DriverId = @DriverId
                ORDER BY r.CreatedAt DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DriverId", currentDriverId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    RouteHistoryDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке истории маршрутов: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDriverData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Name FROM Drivers WHERE Id = @DriverId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DriverId", currentDriverId);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        DriverNameText.Text = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных водителя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRoutes()
        {
            try
            {
                // Загрузка текущих маршрутов
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            r.Id,
                            r.StartPoint,
                            r.EndPoint,
                            r.Distance,
                            v.Brand + ' ' + v.Model AS Vehicle,
                            r.CreatedAt AS RouteDate,
                            CASE 
                                WHEN r.IsCompleted = 1 THEN 'Завершен'
                                ELSE 'В процессе'
                            END AS Status,
                            CASE 
                                WHEN r.IsCompleted = 1 THEN 'Green'
                                ELSE 'Orange'
                            END AS StatusColor
                        FROM Routes r
                        JOIN Vehicles v ON r.VehicleId = v.Id
                        WHERE r.DriverId = @DriverId
                        ORDER BY r.CreatedAt DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DriverId", currentDriverId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    CurrentRoutesDataGrid.ItemsSource = dt.DefaultView;
                }

                // Загрузка статистики
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            COUNT(*) AS TotalRoutes,
                            SUM(Distance) AS TotalDistance
                        FROM Routes
                        WHERE DriverId = @DriverId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DriverId", currentDriverId);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        TotalRoutesText.Text = reader["TotalRoutes"].ToString();
                        TotalDistanceText.Text = reader["TotalDistance"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке маршрутов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadVehicleInfo()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Основная информация о транспорте
                    string query = @"
                        SELECT 
                            v.Brand,
                            v.Model,
                            v.Year,
                            v.Number
                        FROM Vehicles v
                        JOIN Drivers d ON v.Id = d.VehicleId
                        WHERE d.Id = @DriverId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DriverId", currentDriverId);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        VehicleBrandText.Text = reader["Brand"].ToString();
                        VehicleModelText.Text = reader["Model"].ToString();
                        VehicleYearText.Text = reader["Year"].ToString();
                        VehicleNumberText.Text = reader["Number"].ToString();
                    }
                    reader.Close();

                    // Техническое состояние
                    query = @"
                        SELECT 
                            Component,
                            Condition,
                            CheckDate,
                            Recommendation
                        FROM VehicleChecks
                        WHERE VehicleId = (SELECT VehicleId FROM Drivers WHERE Id = @DriverId)
                        ORDER BY CheckDate DESC";

                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DriverId", currentDriverId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    VehicleStatusDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке информации о транспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartRoute_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int routeId = (int)button.CommandParameter;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Routes SET IsStarted = 1 WHERE Id = @RouteId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RouteId", routeId);
                    command.ExecuteNonQuery();
                }

                LoadRoutes();
                StatusText.Text = "Маршрут начат";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при начале маршрута: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CompleteRoute_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int routeId = (int)button.CommandParameter;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Routes SET IsCompleted = 1, CompletedDate = GETDATE() WHERE Id = @RouteId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RouteId", routeId);
                    command.ExecuteNonQuery();
                }

                LoadRoutes();
                StatusText.Text = "Маршрут завершен";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при завершении маршрута: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReportProblem_Click(object sender, RoutedEventArgs e)
        {
            // Здесь можно реализовать форму для отправки сообщения о проблеме
            MessageBox.Show("Форма сообщения о проблеме", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Дополнительная загрузка данных при необходимости
        }
    }
}