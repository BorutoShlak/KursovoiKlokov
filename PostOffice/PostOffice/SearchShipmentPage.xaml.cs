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
    /// Interaction logic for SearchShipmentPage.xaml
    /// </summary>
    public partial class SearchShipmentPage : Page
    {
        private List<Pochta_Shipments> _allShipments;

        private List<Pochta_Statuses> _allStatuses;

        private Pochta_Shipments _selectedShipment;

        public SearchShipmentPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _allShipments = App.con.Pochta_Shipments.Include(u => u.Pochta_Clients).Include(u => u.Pochta_Clients1).Include(u => u.Pochta_Tarrifs).Include(u => u.Pochta_Employees).Include(u => u.Pochta_Statuses).ToList();

                _allStatuses = App.con.Pochta_Statuses.OrderBy(s => s.id).ToList();

                poiskdg.ItemsSource = _allShipments;
                statuscmb.ItemsSource = _allStatuses;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void poisk_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = poisk.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                poiskdg.ItemsSource = _allShipments;
                return;
            }

            var filtered = _allShipments.Where(s => (s.track_number != null && s.track_number.ToLower().Contains(searchText)) ||
                (s.Pochta_Clients != null && s.Pochta_Clients.FIO != null && s.Pochta_Clients.FIO.ToLower().Contains(searchText)) ||
                (s.Pochta_Clients != null && s.Pochta_Clients1.FIO != null && s.Pochta_Clients1.FIO.ToLower().Contains(searchText)) ||
                (s.Pochta_Clients != null && s.Pochta_Clients.phone != null && s.Pochta_Clients.phone.ToLower().Contains(searchText)) ||
                (s.Pochta_Clients != null && s.Pochta_Clients1.phone != null && s.Pochta_Clients1.phone.ToLower().Contains(searchText))).ToList();

            poiskdg.ItemsSource = filtered;
        }

        private void sbros_Click(object sender, RoutedEventArgs e)
        {
            poisk.Text = string.Empty;
            poiskdg.ItemsSource = _allShipments;
            poiskdg.SelectedItem = null;
        }

        private void statuscmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedShipment == null || statuscmb.SelectedValue == null) return;
            if (e.RemovedItems.Count == 0) return;

            int newStatusId = (int)statuscmb.SelectedValue;
            int currentStatusId = _selectedShipment.status;

            if (newStatusId < currentStatusId)
            {
                MessageBox.Show("Нельзя вернуть статус назад");
                statuscmb.SelectedValue = currentStatusId;
                return;
            }

            if (newStatusId == currentStatusId) return;

            var newStatus = _allStatuses.FirstOrDefault(s => s.id == newStatusId);
            if (newStatus == null) return;

            try
            {
                _selectedShipment.status = newStatusId;
                _selectedShipment.Pochta_Statuses = newStatus;

                var historyRecord = new Pochta_StatusHistory
                {
                    shipment = _selectedShipment.id,
                    status = newStatusId,
                    date_of_change = DateTime.Now
                };

                App.con.Pochta_StatusHistory.Add(historyRecord);
                App.con.SaveChanges();

                MessageBox.Show("Статус успешно изменен");

                poiskdg.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                statuscmb.SelectedValue = currentStatusId;
            }

        }

        private void poiskdg_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _selectedShipment = poiskdg.SelectedItem as Pochta_Shipments;

            if (_selectedShipment != null)
            {
                statuscmb.IsEnabled = true;
                statuscmb.SelectedValue = _selectedShipment.status;
            }
            else
            {
                statuscmb.IsEnabled = false;
                statuscmb.SelectedValue = null;
            }
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {

            if (!dateFrom.SelectedDate.HasValue || !dateTo.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите период для отчёта");
                return;
            }

            DateTime start = dateFrom.SelectedDate.Value.Date;
            DateTime end = dateTo.SelectedDate.Value.Date;

            if (start > end)
            {
                MessageBox.Show("Дата начала не может быть позже даты окончания");
                return;
            }

            DateTime endOfDay = end.AddDays(1).AddTicks(-1);

            var reportData = _allShipments.Where(s => s.date_priema >= start && s.date_priema <= endOfDay).ToList();

            int totalCount = reportData.Count;
            decimal totalCost = reportData.Sum(s => s.cost);

            string message = $"Период: с {start:dd.MM.yyyy} по {end:dd.MM.yyyy}\n\n" +
                             $"Всего отправлений: {totalCount}\n" +
                             $"Общая стоимость: {totalCost} руб.";

            MessageBox.Show(message, "Отчет об отправлениях");
        }
    }
}


