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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }

        private void changetariffs_Click(object sender, RoutedEventArgs e)
        {
            TariffsManagementPage tariffsManagementPage = new TariffsManagementPage();
            MainFrame.Navigate(tariffsManagementPage);
        }

        private void vixod_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void changeemployees_Click(object sender, RoutedEventArgs e)
        {
            EmployeesManagementPage employeesManagementPage = new EmployeesManagementPage();
            MainFrame.Navigate(employeesManagementPage);
        }

        private void changeclients_Click(object sender, RoutedEventArgs e)
        {
            ClientsManagementPage clientsManagementPage = new ClientsManagementPage();
            MainFrame.Navigate(clientsManagementPage);
        }
    }
}
