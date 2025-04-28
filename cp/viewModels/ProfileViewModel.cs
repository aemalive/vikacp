using cp.commands;
using cp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace cp.viewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private string _username;
        private string _fullName;
        private string _email;
        private string _password;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand PasswordChangedCommand { get; }

        public ProfileViewModel()
        {
            SaveCommand = new RelayCommand(SaveProfile);
            PasswordChangedCommand = new RelayCommand<string>(UpdatePassword);
        }

        private void SaveProfile()
        {
            // Создаём объект User с данными из формы
            User user = new User
            {
                Username = Username,
                FullName = FullName,
                Email = Email,
                Password = Password
            };

            // TODO: Логика сохранения данных

            // Для теста выводим данные в консоль
            Console.WriteLine($"Сохранён пользователь: {user.Username}, {user.FullName}, {user.Email}");

            // Очистить поля после сохранения (опционально)
            Username = string.Empty;
            FullName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
        }

        private void UpdatePassword(string password)
        {
            Password = password;
        }
    }
}
