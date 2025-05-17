using cp.commands;
using cp.models;
using cp.views;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace cp.viewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _username;
        private string _password;
        private string _fullName;
        private string _email;
        private string _confirmPassword;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }
        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(RegisterUser);
        }

        private void RegisterUser()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
        string.IsNullOrWhiteSpace(Password) ||
        string.IsNullOrWhiteSpace(ConfirmPassword) ||
        string.IsNullOrWhiteSpace(FullName) ||
        string.IsNullOrWhiteSpace(Email))
            {
                MessageBox.Show("Все поля должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Password.Length < 8 || Password.Length > 30)
            {
                MessageBox.Show("Пароль должен быть от 8 до 30 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Неверный формат email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new FlowerShopDbContext())
            {
                if (db.Users.Any(u => u.Username == Username))
                {
                    MessageBox.Show("Имя пользователя уже занято.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var user = new User
                {
                    Username = Username,
                    Password = Password,
                    FullName = FullName,
                    Email = Email,
                    Role = "USER"
                };

                try
                {
                    db.Users.Add(user);
                    db.SaveChanges();

                    MessageBox.Show($"Пользователь {user.Username} успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    var page = new LoginPage();
                    (App.Current.MainWindow.DataContext as MainViewModel).CurrentPage = page;

                    Username = Password = ConfirmPassword = FullName = Email = string.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
