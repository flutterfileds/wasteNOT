using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wasteNOT
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    /// 

    public abstract class User
    {
        public string Email { get; set; }
        public string PhoneNum { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        protected readonly string connectionString = ConnectionString.GetConnectionString();

        public User(string email, string phone, string name, string password)
        {
            Email = email.ToLower().Trim();
            PhoneNum = phone.Trim();
            Name = name.Trim();
            Password = HashPassword(password);
        }

        public abstract void Insert();

        public abstract bool ValidateInput();

        public abstract void ClearInputs();

        protected string HashPassword(string password)
        {
            using (SHA256 sHA256 = SHA256.Create())
            {
                byte[] bytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }

        }
    }

    public class AllUser : User
    {
        public AllUser(string email, string phonenum, string name, string password) : base(email, phonenum, name, password) { }



        public override void Insert()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                string query = @"INSERT INTO public.""user"" (user_email, user_phone, user_name, user_password) VALUES (@Email, @Phone, @Name, @Password)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Phone", PhoneNum);
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Password", Password);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

        }

        public override bool ValidateInput()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(PhoneNum) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Please fill in all fields");
                return false;
            }
            if (!Email.Contains('@') || !Email.Contains('.'))
            {
                MessageBox.Show("Please enter a valid email address");
                return false;
            }

            if (Password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long");
                return false;
            }
            return true;
        }

        public override  void ClearInputs()
        {
            throw new NotImplementedException();
        }
    }
        
   
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void btnToLogin_Click(object sender, RoutedEventArgs e)
        {

            // Create an instance of the Login window
            LoginWindow loginWindow = new LoginWindow();

            // Show the Login window
            loginWindow.Show();

            // Close the current MainWindow
            if (this != null)
                this.Close();
        }

        private void textEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtEmail.Focus();
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmail.Text))
            {
                textEmail.Visibility = Visibility.Collapsed;
            }
            else
            {
                textEmail.Visibility = Visibility.Visible;
            }
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtPassword.Focus();
        }

        
        private void txtPhoneNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPhoneNum.Text))
            {
                textPhoneNum.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPhoneNum.Visibility = Visibility.Visible;
            }
        }

        private void textPhoneNum_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtPhoneNum.Focus();
        }

        private void textUsername_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtUsername.Focus();
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUsername.Text))
            {
                textUsername.Visibility = Visibility.Collapsed;
            }
            else
            {
                textUsername.Visibility = Visibility.Visible;
            }
        }

        private void btnSignup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    var user = new AllUser(txtEmail.Text, txtPhoneNum.Text, txtUsername.Text, txtPassword.Password);

                    user.Insert();
                    MessageBox.Show("Successfully signed up!");
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error registering User" + ex.Message);
            }
        }

        public  bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtPassword.Password) || string.IsNullOrEmpty(txtPhoneNum.Text) || string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Please fill in all fields");
                return false;
            }
            if (!txtEmail.Text.Contains('@') || !txtEmail.Text.Contains('.'))
            {
                MessageBox.Show("Please enter a valid email address");
                return false;
            }

            if (txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long");
                return false;
            }
            return true;
        }

        public void ClearInputs()
        {
            txtEmail.Text = string.Empty;
            txtPassword.Password = string.Empty;
            txtPhoneNum.Text = string.Empty;
            txtUsername.Text = string.Empty;
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword.Password))
            {
                textPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPassword.Visibility = Visibility.Visible;
            }
        }
    }

}
