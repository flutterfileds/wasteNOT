using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

namespace wasteNOT.SellerPages
{
    /// <summary>
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    /// 

    public abstract partial class ProductBase {
        public string Name { get; set; }
        public string Email { get; set; }

        public string Type { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public byte[] Photo { get; set; }

        protected readonly string connectionString = ConnectionString.GetConnectionString();

        public ProductBase(string name, string type, int price, string description, string email, byte[] photo)
        {
            Name = name;
            Price = price;
            Type = type;
            Email = UserSession.UserEmail;
            Description = description;
            Photo = photo;
        }

        public abstract void Add();



    }
    public class AddNewProduct : ProductBase
    {
        public AddNewProduct(string name, string type, int price, string description, string email, byte[] photo) : base(name, type, price, description, email, photo)
        {
        }

        public override void Add()
        {
            using (var connection = new NpgsqlConnection(connectionString)) {
                string query = @"INSERT INTO public.item (item_name, item_price, item_type, item_desc, seller_id, item_pic)
                SELECT 
                @Name,
                @Price,
                @Type,
                @Description,
                @UserId,
                @Photo";

                using (var command = new NpgsqlCommand(query, connection)) { 
                command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Price", Price);
                    command.Parameters.AddWithValue("@Type", Type);
                    command.Parameters.AddWithValue("@Description", Description);
                    command.Parameters.AddWithValue("@UserId", UserSession.CurrentUserId);
                    command.Parameters.AddWithValue("@Photo", Photo);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }


        }
    }
        public partial class AddProduct : Page
    {
        
        public AddProduct()
        {
            InitializeComponent();
        }

        private byte[] selectedPhoto;
        private void btnSaveProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    if (selectedPhoto == null)
                    {
                        MessageBox.Show("Please select a product photo.");
                        return;
                    }

                    var product = new AddNewProduct(txtProductName.Text, txtProductType.Text, int.Parse(txtPrice.Text),
                        txtDescription.Text, UserSession.UserEmail, selectedPhoto);

                    product.Add();
                    MessageBox.Show("Product added successfully");
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error registering item: " + ex.Message);
            }
        }

        private void btnProductPhoto_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Select a Photo"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                lblPhotoName.Text = System.IO.Path.GetFileName(openFileDialog.FileName);
                selectedPhoto = System.IO.File.ReadAllBytes(openFileDialog.FileName);
            }
        }
            
        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtProductName.Text) || string.IsNullOrEmpty(txtProductType.Text) ||
        string.IsNullOrEmpty(txtPrice.Text) || string.IsNullOrEmpty(txtDescription.Text))
            {
                MessageBox.Show("Please fill in all fields");
                return false;
            }
            return true;
        }

        private void ClearInputs()
        {
            txtProductName.Text = string.Empty;
            txtProductType.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtDescription.Text = string.Empty;
            lblPhotoName.Text = "No photo selected";
            selectedPhoto = null;
        }
    }
}
