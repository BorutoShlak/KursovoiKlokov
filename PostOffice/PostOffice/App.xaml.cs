using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PostOffice
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static user7_dbEntities1 con = new user7_dbEntities1();

        public static int? CurrentEmployeeId { get; set; }
    }
}
