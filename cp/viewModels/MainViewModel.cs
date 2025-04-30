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
using System.Windows.Controls;

namespace cp.viewModels
{
    public class MainViewModel : BaseViewModel
    {
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
        }

        private void NavigateToPage(string pageName)
        {
            Page page = pageName switch
            {
                "LoginPage" => new LoginPage(),
                "RegisterPage" => new RegisterPage(),
                "CatalogPage" => new CatalogPage(),
                "BasketPage" => new BasketPage(),
                "ProfilePage" => AuthService.CurrentUser?.Role == "GUEST"
                        ? new LoginPage()  
                        : AuthService.IsAuthenticated
                            ? (Page)new ProfilePage(new ProfileViewModel(NavigateToPage))
                            : new LoginPage(),  
                _ => CurrentPage
            };

            CurrentPage = page;
        }
    }

}
