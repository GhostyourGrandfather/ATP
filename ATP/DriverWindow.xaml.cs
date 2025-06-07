using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace ATP
{
    public partial class DriverWindow : Window
    {
        private int _driverId;
        private DataTable _routesTable;

        public DriverWindow(int driverId)
        {
            InitializeComponent();
            _driverId = driverId;
            LoadDriverData();
        }

        private void LoadDriverData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(App.ConnectionString))
                {
                    connection.Open();

                    // Загрузка информации о водителе
                    string driverQuery = "SELECT Name FROM Drivers WHERE Id = @DriverId";
                    using (SqlCommand driverCommand = new SqlCommand(driverQuery, connection))
                    {
                        driverCommand.Parameters.AddWithValue("@DriverId", _driverId);
                        string driverName = driverCommand.ExecuteScalar()?.ToString();
                        DriverNameText.Text = driverName;
                        this.Title = $"АТП - Водитель: {driverName}";
                    }

                    // Загрузка маршрутов водителя
                    string routesQuery = @"SELECT Id, StartPoint, EndPoint, Distance, Status 
                                         FROM Routes 
                                         WHERE DriverId = @DriverId";
                    _routesTable = new DataTable();
                    using (SqlCommand routesCommand = new SqlCommand(routesQuery, connection))
                    {
                        routesCommand.Parameters.AddWithValue("@DriverId", _driverId);
                        SqlDataAdapter adapter = new SqlDataAdapter(routesCommand);
                        adapter.Fill(_routesTable);
                        CurrentRoutesDataGrid.ItemsSource = _routesTable.DefaultView;
                    }

                    // Загрузка статистики
                    string statsQuery = @"SELECT COUNT(*) AS TotalRoutes, SUM(Distance) AS TotalDistance 
                                        FROM Routes 
                                        WHERE DriverId = @DriverId";
                    using (SqlCommand statsCommand = new SqlCommand(statsQuery, connection))
                    {
                        statsCommand.Parameters.AddWithValue("@DriverId", _driverId);
                        using (SqlDataReader reader = statsCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TotalRoutesText.Text = reader["TotalRoutes"].ToString();
                                TotalDistanceText.Text = reader["TotalDistance"].ToString();
                            }
                        }
                    }

                    // Загрузка информации о транспорте
                    LoadVehicleInfo(connection);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadVehicleInfo(SqlConnection connection)
        {
            string vehicleQuery = @"SELECT v.Brand, v.Model, v.Year, v.Number 
                                   FROM Vehicles v
                                   JOIN Drivers d ON v.Id = d.VehicleId
                                   WHERE d.Id = @DriverId";

            using (SqlCommand vehicleCommand = new SqlCommand(vehicleQuery, connection))
            {
                vehicleCommand.Parameters.AddWithValue("@DriverId", _driverId);
                using (SqlDataReader reader = vehicleCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        VehicleBrandText.Text = reader["Brand"].ToString();
                        VehicleModelText.Text = reader["Model"].ToString();
                        VehicleYearText.Text = reader["Year"].ToString();
                        VehicleNumberText.Text = reader["Number"].ToString();
                    }
                }
            }

            string statusQuery = @"SELECT Component, Condition, CheckDate, Recommendation 
                                 FROM VehicleStatus 
                                 WHERE VehicleId IN (SELECT VehicleId FROM Drivers WHERE Id = @DriverId)";
            DataTable statusTable = new DataTable();
            using (SqlCommand statusCommand = new SqlCommand(statusQuery, connection))
            {
                statusCommand.Parameters.AddWithValue("@DriverId", _driverId);
                SqlDataAdapter adapter = new SqlDataAdapter(statusCommand);
                adapter.Fill(statusTable);
                VehicleStatusDataGrid.ItemsSource = statusTable.DefaultView;
            }
        }

        private void StartRoute_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentRoutesDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int routeId = Convert.ToInt32(selectedRow["Id"]);
                UpdateRouteStatus(routeId, "В пути");
            }
        }

        private void CompleteRoute_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentRoutesDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int routeId = Convert.ToInt32(selectedRow["Id"]);
                UpdateRouteStatus(routeId, "Завершен");
            }
        }

        private void UpdateRouteStatus(int routeId, string newStatus)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(App.ConnectionString))
                {
                    connection.Open();
                    string query = "UPDATE Routes SET Status = @Status WHERE Id = @RouteId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Status", newStatus);
                        command.Parameters.AddWithValue("@RouteId", routeId);
                        command.ExecuteNonQuery();
                    }
                }
                LoadDriverData(); // Обновляем данные
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления статуса маршрута: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Остальные методы остаются без изменений
    }
}