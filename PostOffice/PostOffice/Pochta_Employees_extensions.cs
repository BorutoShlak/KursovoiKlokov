using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PostOffice
{
    public partial class Pochta_Employees
    {

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case "lastname":
                        if (string.IsNullOrWhiteSpace(lastname))
                            error = "Фамилия обязательна";
                        else if (lastname.Length > 20)
                            error = "Максимум 20 символов";
                        else if (!Regex.IsMatch(lastname, @"^[А-Яа-яЁё]+$"))
                            error = "Только русские буквы";
                        break;

                    case "name":
                        if (string.IsNullOrWhiteSpace(name))
                            error = "Имя обязательно";
                        else if (name.Length > 20)
                            error = "Максимум 20 символов";
                        else if (!Regex.IsMatch(name, @"^[А-Яа-яЁё]+$"))
                            error = "Только русские буквы";
                        break;

                    case "patronymic":
                        if (!string.IsNullOrWhiteSpace(patronymic))
                        {
                            if (patronymic.Length > 20)
                                error = "Максимум 20 символов";
                            else if (!Regex.IsMatch(patronymic, @"^[А-Яа-яЁё]+$"))
                                error = "Только русские буквы";
                        }
                        break;

                    case "Pochta_Dolzhonst":
                        if (Pochta_Dolzhonst == null)
                            error = "Выберите должность из списка";
                        break;

                    case "phone":
                        if (string.IsNullOrWhiteSpace(phone))
                            error = "Телефон обязателен";
                        else if (!Regex.IsMatch(phone, @"^\d{11}$"))
                            error = "Ровно 11 цифр";
                        break;

                    case "UserLogin":
                        if (string.IsNullOrWhiteSpace(UserLogin))
                            error = "Логин обязателен";
                        else if (UserLogin.Length > 20)
                            error = "Максимум 20 символов";
                        else if (!Regex.IsMatch(UserLogin, @"^[a-zA-Z0-9]+$"))
                            error = "Только английские буквы и цифры";
                        break;

                    case "UserPassword":
                        if (string.IsNullOrWhiteSpace(UserPassword))
                            error = "Пароль обязателен";
                        else if (UserPassword.Length > 20)
                            error = "Максимум 20 символов";
                        else if (!Regex.IsMatch(UserPassword, @"^[a-zA-Z0-9]+$"))
                            error = "Только английские буквы и цифры";
                        break;
                }

                return error;
            }
        }


        public string UserLogin
        {
            get => Pochta_Users?.FirstOrDefault()?.login;
            set
            {
                if (Pochta_Users == null)
                {
                    Pochta_Users = new HashSet<Pochta_Users>();
                }

                var user = Pochta_Users?.FirstOrDefault();

                if (user == null && !string.IsNullOrEmpty(value))
                {
                    user = new Pochta_Users
                    {
                        role = 1
                    };
                    Pochta_Users.Add(user);
                }

                if (user != null)
                {
                    user.login = value;
                }
            }
        }

        public string UserPassword
        {
            get => Pochta_Users?.FirstOrDefault()?.password;
            set
            {
                if (Pochta_Users == null)
                {
                    Pochta_Users = new HashSet<Pochta_Users>();
                }

                var user = Pochta_Users?.FirstOrDefault();

                if (user == null && !string.IsNullOrEmpty(value))
                {
                    user = new Pochta_Users
                    {
                        role = 1
                    };
                    Pochta_Users.Add(user);
                }

                if (user != null)
                {
                    user.password = value;
                }
            }
        }
        public string FIO
        {
            get
            {
                return $"{lastname} {name} {patronymic}".Trim();
            }
        }
    }
}