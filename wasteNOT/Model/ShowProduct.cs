using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using wasteNOT.SellerPages;

namespace wasteNOT.Model
{
    public class ShowProductAccess
    {
        private readonly string _connectionString = ConnectionString.GetConnectionString(); // Your connection string

        public class Product
        {
            public int ItemId { get; set; }  // Changed from Id to match database
            public string Name { get; set; }  // Maps to item_name
            public string Category { get; set; } // Maps to item_type
            public string Description { get; set; } // Maps to item_desc
            public decimal Price { get; set; } // Maps to item_price
            public string Status { get; set; } // Maps to status
            public string ImageSource { get; set; } // You'll need to add this to your database if not exists
            public int Quantity { get; set; } = 0;
            public string FormattedPrice => Price.ToString("C");
        }

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                    SELECT i.item_id, i.item_name, i.item_type, i.item_desc, 
                           i.status, i.item_price,
                           COALESCE(c.quantity, 0) as cart_quantity
                    FROM item i
                    LEFT JOIN cart c ON i.item_id = c.item_id 
                    AND c.user_id = @userId
                    WHERE i.status = 'available'::item_status";

                    cmd.Parameters.AddWithValue("@userId", GetCurrentUserId());

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ItemId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Category = reader.GetString(2),
                                Description = reader.GetString(3),
                                Status = reader.GetString(4),
                                Price = reader.GetInt32(5),
                                Quantity = reader.GetInt32(6)
                            });
                        }
                    }
                }
            }
            return products;
        }

        public void AddToCart(int itemId, int quantity)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                    INSERT INTO cart (user_id, item_id, quantity)
                    VALUES (@userId, @itemId, @quantity)
                    ON CONFLICT (user_id, item_id)
                    DO UPDATE SET quantity = cart.quantity + @quantity";

                    cmd.Parameters.AddWithValue("@userId", GetCurrentUserId());
                    cmd.Parameters.AddWithValue("@itemId", itemId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateCartQuantity(int itemId, int quantity)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                    UPDATE cart 
                    SET quantity = @quantity 
                    WHERE item_id = @itemId AND user_id = @userId";

                    cmd.Parameters.AddWithValue("@userId", GetCurrentUserId());
                    cmd.Parameters.AddWithValue("@itemId", itemId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RemoveFromCart(int itemId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                    DELETE FROM cart 
                    WHERE item_id = @itemId AND user_id = @userId";

                    cmd.Parameters.AddWithValue("@userId", GetCurrentUserId());
                    cmd.Parameters.AddWithValue("@itemId", itemId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private int GetCurrentUserId()
        {
            // Implement based on your authentication system
            // For example, you might store the current user ID in a static property
            return UserSession.CurrentUserId; // You'll need to implement this
        }

        public List<Product> GetProductsByCategory(string category)
        {
            var products = new List<Product>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                    SELECT i.item_id, i.item_name, i.item_type, i.item_desc, 
                           i.status, i.item_price,
                           COALESCE(c.quantity, 0) as cart_quantity
                    FROM item i
                    LEFT JOIN cart c ON i.item_id = c.item_id 
                    AND c.user_id = @userId
                    WHERE i.item_type = @category 
                    AND i.status = 'available'::item_status";

                    cmd.Parameters.AddWithValue("@userId", GetCurrentUserId());
                    cmd.Parameters.AddWithValue("@category", category);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ItemId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Category = reader.GetString(2),
                                Description = reader.GetString(3),
                                Status = reader.GetString(4),
                                Price = reader.GetInt32(5),
                                Quantity = reader.GetInt32(6)
                            });
                        }
                    }
                }
            }
            return products;
        }
    }
}
