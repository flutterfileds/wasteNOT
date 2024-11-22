using Npgsql;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace wasteNOT
{
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
            if (!string.IsNullOrEmpty(txtEmail.Text) && txtEmail.Text.Length > 0)
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

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmail.Text) && !string.IsNullOrEmpty(txtPassword.Password))
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
                    var (isValid, userId, userName) = ValidateAccount(email, password);
                    if (isValid)
                    {
                        UserSession.UserEmail = email;
                        UserSession.Login(userId,userName);
                        MessageBox.Show("Successfully Logged In");

                        // Create an instance of MainWindow
                        MainWindow mainWindow = new MainWindow();

                        // Show the new window
                        mainWindow.Show();

                        // Close the current window
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid email or password");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred. Please try again later\n" + ex.Message);
                }
            }
        }

        private (bool isValid, int userId, string userName) ValidateAccount(string email, string password)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string hashedPassword = HashPassword(password);
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT user_id, user_name FROM public.\"user\" WHERE user_email = @Email AND user_password = @Password";
                    cmd.Parameters.AddWithValue("Email", email);
                    cmd.Parameters.AddWithValue("Password", hashedPassword);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32(0); // Get the user_id from the first column
                            string userName = reader.GetString(1); // Get the user_name from the second column
                            return (true, userId, userName);
                        }
                        return (false, 0, "");
                    }
                }
            }
        }

        private void btnToSignup_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow signUpWindow = new SignUpWindow();
            signUpWindow.Show();
            this.Close();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}