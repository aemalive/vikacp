using cp;
using cp.commands;
using cp.helpers;
using cp.services;
using cp.viewModels;
using cp.views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace cp.viewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<LanguageItem> Languages { get; }
        private LanguageItem _selectedLanguage;
        public LanguageItem SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    OnPropertyChanged();
                    ChangeLanguageCommand.Execute(value);
                }
            }
        }

        public ICommand ChangeLanguageCommand { get; }
        private Page _currentPage;
        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateCommand { get; }

        public MainViewModel()
        {
            NavigateCommand = new RelayCommand<string>(NavigateToPage);
            CurrentPage = new CatalogPage();
            Languages = new ObservableCollection<LanguageItem>
            {
                new LanguageItem { Name = "Русский", CultureCode = "ru-RU" },
                new LanguageItem { Name = "English", CultureCode = "en-US" }
            };
            _selectedLanguage = Languages[0];

            ChangeLanguageCommand = new RelayCommand<LanguageItem>(lang =>
            {
                var app = Application.Current as cp.App;
                app?.LoadLanguageDictionary(lang.CultureCode);
            });
        }

        private void NavigateToPage(string pageName)
        {
            if (pageName == "CartPage")
            {
                if (!AuthService.IsAuthenticated)
                {
                    MessageBox.Show("Вы не авторизованы. Пожалуйста, войдите в систему.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (AuthService.CurrentUser?.Role == "ADMIN")
                {
                    MessageBox.Show("Доступ в корзину доступен только для пользователей.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }



            if (pageName == "ProfilePage" && (AuthService.CurrentUser?.Role == "GUEST" || !AuthService.IsAuthenticated))
            {
                CurrentPage = new LoginPage();
                return;
            }

            CurrentPage = pageName switch
            {
                "LoginPage" => new LoginPage(),
                "RegisterPage" => new RegisterPage(),
                "CatalogPage" => new CatalogPage(),
                "CartPage" => new CartPage(),
                "ProfilePage" => new ProfilePage(new ProfileViewModel(NavigateToPage)),
                _ => CurrentPage
            };
        }

    }

}
