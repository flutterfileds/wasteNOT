using Npgsql;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;

namespace wasteNOT
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int ShippingId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal OrderTotal { get; set; }
    }

    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value as string)?.ToLower() switch
            {
                "pending" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500")),
                "in progress" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007BFF")),
                "shipped" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#17A2B8")),
                "completed" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#28A745")),
                _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6C757D"))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class History : Page
    {
        private List<Order> orders;

        public History()
        {
            InitializeComponent();
            LoadOrders();

            // Add search functionality
            
        }

        private void LoadOrders()
        {
            try
            {
                orders = new List<Order>();
                using (var connection = new NpgsqlConnection(ConnectionString.GetConnectionString()))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM public.order ORDER BY order_date DESC", connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                OrderId = reader.GetInt32(reader.GetOrdinal("order_id")),
                                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                                ShippingId = reader.GetInt32(reader.GetOrdinal("shipping_id")),
                                Status = reader.GetString(reader.GetOrdinal("status")),
                                OrderTotal = reader.GetDecimal(reader.GetOrdinal("order_total"))
                            });
                        }
                    }
                }
                OrdersList.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void textSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                OrdersList.ItemsSource = orders;
                return;
            }

            var filteredOrders = orders.FindAll(o =>
                o.OrderId.ToString().Contains(searchText) ||
                o.ShippingId.ToString().Contains(searchText) ||
                o.Status.ToLower().Contains(searchText)
            );

            OrdersList.ItemsSource = filteredOrders;
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                var selectedOrder = orders.Find(o => o.OrderId == orderId);
                if (selectedOrder != null)
                {
                    // Navigate to order details page - implement as needed
                    // NavigationService.Navigate(new OrderDetails(selectedOrder));
                    MessageBox.Show($"Viewing details for Order #{orderId}");
                }
            }
        }

        private void textSearch_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}