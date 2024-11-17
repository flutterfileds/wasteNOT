using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wasteNOT.SellerPages
{
    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class Products : Page
    {
        private readonly string conn = ConnectionString.GetConnectionString();
        private ObservableCollection<Product> _products;

        public ObservableCollection<Product> ProductsList
        {
            get { return _products; }
            set { _products = value; }
        }
        public Products()
        {
            InitializeComponent();
            _products = new ObservableCollection<Product>();
            dgvProducts.ItemsSource = _products;
            DataContext = this;
        }

        private void LoadProductsFromDatabase()
        {
            try
            {
                _products.Clear();
                using (var connection = new NpgsqlConnection(conn))
                {
                    connection.Open();

                    string userIdQuery = "SELECT user_id FROM public.\"user\" WHERE user_email = @Email";
                    int userId;

                    using (var userCommand = new NpgsqlCommand(userIdQuery, connection))
                    {
                        userCommand.Parameters.AddWithValue("@Email", UserSession.UserEmail);
                        userId = (int)userCommand.ExecuteScalar();
                    }
                    string sql ="SELECT seller_id, item_name, item_type, item_desc, status, item_price FROM public.item WHERE seller_id = @UserId ORDER BY item_id ASC ";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                           
                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    UserID = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Type = reader.GetString(2),
                                    Description = reader.GetString(3),
                                    Status = reader.GetString(4),
                                    Price = reader.GetInt32(5)
                                };

                                _products.Add(product);
                            }
                        }
                        
                    }
                }
            }
            catch (Exception ex)             {
                MessageBox.Show($"Error loading products: "+ex.Message);

            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            LoadProductsFromDatabase();
        }
    }

   
    public class Product
    {

        public  int UserID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int Price
        {    get; set;
        }
    }

    
}
