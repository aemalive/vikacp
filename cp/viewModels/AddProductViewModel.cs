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
    public class AddProductViewModel : BaseViewModel
    {
        private string _name;
        private string _price;
        private string _description;
        private string _imageURL;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public string ImageURL
        {
            get => _imageURL;
            set { _imageURL = value; OnPropertyChanged(); }
        }

        public ICommand AddProductCommand { get; }
        public ICommand SelectImageCommand { get; }

        public AddProductViewModel()
        {
            AddProductCommand = new RelayCommand(AddProduct);
            SelectImageCommand = new RelayCommand(SelectImage);
        }

        private void SelectImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Выберите изображение"
            };

            if (dialog.ShowDialog() == true)
            {
                ImageURL = dialog.FileName;
            }
        }

        private void AddProduct()
        {
            if (string.IsNullOrWhiteSpace(ImageURL))
            {
                MessageBox.Show("Пожалуйста, выберите изображение.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Название обязательно для заполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(Price, out var price))
            {
                MessageBox.Show("Введите корректную стоимость.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (price < 15 || price > 800)
            {
                MessageBox.Show("Стоимость должна быть от 15 до 800 рублей.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var flower = new Flower
            {
                Name = Name,
                Price = price,
                Description = Description,
                ImageURL = ImageURL
            };

            try
            {
                using (var db = new FlowerShopDbContext())
                {
                    db.Flowers.Add(flower);
                    db.SaveChanges();
                }

                MessageBox.Show("Товар успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                (App.Current.MainWindow.DataContext as MainViewModel).CurrentPage = new CatalogPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
