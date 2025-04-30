using cp.commands;
using cp.models;
using cp.services;
using cp.views;
using System;
using System.Collections.Generic;
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

        public bool IsAdmin => Role == "Admin";
        public bool IsCustomer => Role == "Customer";

        public ICommand LogoutCommand { get; }

        private readonly Action<string> _navigate;

        public ProfileViewModel(Action<string> navigate)
        {
            _navigate = navigate;
            LogoutCommand = new RelayCommand(Logout);
        }

        private void Logout()
        {
            AuthService.Logout();
            _navigate?.Invoke("LoginPage");
        }
    }
}
