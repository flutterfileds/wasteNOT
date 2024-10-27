﻿using System;
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
using System.Windows.Shapes;

namespace wasteNOT
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
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
            this.Close();
        }
    }


}