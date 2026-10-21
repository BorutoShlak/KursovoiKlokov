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
using System.Data;
using System.Data.Entity;
using BCrypt;

namespace PostOffice
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void vxod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = App.con.Pochta_Users.Include(r => r.Pochta_Role).Include(u => u.Pochta_Employees).SingleOrDefault(u => u.login == log.Text.Trim() && u.password == pas.Password.Trim());

                if (user != null)
                {
                    var roleName = user.Pochta_Role.name;

                    App.CurrentEmployeeId = user.employee;

                    switch (user.role)
                    {
                        case 1:
                            MessageBox.Show($"Авторизация успешна. Ваша роль {roleName}");
                            EmployeeWindow employeeWindow = new EmployeeWindow();
                            employeeWindow.Show();
                            this.Close();
                            break;
                        case 2:
                            MessageBox.Show($"Авторизация успешна. Ваша роль {roleName}");
                            AdminWindow adminWindow = new AdminWindow();
                            adminWindow.Show();
                            this.Close();
                            break;
                        default:
                            MessageBox.Show("Неизвестная роль");
                            break;
                    }
                }

                else
                {
                    MessageBox.Show("Неправильный логин или пароль");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
