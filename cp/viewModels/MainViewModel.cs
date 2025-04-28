using cp.viewModels;
using cp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using cp.views;
using cp.commands;
using cp.services;

namespace cp.viewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _currentPage;
        public string CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                NavigateToPage(value);
            }
        }

        public ICommand NavigateCommand { get; }

        public MainViewModel()
        {
            NavigateCommand = new RelayCommand<string>(NavigateToPage);
        }


        private void NavigateToPage(string pageName)
        {
            var frame = (App.Current.MainWindow as MainWindow).MainFrame;
            switch (pageName)
            {
                case "CatalogPage":
                    frame.Navigate(new CatalogPage());
                    break;
                case "BasketPage":
                    frame.Navigate(new BasketPage());
                    break;
                case "ProfilePage":
                    if (!AuthService.IsAuthenticated)
                        frame.Navigate(new LoginPage());
                    else
                        frame.Navigate(new ProfilePage());
                    break;
                default:
                    break;
            }
        }
    }

}
