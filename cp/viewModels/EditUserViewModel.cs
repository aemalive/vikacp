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

namespace cp.viewModels
{
    public class EditUserViewModel : BaseViewModel
    {
        private User _user;
        public User User
        {
            get => _user;
            set { _user = value; OnPropertyChanged(); }
        }

        public string Username
        {
            get => User.Username;
            set { User.Username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => User.Password;
            set { User.Password = value; OnPropertyChanged(); }
        }

        public string FullName
        {
            get => User.FullName;
            set { User.FullName = value; OnPropertyChanged(); }
        }

        public string Role
        {
            get => User.Role;
            set { User.Role = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => User.Email;
            set { User.Email = value; OnPropertyChanged(); }
        }
        public string PreferredAddress
        {
            get => User.PreferredAddress;
            set { User.PreferredAddress = value; OnPropertyChanged(); }
        }


        public ICommand SaveChangesCommand { get; }
        public ICommand ResetPasswordCommand { get; }

        public EditUserViewModel(User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));

            SaveChangesCommand = new RelayCommand(SaveChanges);
        }

        private void SaveChanges()
        {
            using var db = new FlowerShopDbContext();
            db.Users.Update(User);
            db.SaveChanges();

            string message = TryFindResource("UserUpdated") ?? "Пользователь успешно обновлён!";
            string caption = TryFindResource("Success") ?? "Успех";

            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);

            var mainVm = App.Current.MainWindow.DataContext as MainViewModel;
            if (mainVm != null)
            {
                Action<string> nav = pageName => mainVm.NavigateCommand.Execute(pageName);
                mainVm.CurrentPage = new ProfilePage(new ProfileViewModel(nav));
            }
        }

        private string? TryFindResource(string key)
        {
            return Application.Current.TryFindResource(key) as string;
        }

    }
}
