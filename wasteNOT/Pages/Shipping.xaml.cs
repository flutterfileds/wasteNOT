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
    /// Interaction logic for Shipping.xaml
    /// </summary>
    public partial class Shipping : Page
    {
        public Shipping()
        {
            InitializeComponent();
        }

        private void btnOKShipping_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Pages/Confirmed.xaml", UriKind.Relative));
        }
    }
}
