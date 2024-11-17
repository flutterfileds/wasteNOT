using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace wasteNOT.Pages
{
    public partial class Cart : Page, INotifyPropertyChanged
    {
        public ObservableCollection<CartItemViewModel> CartItems { get; set; }

        private readonly string _connectionString;
        private int _currentUserId;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Cart(int userId)
        {
            InitializeComponent();
            _connectionString = ConnectionString.GetConnectionString();
            _currentUserId = userId;

            CartItems = new ObservableCollection<CartItemViewModel>();
            DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCartItems();
        }

        private void LoadCartItems()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        SELECT 
                            i.item_id AS ItemId,
                            i.item_name AS ProductName, 
                            i.item_desc AS Description, 
                            c.quantity AS Quantity, 
                            i.item_price AS Price
                        FROM cart c
                        JOIN item i ON c.item_id = i.item_id
                        WHERE c.user_id = @UserId";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", _currentUserId);

                        using (var reader = command.ExecuteReader())
                        {
                            CartItems.Clear();
                            while (reader.Read())
                            {
                                CartItems.Add(new CartItemViewModel
                                {
                                    ItemId = reader.GetInt32(reader.GetOrdinal("ItemId")),
                                    ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                                });
                            }
                        }
                    }
                }

                UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cart: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSummary()
        {
            if (CartItems.Any())
            {
                var subtotal = CartItems.Sum(item => item.TotalPrice);
                var tax = subtotal * 0.08m;
                var total = subtotal + tax;

                txtSubtotal.Text = $"Rp{subtotal:N2}";
                txtTax.Text = $"Rp{tax:N2}";
                txtTotal.Text = $"Rp{total:N2}";

                btnOrder.IsEnabled = true;
            }
            else
            {
                txtSubtotal.Text = "Rp0.00";
                txtTax.Text = "Rp0.00";
                txtTotal.Text = "Rp0.00";
                btnOrder.IsEnabled = false;
            }
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to shipping or payment page
            NavigationService.Navigate(new Shipping(_currentUserId, CartItems.ToList()));
        }
    }

    public class CartItemViewModel : INotifyPropertyChanged
    {
        public int ItemId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public decimal Price { get; set; }
        public decimal TotalPrice => Quantity * Price;

        public ICommand IncreaseQuantityCommand => new RelayCommand(IncreaseQuantity, CanIncreaseQuantity);
        public ICommand DecreaseQuantityCommand => new RelayCommand(DecreaseQuantity, CanDecreaseQuantity);

        private void IncreaseQuantity()
        {
            Quantity++;
            UpdateCartItemQuantity();
        }

        private bool CanIncreaseQuantity() => true; // Add stock check logic

        private void DecreaseQuantity()
        {
            if (Quantity > 1)
            {
                Quantity--;
                UpdateCartItemQuantity();
            }
        }

        private bool CanDecreaseQuantity() => Quantity > 1;

        private void UpdateCartItemQuantity()
        {
            // Update quantity in database
            // Implement database update logic here
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Utility command class
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();
    }
}