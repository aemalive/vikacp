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

namespace cp.viewModels
{
    public class CartViewModel : BaseViewModel
    {
        public ObservableCollection<CartItem> CartItems { get; set; } = new();

        private string _orderComment;
        public string OrderComment
        {
            get => _orderComment;
            set { _orderComment = value; OnPropertyChanged(); }
        }

        public decimal TotalPrice => CartItems.Sum(i => i.Flower.Price * i.Quantity);

        public ICommand OnGoOrderCommand { get; }
        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand RemoveItemCommand { get; }

        public CartViewModel()
        {
            OnGoOrderCommand = new RelayCommand(OnGoOrder);
            IncreaseQuantityCommand = new RelayCommand<CartItem>(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand<CartItem>(DecreaseQuantity);
            RemoveItemCommand = new RelayCommand<CartItem>(RemoveItem);
            LoadCart();
        }

        private void LoadCart()
        {

            using var db = new FlowerShopDbContext();

            var cart = db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Flower)
                .FirstOrDefault(c => c.UserId == AuthService.CurrentUser.Id);

            CartItems.Clear();

            if (cart != null)
            {
                foreach (var item in cart.Items)
                    CartItems.Add(item);
            }

            OnPropertyChanged(nameof(TotalPrice));
        }


        private void OnGoOrder()
        {
            var mainVm = (Application.Current.MainWindow.DataContext as MainViewModel);
            mainVm.CurrentPage = new OrderPage();
        }
        private void IncreaseQuantity(CartItem item)
        {
            if (item == null) return;

            item.Quantity++;
            UpdateItemInDb(item);
            OnPropertyChanged();
        }

        private void DecreaseQuantity(CartItem item)
        {
            if (item == null || item.Quantity <= 1) return;

            item.Quantity--;
            UpdateItemInDb(item);
            OnPropertyChanged();
        }

        private void RemoveItem(CartItem item)
        {
            if (item == null) return;

            var result = MessageBox.Show(
                $"Удалить товар \"{item.Flower?.Name}\" из корзины?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            using var db = new FlowerShopDbContext();
            var dbItem = db.CartItems.FirstOrDefault(i => i.Id == item.Id);
            if (dbItem != null)
            {
                db.CartItems.Remove(dbItem);
                db.SaveChanges();
            }

            CartItems.Remove(item);
            OnPropertyChanged(nameof(TotalPrice));
        }


        private void UpdateItemInDb(CartItem item)
        {
            using var db = new FlowerShopDbContext();
            var dbItem = db.CartItems.FirstOrDefault(i => i.Id == item.Id);
            if (dbItem != null)
            {
                dbItem.Quantity = item.Quantity;
                db.SaveChanges();
            }

            OnPropertyChanged(nameof(TotalPrice));
        }

    }
}
