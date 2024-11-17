using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace wasteNOT.Pages
{
    public partial class Payment : Page
    {
        private int _userId;
        private List<CartItemViewModel> _cartItems;
        private int _shippingId;
        private decimal _totalAmount;

        public Payment(int userId, List<CartItemViewModel> cartItems, int shippingId)
        {
            InitializeComponent();

            _userId = userId;
            _cartItems = cartItems;
            _shippingId = shippingId;
            _totalAmount = _cartItems.Sum(item => item.TotalPrice);

            InitializePaymentDetails();
        }

        private void InitializePaymentDetails()
        {
            txtTotalAmount.Text = $"Rp {_totalAmount:N2}";
            var paymentDeadline = DateTime.Now.AddHours(24);
            txtPaymentDueDate.Text = $"Due on {paymentDeadline:dd MMM yyyy HH:mm}";
            txtBankName.Text = "Bank ABC";
            txtAccountNumber.Text = "8806 085219546759";
        }

        
        private int CreateOrder()
        {
            using (var connection = new NpgsqlConnection(ConnectionString.GetConnectionString()))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var orderQuery = @"
                    INSERT INTO ""order"" 
                    (user_id, shipping_id, order_date, status, order_total) 
                    VALUES 
                    (@UserId, @ShippingId, @OrderDate, @Status, @Total)
                    RETURNING order_id";

                        int orderId;
                        using (var command = new NpgsqlCommand(orderQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@UserId", _userId);
                            command.Parameters.AddWithValue("@ShippingId", _shippingId);
                            command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                            command.Parameters.AddWithValue("@Status", "Pending"); // Use enum value
                            command.Parameters.AddWithValue("@Total", (int)_totalAmount);

                            orderId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // Insert Order Items
                        var orderItemQuery = @"
                    INSERT INTO order_item 
                    (order_id, item_id, order_item_qty, order_item_price) 
                    VALUES 
                    (@OrderId, @ItemId, @Quantity, @Price)";

                        foreach (var cartItem in _cartItems)
                        {
                            using (var command = new NpgsqlCommand(orderItemQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@OrderId", orderId);
                                command.Parameters.AddWithValue("@ItemId", cartItem.ItemId);
                                command.Parameters.AddWithValue("@Quantity", cartItem.Quantity);
                                command.Parameters.AddWithValue("@Price", (int)cartItem.Price);

                                command.ExecuteNonQuery();
                            }
                        }

                        // Clear Cart
                        var clearCartQuery = "DELETE FROM cart WHERE user_id = @UserId";
                        using (var command = new NpgsqlCommand(clearCartQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@UserId", _userId);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return orderId;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtAccountNumber.Text);
            MessageBox.Show("Account number copied to clipboard!", "Copy", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnOKPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int orderId = CreateOrder();
                NavigationService.Navigate(new Confirmed(orderId));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Order creation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}