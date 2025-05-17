using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cp.commands;
using cp.models;
using cp.views;
using System.Windows.Input;
using System.Windows;
using Microsoft.Win32;

namespace cp.viewModels
{
    public class EditProductViewModel : BaseViewModel
    {
        private Flower _flower;
        public Flower Flower
        {
            get => _flower;
            set { _flower = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => Flower.Name;
            set { Flower.Name = value; OnPropertyChanged(); }
        }

        public string Price
        {
            get => Flower.Price.ToString();
            set
            {
                if (decimal.TryParse(value, out var price))
                {
                    Flower.Price = price;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => Flower.Description;
            set { Flower.Description = value; OnPropertyChanged(); }
        }

        public string ImageURL
        {
            get => Flower.ImageURL;
            set { Flower.ImageURL = value; OnPropertyChanged(); }
        }

        public ICommand SaveChangesCommand { get; }
        public ICommand SelectImageCommand { get; }
        public EditProductViewModel(Flower flower)
        {
            Flower = flower ?? throw new ArgumentNullException(nameof(flower));

            SaveChangesCommand = new RelayCommand(SaveChanges);
            SelectImageCommand = new RelayCommand(SelectImage);
        }

        private void SaveChanges()
        {
            using (var db = new FlowerShopDbContext())
            {
                if (string.IsNullOrWhiteSpace(ImageURL))
                {
                    MessageBox.Show(GetString("Err_SelectImage"), GetString("Error_Title"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Name))
                {
                    MessageBox.Show(GetString("Err_NameRequired"), GetString("Error_Title"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(Price, out var price))
                {
                    MessageBox.Show(GetString("Err_InvalidPrice"), GetString("Error_Title"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (price < 15 || price > 800)
                {
                    MessageBox.Show(GetString("Err_PriceRange"), GetString("Error_Title"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                db.Flowers.Update(Flower);
                db.SaveChanges();
            }

            MessageBox.Show(GetString("Success_SaveChanges"), GetString("Success_Title"), MessageBoxButton.OK, MessageBoxImage.Information);

            (App.Current.MainWindow.DataContext as MainViewModel).CurrentPage = new CatalogPage();
        }
        private void SelectImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = GetString("Dialog_SelectImageFilter"),
                Title = GetString("Dialog_SelectImageTitle")
            };

            if (dialog.ShowDialog() == true)
            {
                ImageURL = dialog.FileName;
            }
        }

        private string GetString(string key) =>
    Application.Current.TryFindResource(key) as string ?? $"[{key}]";

    }
}
