using cp.commands;
using cp.models;
using cp.services;
using cp.views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace cp.viewModels
{
    public class CatalogViewModel : BaseViewModel
    {
        private Flower _selectedFlower;
        public Flower SelectedFlower
        {
            get => _selectedFlower;
            set
            {
                _selectedFlower = value;
                OnPropertyChanged();
                OpenDetailsCommand?.Execute(value);
            }
        }

        public ObservableCollection<Flower> Flowers { get; set; }

        public ICommand OpenDetailsCommand { get; }

        public bool IsAdmin => AuthService.CurrentUser?.Role == "ADMIN";
        public bool IsUser => AuthService.CurrentUser?.Role == "USER";

        public ICommand AddProductCommand { get; }

        public ICommand EditProductCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand DeleteProductCommand { get; }

        public CatalogViewModel()
        {
            using var db = new FlowerShopDbContext();
            _allFlowers = new ObservableCollection<Flower>(db.Flowers.ToList());
            FilteredFlowers = new ObservableCollection<Flower>(_allFlowers);

            OpenDetailsCommand = new RelayCommand<Flower>(OpenDetails);
            AddProductCommand = new RelayCommand(OpenAddProductPage);
            EditProductCommand = new RelayCommand<Flower>(EditProduct);
            DeleteProductCommand = new RelayCommand<Flower>(DeleteProduct);

            ResetFiltersCommand = new RelayCommand(ResetFilters);

            AddToCartCommand = new RelayCommand<Flower>(AddToCart);
        }

        private void OpenDetails(Flower flower)
        {
            var page = new FlowerDetailsPage(flower);
            (App.Current.MainWindow.DataContext as MainViewModel).CurrentPage = page;
        }

        private void OpenAddProductPage()
        {
            var page = new AddProductPage();
            (App.Current.MainWindow.DataContext as MainViewModel).CurrentPage = page;
        }

        private void EditProduct(Flower flower)
        {
            var page = new EditProductPage(flower);
            (App.Current.MainWindow.DataContext as MainViewModel).CurrentPage = page;
        }

        private void DeleteProduct(Flower flower)
        {
            if (flower == null) return;
            var textTmpl = (string)Application.Current.FindResource("Msg_DeleteConfirm_Text");
            var caption = (string)Application.Current.FindResource("Msg_DeleteConfirm_Caption");
            var text = string.Format(textTmpl, flower.Name);

            var result = MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;


            using (var db = new FlowerShopDbContext())
            {
                var flowerToDelete = db.Flowers.FirstOrDefault(f => f.Id == flower.Id);
                if (flowerToDelete != null)
                {
                    db.Flowers.Remove(flowerToDelete);
                    db.SaveChanges();
                }
            }

            _allFlowers.Remove(flower);
            FilteredFlowers.Remove(flower);
        }
        private ObservableCollection<Flower> _allFlowers;

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ObservableCollection<Flower> FilteredFlowers { get; set; } = new();

        public List<string> SortOptions { get; } = new() { "По названию", "По цене (возр.)", "По цене (убыв.)" };

        private string _selectedSortOption;
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ICommand ResetFiltersCommand { get; }

        private void ApplyFilters()
        {
            var filtered = _allFlowers.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(f => f.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            filtered = SelectedSortOption switch
            {
                "По названию" => filtered.OrderBy(f => f.Name),
                "По цене (возр.)" => filtered.OrderBy(f => f.Price),
                "По цене (убыв.)" => filtered.OrderByDescending(f => f.Price),
                _ => filtered
            };

            FilteredFlowers.Clear();
            foreach (var flower in filtered)
                FilteredFlowers.Add(flower);
        }

        private void ResetFilters()
        {
            SearchText = string.Empty;
            SelectedSortOption = null;
            ApplyFilters();
        }
        private void AddToCart(Flower flower)
        {
            var currentUser = AuthService.CurrentUser;


            using (var db = new FlowerShopDbContext())
            {
                var cart = db.Carts
                    .Include(c => c.Items)
                    .FirstOrDefault(c => c.UserId == currentUser.Id);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = currentUser.Id,
                        Items = new List<CartItem>()
                    };
                    db.Carts.Add(cart);
                }

                var existingItem = cart.Items.FirstOrDefault(i => i.FlowerId == flower.Id);

                if (existingItem != null)
                {
                    existingItem.Quantity += 1;
                }
                else
                {
                    cart.Items.Add(new CartItem
                    {
                        FlowerId = flower.Id,
                        Quantity = 1
                    });
                }

                db.SaveChanges();
            }

            var textTmpl = (string)Application.Current.FindResource("Msg_AddedToCart_Text");
            var text = string.Format(textTmpl, flower.Name);
            MessageBox.Show(text);
        }



    }
}
