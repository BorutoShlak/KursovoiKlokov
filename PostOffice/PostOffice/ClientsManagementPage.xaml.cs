using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PostOffice
{
    /// <summary>
    /// Логика взаимодействия для ClientsManagementPage.xaml
    /// </summary>
    public partial class ClientsManagementPage : Page
    {
        public ClientsManagementPage()
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
                usersdg.ItemsSource = null;
                var clients = App.con.Pochta_Clients.ToList();
                usersdg.ItemsSource = clients;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void savechngs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                usersdg.CommitEdit();
                usersdg.CommitEdit(DataGridEditingUnit.Row, true);

                var clients = usersdg.ItemsSource.Cast<Pochta_Clients>().ToList();
                List<string> errors = new List<string>();

                for (int i = 0; i < clients.Count; i++)
                {
                    var c = clients[i];
                    int rowNum = i + 1;

                    if (c.id == 0 && string.IsNullOrWhiteSpace(c.lastname) && string.IsNullOrWhiteSpace(c.name))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(c.lastname))
                        errors.Add("Фамилия обязательна для заполнения");
                    else if (c.lastname.Length > 20)
                        errors.Add("Фамилия не более 20 символов");
                    else if (!Regex.IsMatch(c.lastname, @"^[А-Яа-яЁё]+$"))
                        errors.Add("Фамилия должна содержать только русские буквы");

                    if (string.IsNullOrWhiteSpace(c.name))
                        errors.Add("Имя обязательно для заполнения");
                    else if (c.name.Length > 20)
                        errors.Add("Имя не более 20 символов");
                    else if (!Regex.IsMatch(c.name, @"^[А-Яа-яЁё]+$"))
                        errors.Add("Имя должно содержать только русские буквы");

                    if (!string.IsNullOrWhiteSpace(c.patronymic))
                    {
                        if (c.patronymic.Length > 20)
                            errors.Add("Отчество не более 20 символов");
                        else if (!Regex.IsMatch(c.patronymic, @"^[А-Яа-яЁё]+$"))
                            errors.Add("Отчество должно содержать только русские буквы");
                    }

                    if (string.IsNullOrWhiteSpace(c.address))
                        errors.Add("Адрес обязателен для заполнения");
                    else if (c.address.Length > 50)
                        errors.Add("Адрес не более 50 символов");
                    else if (!Regex.IsMatch(c.address, @"^[А-Яа-яЁё0-9\s,\.\-]+$"))
                        errors.Add("Адрес содержит недопустимые символы");


                    if (string.IsNullOrWhiteSpace(c.phone))
                    {
                        errors.Add("Телефон обязателен для заполнения");
                    }
                    else if (!Regex.IsMatch(c.phone, @"^\d{11}$"))
                    {
                        errors.Add("Телефон должен состоять ровно из 11 цифр");
                    }
                    else
                    {
                        bool isPhoneUnique = !App.con.Pochta_Clients.Any(dbClient => dbClient.phone == c.phone && dbClient.id != c.id);

                        if (!isPhoneUnique)
                        {
                            errors.Add("Клиент с таким номером телефона уже зарегистрирован");
                        }
                    }
                }

                if (errors.Count > 0)
                {
                    MessageBox.Show("Обнаружены следующие ошибки заполнения:\n\n" + string.Join("\n", errors));
                    return;
                }

                foreach (var c in clients)
                {
                    if (c.id == 0)
                    {
                        App.con.Pochta_Clients.Add(c);
                    }
                }

                App.con.SaveChanges();

                MessageBox.Show("Данные клиентов успешно сохранены");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void deleteclient_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = usersdg.SelectedItem as Pochta_Clients;

            if (selectedClient == null)
            {
                MessageBox.Show("Выберите клиента в таблице для удаления");
                return;
            }

                try
                {
                    App.con.Pochta_Clients.Remove(selectedClient);
                    App.con.SaveChanges();

                    MessageBox.Show("Клиент успешно удалён");
                    LoadData();
                }
                catch
                {
                    MessageBox.Show("Не удалось удалить клиента, так как он уже указан как отправитель или получатель в существующих отправлениях");
                }
            }
        }
    }
