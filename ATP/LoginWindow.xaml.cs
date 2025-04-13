using System;
using System.Data.SqlClient;
using System.Windows;
using ATP.Properties;

namespace ATP
{
    public partial class LoginWindow : Window
    {
        private const string ConnectionString = "Data Source=(local);Initial Catalog=ATP_Management;Integrated Security=True";

        public LoginWindow()
        {
            InitializeComponent();
            LoadSavedCredentials();
        }

        private void LoadSavedCredentials()
        {
            // Загрузка сохраненных учетных данных (если есть)
            UsernameTextBox.Text = Properties.Settings.Default.SavedUsername ?? "";
            RememberCheckBox.IsChecked = Properties.Settings.Default.RememberMe;
        }

        private void SaveCredentials()
        {
            if (RememberCheckBox.IsChecked == true)
            {
                Properties.Settings.Default.SavedUsername = UsernameTextBox.Text;
                Properties.Settings.Default.RememberMe = true;
            }
            else
            {
                Properties.Settings.Default.SavedUsername = "";
                Properties.Settings.Default.RememberMe = false;
            }
            Properties.Settings.Default.Save();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                StatusText.Text = "Введите логин и пароль";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT Id, Name, Role 
                        FROM Users 
                        WHERE Username = @Username AND Password = @Password";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            string role = reader.GetString(2);

                            SaveCredentials();

                            OpenAppropriateWindow(userId, name, role);
                        }
                        else
                        {
                            StatusText.Text = "Неверный логин или пароль";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка подключения к базе данных";
                MessageBox.Show($"Ошибка авторизации: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenAppropriateWindow(int userId, string name, string role)
        {
            switch (role.ToLower())
            {
                case "admin":
                    MainWindow adminWindow = new MainWindow();
                    adminWindow.Show();
                    break;
                case "driver":
                    DriverWindow driverWindow = new DriverWindow(userId);
                    driverWindow.Show();
                    break;
                default:
                    MessageBox.Show("Неизвестная роль пользователя", "Ошибка", 
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            this.Close();
        }
    }
}