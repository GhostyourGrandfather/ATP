﻿using System;
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
        }

        private readonly User[] _users = new[]
        {
            new User { Id = 1, Username = "a", Password = "a", Name = "Администратор", Role = "admin" },
            new User { Id = 2, Username = "driver1", Password = "driver123", Name = "Иванов И.И.", Role = "driver" },
            new User { Id = 3, Username = "driver2", Password = "driver456", Name = "Петров П.П.", Role = "driver" }
        };

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

            User foundUser = null;
            foreach (var user in _users)
            {
                if (user.Username == username && user.Password == password)
                {
                    foundUser = user;
                    break;
                }
            }

            if (foundUser != null)
            {
                OpenAppropriateWindow(foundUser);
            }
            else
            {
                StatusText.Text = "Неверный логин или пароль";
            }
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
                        nextWindow = new DriverWindow(user.Id);
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