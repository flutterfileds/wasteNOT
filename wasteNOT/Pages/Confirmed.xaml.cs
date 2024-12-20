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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wasteNOT.Pages
{
    /// <summary>
    /// Interaction logic for Confirmed.xaml
    /// </summary>
    public partial class Confirmed : Page
    {
        private int _orderId;

        // New constructor that accepts order ID
        public Confirmed(int orderId)
        {
            InitializeComponent();

            _orderId = orderId;

            // Set the order ID in the UI
            SetOrderDetails();
        }

        private void SetOrderDetails()
        {
            // Update the order ID text
            txtOrderId.Text = $"Thank You for Ordering! Your Order ID is #{_orderId}";
        }

        private void btnBack2h_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Pages/Home.xaml", UriKind.Relative));
        }

        private void btnViewOrder_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Pages/History.xaml", UriKind.Relative));
        }
    }
}
