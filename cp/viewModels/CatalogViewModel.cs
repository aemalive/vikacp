using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cp.commands;
using cp.models;
using System.Windows.Input;
using cp.views;
using cp.services;

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

        public ICommand AddProductCommand { get; }

        public ICommand EditProductCommand { get; }


        public CatalogViewModel()
        {
            using var db = new FlowerShopDbContext();
            _allFlowers = new ObservableCollection<Flower>(db.Flowers.ToList());
            FilteredFlowers = new ObservableCollection<Flower>(_allFlowers);

            OpenDetailsCommand = new RelayCommand<Flower>(OpenDetails);
            AddProductCommand = new RelayCommand(OpenAddProductPage);
            EditProductCommand = new RelayCommand<Flower>(EditProduct);

            ResetFiltersCommand = new RelayCommand(ResetFilters);
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

    }
}
