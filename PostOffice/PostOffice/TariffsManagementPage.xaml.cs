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
    /// Логика взаимодействия для TariffsManagementPage.xaml
    /// </summary>
    public partial class TariffsManagementPage : Page
    {
        public TariffsManagementPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                tariffsdg.ItemsSource = null;
                var tariffs = App.con.Pochta_Tarrifs.ToList();

                tariffsdg.ItemsSource = tariffs;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void savechngs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tariffsdg.CommitEdit();
                tariffsdg.CommitEdit(DataGridEditingUnit.Row, true);

                var tariffs = tariffsdg.ItemsSource.Cast<Pochta_Tarrifs>().ToList();
                List<string> errors = new List<string>();

                for (int i = 0; i < tariffs.Count; i++)
                {
                    var t = tariffs[i];
                    int rowNum = i + 1;

                    if (string.IsNullOrWhiteSpace(t.tip))
                        errors.Add("Тип отправления обязателен");
                    else if (t.tip.Length > 25)
                        errors.Add("Тип отправления максимум 25 символов.");
                    else if (!Regex.IsMatch(t.tip, @"^[А-Яа-яЁё\s]+$"))
                        errors.Add("Тип отправления - только русские буквы");

                    string wStr = t.weight.ToString();
                    if (string.IsNullOrWhiteSpace(wStr))
                        errors.Add("Вес обязателен.");
                    else if (!Regex.IsMatch(wStr, @"^\d{1,5}$"))
                        errors.Add($"Вес - максимум пятизначное число");

                    if (t.cost <= 0)
                        errors.Add("Стоимость должна быть больше 0");
                    else if (t.cost > 9999.99m)
                        errors.Add("Стоимость не может превышать 9999.99");

                    if (string.IsNullOrWhiteSpace(t.name))
                        errors.Add("Название обязательно");
                    else if (t.name.Length > 35)
                        errors.Add("Название макс. 35 символов");
                    else if (!Regex.IsMatch(t.name, @"^[А-Яа-яЁё\s]+$"))
                        errors.Add("Название - только русские буквы");
                }

                if (errors.Count > 0)
                {
                    string fullErrorMessage = "Обнаружены следующие ошибки заполнения:\n\n" + string.Join("\n", errors);

                    MessageBox.Show(fullErrorMessage);
                    return;
                }

                foreach (var t in tariffs)
                {
                    if (t.id == 0)
                    {
                        App.con.Pochta_Tarrifs.Add(t);
                    }
                }

                App.con.SaveChanges();

                MessageBox.Show("Данные успешно сохранены");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void deletetarrif_Click(object sender, RoutedEventArgs e)
        {
            var selectedTariff = tariffsdg.SelectedItem as Pochta_Tarrifs;

            if (selectedTariff == null)
            {
                MessageBox.Show("Выберите тариф в таблице для удаления");
                return;
            }

            {
                try
                {
                    App.con.Pochta_Tarrifs.Remove(selectedTariff);
                    App.con.SaveChanges();

                    MessageBox.Show("Тариф успешно удален");
                    LoadData();
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось удалить тариф, так как он используется в существующих отправлениях");
                }
            }
        }

    }
}

