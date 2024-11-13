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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wasteNOT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly string connectionString = ConnectionString.GetConnectionString();
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void textEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtEmail.Focus();
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!string.IsNullOrEmpty(txtEmail.Text) &&txtEmail.Text.Length > 0) 
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

        private void txtPassword_PasswordChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword.Password) && txtPassword.Password.Length > 0)
            {
                textPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPassword.Visibility = Visibility.Visible;
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(txtEmail.Text) && !string.IsNullOrEmpty(txtPassword.Password))
            {
                string email = txtEmail.Text;
                string password = txtPassword.Password;
                
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Please fill in all fields");
                    return;
                }
                try
                {
                    if(ValidateAccount(email, password))
                    {
                        UserSession.UserEmail = email;
                        MessageBox.Show("Succesfully Login");

                // Create an instance of MainWindow
                MainWindow mainWindow = new MainWindow();

                // Show the new window
                mainWindow.Show();

                // Optionally, close the current window
                this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid email or password");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured. Please try again later" + ex.Message);
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
        private bool ValidateAccount(string email, string password)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string hashedPassword = HashPassword(password);
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT * FROM public.\"user\" WHERE user_email = @Email AND user_password= @Password";
                    cmd.Parameters.AddWithValue("Email", email);
                    cmd.Parameters.AddWithValue("Password", hashedPassword);
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }
            private void btnToSignup_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of SignUpWindow
            SignUpWindow signUpWindow = new SignUpWindow();

            // Show the new window
            signUpWindow.Show();

            // Optionally, close the current window
            this.Close();
        }

        private string HashPassword(string password) {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the password string to bytes
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Create a StringBuilder to store the resulting hash
                StringBuilder builder = new StringBuilder();

                // Convert each byte into a 2-digit hexadecimal string
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                // Return the final hash string
                return builder.ToString();
            }
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword.Password) && txtPassword.Password.Length > 0)
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
