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
using System.Windows.Shapes;

namespace wasteNOT
{
    /// <summary>
    /// Interaction logic for SellerWindow.xaml
    /// </summary>
    public partial class SellerWindow : Window
    {
        public SellerWindow()
        {
            InitializeComponent();
        }

        private void homeBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();

            // Show the Login window
            mainWindow.Show();

            // Close the current MainWindow
            this.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnMyOrders_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new System.Uri("SellerPages/Orders.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnMyProducts_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new System.Uri("SellerPages/Products.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new System.Uri("SellerPages/AddProduct.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnStore_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new System.Uri("SellerPages/Store.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
