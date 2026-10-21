using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace PostOffice
{
    /// <summary>
    /// Логика взаимодействия для ShipmentPage.xaml
    /// </summary>
    public partial class ShipmentPage : Page
    {
        private static Random _rnd = new Random();

        private List<Pochta_Clients> _allClients;

        public ShipmentPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            usersinit();
        }

        private void tipship_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = tipship.SelectedIndex;
            bool isValuable = (index == 1 || index == 3 || index == 5);

            costship.IsEnabled = isValuable;

            if (!isValuable)
            {
                costship.Text = "";
                costship.MaxLength = 6;
            }
            else
            {
                if (index == 1)
                {
                    costship.MaxLength = 6;
                }
                else if (index == 3)
                {
                    costship.MaxLength = 6;
                }
                else if (index == 5)
                {
                    costship.MaxLength = 5;
                }
            }
        }

        private void usersinit()
        {
            _allClients = App.con.Pochta_Clients.ToList();

            sendercmb.ItemsSource = _allClients;
            recievercmb.ItemsSource = _allClients;
        }

        private void ClientComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                return;

            if (sender is ComboBox comboBox)
            {
                string searchText = comboBox.Text.ToLower().Trim();

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    comboBox.ItemsSource = _allClients;
                }
                else
                {
                    var filteredClients = _allClients.Where(c =>(c.lastname + " " + c.name + " " + c.patronymic + " " + c.phone).ToLower().Contains(searchText)).ToList();

                    comboBox.ItemsSource = filteredClients;
                }

                comboBox.IsDropDownOpen = true;
            }
        }

        private void createship_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!App.CurrentEmployeeId.HasValue)
                {
                    MessageBox.Show("Вы не авторизованы в системе");
                    return;
                }
                int currentEmployeeId = App.CurrentEmployeeId.Value;

                if (ValidateAndCalculate(out string tarifName, out int weight,
                    out decimal finalCost, out int senderId, out int receiverId,
                    out int tarifId))
                {
                    string trackNumber = generatetrack();
                    tracknumber.Text = trackNumber;

                    var newShipment = new Pochta_Shipments
                    {
                        track_number = trackNumber,
                        tip = tarifName,
                        fact_weight = weight.ToString(),
                        sender = senderId,
                        receiver = receiverId,
                        date_priema = DateTime.Today,
                        tarif = tarifId,
                        cost = finalCost,
                        status = 1,
                        employee = currentEmployeeId
                    };

                    App.con.Pochta_Shipments.Add(newShipment);
                    App.con.SaveChanges();

                    int shipmentId = newShipment.id;

                    var statusHistory = new Pochta_StatusHistory
                    {
                        shipment = shipmentId,
                        status = 1,
                        date_of_change = DateTime.Today
                    };

                    App.con.Pochta_StatusHistory.Add(statusHistory);
                    App.con.SaveChanges();

                    MessageBox.Show("Отправление успешно создано");

                    ClearShipmentForm();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool ValidateAndCalculate(out string tarifName, out int weight,
             out decimal finalCost, out int senderId, out int receiverId, out int tarifId)
        {
            tarifName = "";
            weight = 0;
            finalCost = 0;
            senderId = 0;
            receiverId = 0;
            tarifId = 0;
            var selectedItem = tipship.SelectedItem as System.Windows.Controls.ComboBoxItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Выберите тип отправления");
                return false;
            }

            string localTarifName = selectedItem.Content.ToString();
            tarifName = localTarifName;

            if (!int.TryParse(weightship.Text, out int localWeight) || localWeight <= 0)
            {
                MessageBox.Show("Введите корректный вес");
                return false;
            }
            weight = localWeight;

            if (sendercmb.SelectedValue == null || !int.TryParse(sendercmb.SelectedValue.ToString(), out int localSenderId))
            {
                MessageBox.Show("Выберите отправителя из списка");
                return false;
            }
            senderId = localSenderId;

            if (recievercmb.SelectedValue == null || !int.TryParse(recievercmb.SelectedValue.ToString(), out int localReceiverId))
            {
                MessageBox.Show("Выберите получателя из списка");
                return false;
            }
            receiverId = localReceiverId;

            try
            {
                var matchingTariff = App.con.Pochta_Tarrifs.Where(t => t.tip == localTarifName && localWeight <= t.weight).OrderBy(t => t.weight).FirstOrDefault();

                if (matchingTariff == null)
                {
                    MessageBox.Show($"Не найден тариф для типа '{localTarifName}' с весом {localWeight} грамм");
                    tariftb.Text = "";
                    return false;
                }

                tarifId = matchingTariff.id;
                decimal baseCost = Convert.ToDecimal(matchingTariff.cost);
                finalCost = baseCost;

                if (localTarifName.Contains("Ценное") || localTarifName.Contains("Ценная"))
                {
                    if (!decimal.TryParse(costship.Text, out decimal declaredValue) || declaredValue <= 0)
                    {
                        MessageBox.Show("Для ценного отправления необходимо указать объявленную ценность (число больше 0)");
                        return false;
                    }

                    decimal maxLimit = 0;
                    if (localTarifName == "Ценное письмо") maxLimit = 500000;
                    else if (localTarifName == "Ценная посылка") maxLimit = 100000;
                    else if (localTarifName == "Ценная бандероль") maxLimit = 10000;

                    if (declaredValue > maxLimit)
                    {
                        MessageBox.Show($"Максимальная объявленная ценность для {localTarifName} составляет {maxLimit} руб.");
                        return false;
                    }

                    decimal nacenka = declaredValue * 0.05m;
                    finalCost = baseCost + nacenka;
                }

                tariftb.Text = $"{matchingTariff.name} - {finalCost:F2} руб.";
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }



        private string generatetrack()
        {
            string trackNumber;
            bool isUnique = false;

            do
            {
                var tn = new StringBuilder(14);
                for (int i = 0; i < 14; i++)
                    tn.Append(_rnd.Next(0, 10));

                trackNumber = tn.ToString();

                isUnique = !App.con.Pochta_Shipments.Any(s => s.track_number == trackNumber);
            }
            while (!isUnique);

            return trackNumber;
        }

        private void adds_Click(object sender, RoutedEventArgs e)
        {
            OpenAddClientWindow();
        }

        private void OpenAddClientWindow()
        {
            AddClientWindow addClientWindow = new AddClientWindow();

            bool? result = addClientWindow.ShowDialog();

            if (result == true)
            {
                RefreshClientsList();
            }
        }

        private void RefreshClientsList()
        {
            _allClients = App.con.Pochta_Clients.ToList();

            sendercmb.ItemsSource = null;
            recievercmb.ItemsSource = null;

            sendercmb.ItemsSource = _allClients;
            recievercmb.ItemsSource = _allClients;
        }

        private void DigitOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void ClearShipmentForm()
        {
            tipship.SelectedIndex = -1;
            weightship.Clear();
            costship.Clear();
            costship.IsEnabled = false;
            sendercmb.SelectedIndex = -1;
            sendercmb.Text = "";
            recievercmb.SelectedIndex = -1;
            recievercmb.Text = "";
            tariftb.Clear();
            tracknumber.Clear();
        }

    }
}
