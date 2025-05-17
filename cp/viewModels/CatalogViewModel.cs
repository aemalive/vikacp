using cp.commands;
using cp.models;
using cp.services;
using cp.views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace cp.viewModels
{
    public class SortOption : INotifyPropertyChanged
    {
        public string Key { get; set; }

        public string DisplayName => (string)Application.Current.FindResource(Key);

        public void RefreshDisplayName()
        {
            OnPropertyChanged(nameof(DisplayName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }


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
            SortOptions = new ObservableCollection<SortOption>
    {
        new SortOption { Key = "Sort_ByName" },
        new SortOption { Key = "Sort_ByPriceAsc" },
        new SortOption { Key = "Sort_ByPriceDesc" }
    };

            foreach (var sortOption in SortOptions)
            {
                sortOption.RefreshDisplayName();
            }

            SelectedSortOption = SortOptions[0];

            OpenDetailsCommand = new RelayCommand<Flower>(OpenDetails);
            AddProductCommand = new RelayCommand(OpenAddProductPage);
            EditProductCommand = new RelayCommand<Flower>(EditProduct);
            DeleteProductCommand = new RelayCommand<Flower>(DeleteProduct);
            ResetFiltersCommand = new RelayCommand(ResetFilters);
            AddToCartCommand = new RelayCommand<Flower>(AddToCart);

            (App.Current as App).LanguageChanged += OnLanguageChanged;
        }
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            foreach (var option in SortOptions)
            {
                option.RefreshDisplayName();
            }
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

        public ObservableCollection<SortOption> SortOptions { get; }

        private SortOption _selectedSortOption;
        public SortOption SelectedSortOption
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

            filtered = SelectedSortOption?.Key switch
            {
                "Sort_ByName" => filtered.OrderBy(f => f.Name),
                "Sort_ByPriceAsc" => filtered.OrderBy(f => f.Price),
                "Sort_ByPriceDesc" => filtered.OrderByDescending(f => f.Price),
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
