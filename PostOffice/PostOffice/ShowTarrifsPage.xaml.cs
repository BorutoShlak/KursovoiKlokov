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

namespace PostOffice
{
    /// <summary>
    /// Логика взаимодействия для ShowTarrifsPage.xaml
    /// </summary>
    public partial class ShowTarrifsPage : Page
    {
        private List<Pochta_Tarrifs> _allTarrifs;
        public ShowTarrifsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _allTarrifs = App.con.Pochta_Tarrifs.ToList();

                tarrifsdg.ItemsSource = _allTarrifs;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
