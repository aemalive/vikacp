using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cp.commands;
using cp.models;
using System.Windows.Input;
using System.Windows;
using cp.services;

namespace cp.viewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(OnLogin, CanLogin);
            GoToRegisterCommand = new RelayCommand(OnGoToRegister);
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private void OnLogin()
        {
            using var db = new FlowerShopDbContext();
            var user = db.Users
                         .FirstOrDefault(u => u.Username == Username && u.Password == Password);
            if (user != null)
            {
                AuthService.CurrentUser = user;
                (Application.Current.MainWindow.DataContext as MainViewModel)
                    .CurrentPage = "CatalogPage";
            }
            else
                MessageBox.Show("Неверный логин или пароль");
        }



        private void OnGoToRegister()
        {
            (Application.Current.MainWindow.DataContext as MainViewModel).CurrentPage = "RegisterPage";
        }
    }
}
