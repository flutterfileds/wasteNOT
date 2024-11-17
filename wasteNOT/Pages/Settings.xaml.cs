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

namespace wasteNOT.Pages
{

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        protected readonly string connectionString = ConnectionString.GetConnectionString();
        public Settings()
        {
            InitializeComponent();
        }

        private void btnSaveAcc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = new UpdateUser(UserSession.UserEmail,txtUsername.Text,txtNumber.Text ,txtEmail.Text, txtPassword.Password  );
                if (true)
                { 
                    user.Update();
                    MessageBox.Show("Account updated successfully");
                    ClearInputs();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error updating account" + ex.Message);
            }

        }

        public void ClearInputs()
        {
            txtEmail.Text = string.Empty;
            txtPassword.Password = string.Empty;
            txtNumber.Text = string.Empty;
            txtUsername.Text = string.Empty;
        }
    }

    public class UpdateUser : User
        {
        public UpdateUser(string oldemail, string username, string phonenum, string email, string password) : base( username, phonenum, email, password)
        {
        }

        public override void Insert()
        {
            throw new NotImplementedException();
        }

        public override void ClearInputs()
        {
            
        }

        public void Update()
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    string query = "UPDATE public.user SET user_name = @Username,user_phone = @Phonenum, user_email = @Email, user_password = @Password WHERE user_email = @OldEmail";
                    connection.Open();
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@OldEmail", UserSession.UserEmail);
                        cmd.Parameters.AddWithValue("@Username", Email);
                    cmd.Parameters.AddWithValue("@Phonenum", PhoneNum);
                    cmd.Parameters.AddWithValue("@Email", Name);
                        cmd.Parameters.AddWithValue("@Password", Password);
                        
                        
                        

                    cmd.ExecuteNonQuery();
                        UserSession.UserEmail = Email;

                }
                }
            }

        public override bool ValidateInput()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(PhoneNum) || string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("Please fill in all fields");
                return false;
            }

            // Use a regular expression to validate the email format
            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            if (!emailRegex.IsMatch(Email))
            {
                MessageBox.Show("Invalid email format");
                return false;
            }

            if (Password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters");
                return false;
            }

            return true;
        }
    }
} 
