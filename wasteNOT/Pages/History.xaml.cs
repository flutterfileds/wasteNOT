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

namespace wasteNOT
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : Page
    {
        public History()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Pages/Settings.xaml", UriKind.Relative));
        }

        private void textSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSearch.Focus();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text) && txtSearch.Text.Length > 0)
            {
                textSearch.Visibility = Visibility.Collapsed;
            }
            else
            {
                textSearch.Visibility = Visibility.Visible;
            }
        }
    }
}
