using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace PostOffice
{
    /// <summary>
    /// Логика взаимодействия для AddClientWindow.xaml
    /// </summary>
    public partial class AddClientWindow : Window
    {
        public AddClientWindow()
        {
            InitializeComponent();
        }

        private void addclient_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            try
            {
                var newClient = new Pochta_Clients
                {
                    lastname = lastnametb.Text.Trim(),
                    name = nametb.Text.Trim(),
                    patronymic = string.IsNullOrWhiteSpace(patronymictb.Text) ? null : patronymictb.Text.Trim(),
                    address = adrestb.Text.Trim(),
                    phone = telefontb.Text.Trim()
                };

                App.con.Pochta_Clients.Add(newClient);
                App.con.SaveChanges();

                MessageBox.Show("Клиент успешно добавлен");

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void vixod_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(lastnametb.Text))
            {
                MessageBox.Show("Поле 'Фамилия' не может быть пустым");
                return false;
            }
            if (string.IsNullOrWhiteSpace(nametb.Text))
            {
                MessageBox.Show("Поле 'Имя' не может быть пустым");
                return false;
            }
            if (string.IsNullOrWhiteSpace(adrestb.Text))
            {
                MessageBox.Show("Поле 'Адрес' не может быть пустым");
                return false;
            }
            if (string.IsNullOrWhiteSpace(telefontb.Text))
            {
                MessageBox.Show("Поле 'Номер телефона' не может быть пустым");
                return false;
            }


            string fioPattern = @"^[А-Яа-яЁё]+$";

            if (!Regex.IsMatch(lastnametb.Text.Trim(), fioPattern))
            {
                MessageBox.Show("Фамилия должна содержать только русские буквы");
                return false;
            }

            if (!Regex.IsMatch(nametb.Text.Trim(), fioPattern))
            {
                MessageBox.Show("Имя должно содержать только русские буквы");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(patronymictb.Text))
            {
                if (!Regex.IsMatch(patronymictb.Text.Trim(), fioPattern))
                {
                    MessageBox.Show("Отчество должно содержать только русские буквы");
                    return false;
                }
            }


            string addressPattern = @"^[А-Яа-яЁё0-9\s,\-]+$";

            if (!Regex.IsMatch(adrestb.Text.Trim(), addressPattern))
            {
                MessageBox.Show("Адрес может содержать только русские буквы");
                return false;
            }

            string phonePattern = @"^[78]\d{10}$";
            string cleanPhone = telefontb.Text.Trim();

            if (!Regex.IsMatch(cleanPhone, phonePattern))
            {
                MessageBox.Show("Номер телефона должен состоять ровно из 11 цифр");
                return false;
            }

            bool isPhoneExists = App.con.Pochta_Clients.Any(c => c.phone == cleanPhone);
            if (isPhoneExists)
            {
                MessageBox.Show("Клиент с таким номером телефона уже зарегистрирован в базе данных");
                return false;
            }

            return true;
        }


        private void LetterOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[А-Яа-яЁё]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void LetterDigitOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[А-Яа-яЁё0-9\s,\-]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void DigitOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}