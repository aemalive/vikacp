using cp.commands;
using cp.models;
using cp.services;
using cp.views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace cp.viewModels
{
    public class OrderViewModel : BaseViewModel
    {
        public ObservableCollection<OrderItem> OrderItems { get; set; } = new();

        private string _address;
        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        private string _deliveryTime;
        public string DeliveryTime
        {
            get => _deliveryTime;
            set { _deliveryTime = value; OnPropertyChanged(); }
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(); }
        }

        public Order Order { get; set; }

        public decimal TotalAmount => OrderItems.Sum(i => i.Price * i.Quantity);

        public ICommand ConfirmOrderCommand { get; }
        public ICommand CancelOrderCommand { get; }

        public OrderViewModel()
        {
            ConfirmOrderCommand = new RelayCommand(OnConfirmOrder);
            CancelOrderCommand = new RelayCommand(OnCancelOrder);
            LoadPreferredAddress(); 
            LoadOrder();
        }

        private void LoadPreferredAddress()
        {
            using var db = new FlowerShopDbContext();
            var user = db.Users
                         .Where(u => u.Id == AuthService.CurrentUser.Id)
                         .Select(u => new { u.PreferredAddress })
                         .FirstOrDefault();

            if (user != null && !string.IsNullOrWhiteSpace(user.PreferredAddress))
            {
                Address = user.PreferredAddress;
            }
        }

        private void LoadOrder()
        {
            var cart = GetCartFromDb();

            Order = new Order
            {
                Address = Address,
                DeliveryTime = DeliveryTime,
                Comment = Comment,
                OrderDate = DateTime.UtcNow,
                OrderItems = cart.Items.Select(i => new OrderItem
                {
                    Flower = i.Flower,
                    Quantity = i.Quantity,
                    Price = i.Flower?.Price ?? 0
                }).ToList()
            };

            foreach (var item in Order.OrderItems)
            {
                OrderItems.Add(item);
            }

            OnPropertyChanged(nameof(TotalAmount));
        }

        private Cart GetCartFromDb()
        {
            using var db = new FlowerShopDbContext();
            return db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Flower)
                .FirstOrDefault(c => c.UserId == AuthService.CurrentUser.Id);
        }

        private void OnConfirmOrder()
        {
            if (string.IsNullOrEmpty(DeliveryTime))
            {
                MessageBox.Show(
                    (string)Application.Current.Resources["Order_EnterDeliveryTime"]);
                return;
            }
            if (string.IsNullOrEmpty(Address))
            {
                MessageBox.Show(
                    (string)Application.Current.Resources["Order_EnterAddress"]);
                return;
            }

            var result = MessageBox.Show(
                           (string)Application.Current.Resources["Order_ConfirmMessage"],
                           (string)Application.Current.Resources["Order_ConfirmTitle"],
                           MessageBoxButton.YesNo,
                           MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            var newOrder = new Order
            {
                Address = Address,
                DeliveryTime = DeliveryTime,
                Comment = Comment,
                TotalAmount = TotalAmount,
                UserId = AuthService.CurrentUser.Id,
                OrderDate = DateTime.UtcNow,
                OrderItems = OrderItems.Select(i => new OrderItem
                {
                    FlowerId = i.Flower.Id,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            using (var db = new FlowerShopDbContext())
            {
                db.Orders.Add(newOrder);
                db.SaveChanges();
            }

            ClearCart();
        }

        private void ClearCart()
        {
            using (var db = new FlowerShopDbContext())
            {
                var cart = db.Carts
                    .Include(c => c.Items)
                    .FirstOrDefault(c => c.UserId == AuthService.CurrentUser.Id);

                if (cart != null && cart.Items != null)
                {
                    db.CartItems.RemoveRange(cart.Items);
                    db.SaveChanges();
                }
            }

            var mainVm = (Application.Current.MainWindow.DataContext as MainViewModel);
            mainVm.CurrentPage = new CartPage();
        }
        private void OnCancelOrder()
        {
            var result = MessageBox.Show(
                (string)Application.Current.Resources["Order_CancelConfirmMessage"],
                (string)Application.Current.Resources["Order_CancelConfirmTitle"],
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Address = string.Empty;
                DeliveryTime = string.Empty;
                Comment = string.Empty;
                OrderItems.Clear();

                var mainVm = (Application.Current.MainWindow.DataContext as MainViewModel);
                mainVm.CurrentPage = new CartPage();
            }
        }

    }

}
