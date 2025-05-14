using System;
using System.Windows;
using System.Windows.Input;
using cp.commands;
using cp.models;

namespace cp.viewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _username;
        private string _password;
        private string _fullName;
        private string _email;

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

        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(RegisterUser);
        }

        private void RegisterUser()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(Email))
            {
                MessageBox.Show("Все поля должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                using (var db = new FlowerShopDbContext())
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                }

                MessageBox.Show($"Пользователь {user.Username} успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                Username = string.Empty;
                Password = string.Empty;
                FullName = string.Empty;
                Email = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
