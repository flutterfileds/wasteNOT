using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wasteNOT.Model;

namespace wasteNOT.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page, INotifyPropertyChanged
    {
        private string _welcomeMessage;
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set
            {
                if (_welcomeMessage != value)
                {
                    _welcomeMessage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WelcomeMessage)));
                }
            }
        }

        private readonly ShowProductAccess _productData;

        public event PropertyChangedEventHandler PropertyChanged;
        public Home()
        {
            InitializeComponent();
            _productData = new ShowProductAccess();
            UpdateWelcomeMessage();
            LoadProducts();
        }

        private void UpdateWelcomeMessage()
        {
            string username = UserSession.CurrentUserName ?? "Guest";
            WelcomeMessage = $"Welcome {username}!";
        }

        private void LoadProducts()
        {
            try
            {
                var products = _productData.GetAllProducts();
                ProductsGrid.ItemsSource = products;
            }
            catch(Exception ex) {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // Implement search functionality
            string searchTerm = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadProducts();
            }
            else
            {
                var products = _productData.GetAllProducts()
                    .Where(p => p.Name.ToLower().Contains(searchTerm) ||
                               p.Category.ToLower().Contains(searchTerm))
                    .ToList();
                ProductsGrid.ItemsSource = products;
            }
        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Pages/Settings.xaml", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string category = button?.Content.ToString();

            try
            {
                if (category == "All")
                {
                    LoadProducts();
                }
                else
                {
                    var products = _productData.GetProductsByCategory(category);
                    ProductsGrid.ItemsSource = products;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void textSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSearch.Focus();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text) && txtSearch.Text.Length > 0)
            {
                textSearch.Visibility = Visibility.Collapsed;
            }
            else
            {
                textSearch.Visibility = Visibility.Visible;
            }
        }

        private void btnDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn)
            {
                MessageBox.Show("Please log in to modify cart", "Login Required",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var button = sender as Button;
            if (button?.DataContext is ShowProductAccess.Product product && product.Quantity > 0)
            {
                try
                {
                    product.Quantity--;
                    if (product.Quantity == 0)
                    {
                        _productData.RemoveFromCart(product.ItemId);
                    }
                    else
                    {
                        _productData.UpdateCartQuantity(product.ItemId, product.Quantity);
                    }
                    ProductsGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating cart: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    product.Quantity++; // Revert on error
                    ProductsGrid.Items.Refresh();
                }
            }
        }
        private void btnIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn)
            {
                MessageBox.Show("Please log in to add items to cart", "Login Required",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var button = sender as Button;
            if (button?.DataContext is ShowProductAccess.Product product)
            {
                if (product.Status != "available")
                {
                    MessageBox.Show("This item is currently not available", "Unavailable",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                try
                {
                    product.Quantity++;
                    if (product.Quantity == 1)
                    {
                        _productData.AddToCart(product.ItemId, 1);
                    }
                    else
                    {
                        _productData.UpdateCartQuantity(product.ItemId, product.Quantity);
                    }
                    ProductsGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating cart: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    product.Quantity--; // Revert on error
                    ProductsGrid.Items.Refresh();
                }
            }
        }
    }
}
