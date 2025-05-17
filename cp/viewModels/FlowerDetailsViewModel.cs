using cp.commands;
using cp.models;
using cp.services;
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
    public class FlowerDetailsViewModel : BaseViewModel
    {
        private readonly Flower _flower;
        private readonly FlowerShopDbContext _dbContext = new FlowerShopDbContext();
        public Flower CurrentFlower => _flower;

        public string Name => _flower.Name;
        public decimal Price => _flower.Price;
        public string Description => _flower.Description;
        public string ImageURL => _flower.ImageURL;

        public ObservableCollection<Review> Reviews { get; set; } = new ObservableCollection<Review>();

        private string _reviewTitle;
        public string ReviewTitle
        {
            get => _reviewTitle;
            set { _reviewTitle = value; OnPropertyChanged(); }
        }

        private string _reviewContent;
        public string ReviewContent
        {
            get => _reviewContent;
            set { _reviewContent = value; OnPropertyChanged(); }
        }

        private int _reviewScore;
        public int ReviewScore
        {
            get => _reviewScore;
            set { _reviewScore = value; OnPropertyChanged(); }
        }

        public bool IsUserAuthenticated => AuthService.IsAuthenticated;
        public User AuthenticatedUser => AuthService.CurrentUser;

        public ICommand SubmitReviewCommand { get; }
        public ICommand AddToCartCommand { get; }

        public FlowerDetailsViewModel(Flower flower)
        {
            _flower = flower;
            SubmitReviewCommand = new RelayCommand(SubmitReview);
            DeleteReviewCommand = new RelayCommand<Review>(DeleteReview, CanDeleteReview);
            AddToCartCommand = new RelayCommand<Flower>(AddToCart);

            LoadReviews();
        }
        private bool CanDeleteReview(Review review)
        {
            if (review == null || AuthService.CurrentUser == null)
                return false;

            return IsAdmin || review.UserId == AuthService.CurrentUser.Id;
        }


        private void SubmitReview()
        {
            if (!AuthService.IsAuthenticated)
            {
                MessageBox.Show("Вы должны войти в систему, чтобы оставить отзыв.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ReviewTitle))
            {
                MessageBox.Show("Введите заголовок отзыва.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ReviewTitle.Length > 100)
            {
                MessageBox.Show("Заголовок не должен превышать 100 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ReviewContent))
            {
                MessageBox.Show("Введите текст отзыва.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ReviewContent.Length > 500)
            {
                MessageBox.Show("Текст отзыва не должен превышать 500 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ReviewScore < 0 || ReviewScore > 10)
            {
                MessageBox.Show("Оценка должна быть в диапазоне от 0 до 10.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var review = new Review
            {
                Title = ReviewTitle,
                Content = ReviewContent,
                Score = ReviewScore,
                UserId = AuthService.CurrentUser.Id,
                FlowerId = _flower.Id
            };

            _dbContext.Reviews.Add(review);
            _dbContext.SaveChanges();

            _dbContext.Entry(review).Reference(r => r.User).Load(); 
            Reviews.Add(review);

            ReviewTitle = string.Empty;
            ReviewContent = string.Empty;
            ReviewScore = 0;
        }

        public ICommand DeleteReviewCommand { get; }

        public bool IsAdmin => AuthService.CurrentUser?.Role == "ADMIN";

        public bool IsUser => AuthService.CurrentUser?.Role == "USER";

        private void LoadReviews()
        {
            var reviews = _dbContext.Reviews
                .Where(r => r.FlowerId == _flower.Id)
                .ToList();

            Reviews.Clear();
            foreach (var review in reviews)
            {
                _dbContext.Entry(review).Reference(r => r.User).Load();
                Reviews.Add(review);
            }
        }

        private void DeleteReview(Review review)
        {
            if (!CanDeleteReview(review))
            {
                MessageBox.Show("Вы не можете удалить этот отзыв.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Удалить отзыв?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _dbContext.Reviews.Remove(review);
                _dbContext.SaveChanges();
                Reviews.Remove(review);
            }
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

            MessageBox.Show($"Цветок «{flower.Name}» добавлен в корзину.");
        }
    }
}
