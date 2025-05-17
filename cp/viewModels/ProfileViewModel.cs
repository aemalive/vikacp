using cp.commands;
using cp.models;
using cp.services;
using cp.views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace cp.viewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public string Username => AuthService.CurrentUser.Username;
        public string FullName => AuthService.CurrentUser.FullName;
        public string Role => AuthService.CurrentUser.Role;

        private Order _selectedOrder;
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
                OpenDetailsCommand?.Execute(value);
            }
        }
        public bool IsAdmin => Role == "ADMIN";
        public bool IsCustomer => Role == "USER";

        public ICommand LogoutCommand { get; }
        public ICommand OpenDetailsCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand EditOwnProfileCommand { get; }


        private readonly Action<string> _navigate;

        public ObservableCollection<Order> OrderHistory { get; }

        public ProfileViewModel(Action<string> navigate)
        {
            _navigate = navigate;
            LogoutCommand = new RelayCommand(Logout);

            using var db = new FlowerShopDbContext();

            Users = new ObservableCollection<User>(db.Users.ToList());

            EditUserCommand = new RelayCommand<User>(EditUser);
            DeleteUserCommand = new RelayCommand<User>(DeleteUser);

            EditOwnProfileCommand = new RelayCommand(EditOwnProfile);

            if (IsCustomer)
            {
                OrderHistory = new ObservableCollection<Order>(
                    db.Orders
                      .Include(o => o.OrderItems)
                          .ThenInclude(oi => oi.Flower)
                      .Where(o => o.UserId == AuthService.CurrentUser.Id)
                      .OrderByDescending(o => o.OrderDate)
                      .ToList());

            }
            OpenDetailsCommand = new RelayCommand<Order>(OpenDetails);
        }
        private void OpenDetails(Order order)
        {
            var page = new OrderDetailsPage(order);
            (App.Current.MainWindow.DataContext as MainViewModel).CurrentPage = page;
        }
        private void EditOwnProfile()
        {
            var page = new EditUserPage(AuthService.CurrentUser); 
            (App.Current.MainWindow.DataContext as MainViewModel).CurrentPage = page;
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

        private void DeleteUser(User user)
        {
            if (user == null || user.Id == AuthService.CurrentUser.Id)
                return;

            var result = System.Windows.MessageBox.Show(
                $"Вы уверены, что хотите удалить пользователя \"{user.Username}\"?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using var db = new FlowerShopDbContext();
                var userToDelete = db.Users.FirstOrDefault(u => u.Id == user.Id);

                if (userToDelete != null)
                {
                    db.Users.Remove(userToDelete);
                    db.SaveChanges();
                    Users.Remove(user); 
                }
            }
        }


    }
}
