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

namespace PostOffice
{
    /// <summary>
    /// Логика взаимодействия для EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        public EmployeeWindow()
        {
            InitializeComponent();
        }

        private void exitform_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void createshipment_Click(object sender, RoutedEventArgs e)
        {
            ShipmentPage shipmentPage = new ShipmentPage();
            MainFrame.Navigate(shipmentPage);
        }

        private void searchshipment_Click(object sender, RoutedEventArgs e)
        {
            SearchShipmentPage searchShipmentPage = new SearchShipmentPage();
            MainFrame.Navigate(searchShipmentPage);
        }

        private void tarifes_Click(object sender, RoutedEventArgs e)
        {
            ShowTarrifsPage showTarrifsPage = new ShowTarrifsPage();
            MainFrame.Navigate(showTarrifsPage);
        }
    }
}
