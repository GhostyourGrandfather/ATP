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
        }

        private void LoadVehicles()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(App.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Brand, Model, Year, Number FROM Vehicles";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        DataTable dt = new DataTable();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dt);
                        VehiclesDataGrid.ItemsSource = dt.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки транспорта: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDrivers()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(App.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Name, Experience, LicenseNumber FROM Drivers";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        DataTable dt = new DataTable();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dt);
                        DriversDataGrid.ItemsSource = dt.DefaultView;
                        DriverComboBox.ItemsSource = dt.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки водителей: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRoutes()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(App.ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT r.Id, r.StartPoint, r.EndPoint, r.Distance, r.TravelTime, 
                                   r.Status, d.Name AS DriverName, v.Brand AS VehicleBrand
                                   FROM Routes r
                                   JOIN Drivers d ON r.DriverId = d.Id
                                   JOIN Vehicles v ON r.VehicleId = v.Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        DataTable dt = new DataTable();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dt);
                        RoutesDataGrid.ItemsSource = dt.DefaultView;
                    }
                }
                UpdateRouteStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки маршрутов: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddVehicle_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BrandTextBox.Text)) return;

            try
            {
                using (SqlConnection connection = new SqlConnection(App.ConnectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Vehicles (Brand, Model, Year, Number) 
                                   VALUES (@Brand, @Model, @Year, @Number);
                                   SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Brand", BrandTextBox.Text);
                        command.Parameters.AddWithValue("@Model", ModelTextBox.Text);
                        command.Parameters.AddWithValue("@Year", int.TryParse(YearTextBox.Text, out int year) ? year : 0);
                        command.Parameters.AddWithValue("@Number", NumberTextBox.Text);

                        int newId = Convert.ToInt32(command.ExecuteScalar());

                        // Очистка полей
                        BrandTextBox.Clear();
                        ModelTextBox.Clear();
                        YearTextBox.Clear();
                        NumberTextBox.Clear();

                        LoadVehicles();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении транспорта: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddDriver_Click(object sender, RoutedEventArgs e)
        {

        }

        // Аналогично модифицировать остальные методы для работы с БД
        // (DeleteVehicle, AddDriver, DeleteDriver, AddRoute и т.д.)
    }
}