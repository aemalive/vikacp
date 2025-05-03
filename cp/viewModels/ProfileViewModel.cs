using cp.commands;
using cp.models;
using cp.services;
using cp.views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace cp.viewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public string Username => AuthService.CurrentUser.Username;
        public string FullName => AuthService.CurrentUser.FullName;
        public string Role => AuthService.CurrentUser.Role;

        public bool IsAdmin => Role == "ADMIN";
        public bool IsCustomer => Role == "USER";

        public ICommand LogoutCommand { get; }

        private readonly Action<string> _navigate;

        public ProfileViewModel(Action<string> navigate)
        {
            _navigate = navigate;
            LogoutCommand = new RelayCommand(Logout);

            using var db = new FlowerShopDbContext();
            Users = new ObservableCollection<User>(db.Users.ToList());

            EditUserCommand = new RelayCommand<User>(EditUser);

        }

        private void Logout()
        {
            AuthService.Logout();
            _navigate?.Invoke("LoginPage");
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                EditUserCommand?.Execute(value);
            }
        }

        public ObservableCollection<User> Users { get; }

        public ICommand EditUserCommand { get; }
        private void EditUser(User user)
        {
            var page = new EditUserPage(user); 
            (App.Current.MainWindow.DataContext as MainViewModel).CurrentPage = page;
        }

    }
}
