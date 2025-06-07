using System;
using System.Data.SqlClient;
using System.Windows;

namespace ATP
{
    public partial class LoginWindow : Window
    {
        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public string Role { get; set; }
            public int Id { get; set; }
            public int? DriverId { get; set; } // Добавлено для связи с водителем
        }

        public LoginWindow()
        {
            InitializeComponent();
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
                User foundUser = AuthenticateUser(username, password);

                if (foundUser != null)
                {
                    OpenAppropriateWindow(foundUser);
                }
                else
                {
                    StatusText.Text = "Неверный логин или пароль";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private User AuthenticateUser(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(App.ConnectionString))
            {
                connection.Open();
                string query = @"SELECT u.Id, u.Username, u.Name, u.Role, u.DriverId 
                               FROM Users u
                               WHERE u.Username = @Username AND u.Password = @Password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password); // В реальном приложении используйте хеширование

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Name = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                Role = reader.GetString(3),
                                DriverId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4)
                            };
                        }
                    }
                }
            }
            return null;
        }

        private void OpenAppropriateWindow(User user)
        {
            try
            {
                Window nextWindow;

                switch (user.Role.ToLower())
                {
                    case "admin":
                        nextWindow = new MainWindow();
                        break;
                    case "driver":
                        if (user.DriverId.HasValue)
                        {
                            nextWindow = new DriverWindow(user.DriverId.Value);
                        }
                        else
                        {
                            MessageBox.Show("Для водителя не указан DriverId", "Ошибка",
                                          MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль пользователя", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                nextWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}