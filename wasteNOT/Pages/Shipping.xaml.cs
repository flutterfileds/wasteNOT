using Npgsql;
using System;
using System.Collections.Generic;
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
using System.Xml.Linq;

namespace wasteNOT.Pages
{
    /// <summary>
    /// Interaction logic for Shipping.xaml
    /// </summary>
    public partial class Shipping : Page
    {
        private int _userId;
        private List<CartItemViewModel> _cartItems;
        private decimal _totalOrderAmount;

        public Shipping(int userId, List<CartItemViewModel> cartItems)
        {
            InitializeComponent();
            _userId = userId;
            _cartItems = cartItems;
            _totalOrderAmount = cartItems.Sum(item => item.TotalPrice);

            // Load user and shipping details
            LoadUserDetails();
            LoadExistingAddress();

        }

        private void LoadUserDetails()
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString.GetConnectionString()))
                {
                    connection.Open();
                    var query = @"
                    SELECT user_name, user_phone 
                    FROM ""user"" 
                    WHERE user_id = @UserId";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", _userId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Populate user details in UI
                                txtNameCust.Text = reader["user_name"].ToString();
                                txtNumCust.Text = reader["user_phone"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnProceedToPayment_Click(object sender, RoutedEventArgs e)
        {
            // Validate shipping details
            if (ValidateShippingDetails())
            {
                // Create shipping record
                int shippingId = CreateShippingRecord();

                // Navigate to payment page
                NavigationService.Navigate(new Payment(_userId, _cartItems, shippingId));
            }
        }

        private bool ValidateShippingDetails()
        {
            // Comprehensive validation
            if (string.IsNullOrWhiteSpace(txtNameCust.Text))
            {
                MessageBox.Show("Please enter a name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNumCust.Text))
            {
                MessageBox.Show("Please enter a phone number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtStreetAdd.Text))
            {
                MessageBox.Show("Please enter a street address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCity.Text))
            {
                MessageBox.Show("Please enter a city.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPostcode.Text))
            {
                MessageBox.Show("Please enter a postal code.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private int CreateShippingRecord()
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString.GetConnectionString()))
                {
                    connection.Open();
                    var query = @"
                    INSERT INTO shipping 
                    (user_id, shipping_type, shipping_date, shipping_cost) 
                    VALUES 
                    (@UserId, @ShippingType, @ShippingDate, @ShippingCost) 
                    RETURNING shipping_id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", _userId);
                        command.Parameters.AddWithValue("@ShippingType", "Standard"); // Or get from UI
                        command.Parameters.AddWithValue("@ShippingDate", DateTime.Now);
                        command.Parameters.AddWithValue("@ShippingCost", CalculateShippingCost());

                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating shipping record: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }

        private decimal CalculateShippingCost()
        {
            // Implement shipping cost calculation logic
            return 10000; // Example fixed shipping cost
        }

        private void btnOKShipping_Click(object sender, RoutedEventArgs e)
        {
            // This method can be used as a fallback or to add additional logic
            // Currently, the main navigation is handled by btnProceedToPayment_Click
            if (ValidateShippingDetails())
            {
                
                NavigationService.Navigate(new Payment(_userId, _cartItems, shippingId));
            }
        }

        private int shippingId;
        private void btnAddAdress_Click(object sender, RoutedEventArgs e)
        {
             shippingId = CreateShippingRecord();
            if (ValidateShippingDetails())
            {
                try
                {
                    using (var connection = new NpgsqlConnection(ConnectionString.GetConnectionString()))
                    {
                        connection.Open();
                        var query = @"
                    INSERT INTO user_address 
                    (user_id, address_type, ad_street, ad_city, ad_postalcode) 
                    VALUES 
                    (@UserId, @AddressType, @Street, @City, @PostalCode)
                    RETURNING usaddress_id";

                        using (var command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@UserId", _userId);
                            command.Parameters.AddWithValue("@AddressType", "Shipping"); // You can add a combobox in UI for different address types
                            command.Parameters.AddWithValue("@Street", txtStreetAdd.Text);
                            command.Parameters.AddWithValue("@City", txtCity.Text);
                            command.Parameters.AddWithValue("@PostalCode", txtPostcode.Text);

                            int addressId = Convert.ToInt32(command.ExecuteScalar());

                            if (addressId > 0)
                            {
                                MessageBox.Show("Address saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                                // Also create shipping record
                                shippingId = CreateShippingRecord();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving address: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadExistingAddress()
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString.GetConnectionString()))
                {
                    connection.Open();
                    var query = @"
                SELECT * FROM user_address 
                WHERE user_id = @UserId 
                AND address_type = 'Shipping' 
                ORDER BY usaddress_id DESC 
                LIMIT 1";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", _userId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtStreetAdd.Text = reader["ad_street"].ToString();
                                txtCity.Text = reader["ad_city"].ToString();
                                txtPostcode.Text = reader["ad_postalcode"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading address: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
