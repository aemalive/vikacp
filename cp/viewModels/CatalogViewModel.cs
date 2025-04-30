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
                // Когда выбран элемент, выполняется команда
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
            Flowers = new ObservableCollection<Flower>(db.Flowers.ToList());

            OpenDetailsCommand = new RelayCommand<Flower>(OpenDetails);
            AddProductCommand = new RelayCommand(OpenAddProductPage);
            EditProductCommand = new RelayCommand<Flower>(EditProduct);
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
    }
}
