using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace PostOffice
{
    public partial class EmployeesManagementPage : Page
    {
        public EmployeesManagementPage()
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
                employeesdg.ItemsSource = null;
                var employees = App.con.Pochta_Employees.Include(u => u.Pochta_Dolzhonst).Include(u => u.Pochta_Users).ToList();

                employeesdg.ItemsSource = employees;

                var employeeColumn = employeesdg.Columns
                    .FirstOrDefault(c => c.Header.ToString() == "Должность") as DataGridComboBoxColumn;

                if (employeeColumn != null)
                {
                    employeeColumn.ItemsSource = App.con.Pochta_Dolzhonst.Where(d => d.id != 3).ToList();
                }
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
                employeesdg.CommitEdit();
                employeesdg.CommitEdit(DataGridEditingUnit.Row, true);

                var allEmployees = employeesdg.ItemsSource.Cast<Pochta_Employees>().ToList();
                List<string> errors = new List<string>();

                for (int i = 0; i < allEmployees.Count; i++)
                {
                    var emp = allEmployees[i];
                    int rowNum = i + 1;

                    if (emp.Pochta_Dolzhonst != null && emp.Pochta_Dolzhonst.id == 3)
                    {
                        if (emp.id == 0)
                        {
                            errors.Add("Нельзя создать нового сотрудника с должностью 'Бывший сотрудник'");
                        }
                        continue;
                    }

                    if (emp["lastname"] != null) errors.Add($"Строка {rowNum}: {emp["lastname"]}");
                    if (emp["name"] != null) errors.Add($"Строка {rowNum}: {emp["name"]}");
                    if (emp["patronymic"] != null) errors.Add($"Строка {rowNum}: {emp["patronymic"]}");
                    if (emp["Pochta_Dolzhonst"] != null) errors.Add($"Строка {rowNum}: {emp["Pochta_Dolzhonst"]}");
                    if (emp["phone"] != null) errors.Add($"Строка {rowNum}: {emp["phone"]}");
                    if (emp["UserLogin"] != null) errors.Add($"Строка {rowNum}: {emp["UserLogin"]}");
                    if (emp["UserPassword"] != null) errors.Add($"Строка {rowNum}: {emp["UserPassword"]}");

                    if (!string.IsNullOrWhiteSpace(emp.phone))
                    {
                        bool isPhoneUnique = !App.con.Pochta_Employees.Any(u => u.phone == emp.phone && u.id != emp.id);
                        if (!isPhoneUnique)
                        {
                            errors.Add("Сотрудник с таким номером телефона уже существует");
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(emp.UserLogin))
                    {
                        bool isLoginUnique = !App.con.Pochta_Users.Any(u => u.login == emp.UserLogin && u.employee != emp.id);
                        if (!isLoginUnique)
                        {
                            errors.Add("Логин уже занят другим сотрудником");
                        }
                    }
                }

                if (errors.Count > 0)
                {
                    MessageBox.Show("Обнаружены ошибки заполнения:\n\n" + string.Join("\n", errors));
                    return;
                }

                foreach (var emp in allEmployees)
                {
                    if (emp.Pochta_Dolzhonst != null && emp.Pochta_Dolzhonst.id == 3) continue;


                    int targetRole = (emp.Pochta_Dolzhonst != null && emp.Pochta_Dolzhonst.id == 1) ? 2 : 1;

                    if (emp.id == 0) 
                    {
                        if (!string.IsNullOrWhiteSpace(emp.UserLogin) || !string.IsNullOrWhiteSpace(emp.UserPassword))
                        {
                            if (emp.Pochta_Users == null)
                            {
                                emp.Pochta_Users = new HashSet<Pochta_Users>();
                            }

                            if (!emp.Pochta_Users.Any())
                            {
                                emp.Pochta_Users.Add(new Pochta_Users
                                {
                                    login = emp.UserLogin,
                                    password = emp.UserPassword,
                                    role = targetRole
                                });
                            }
                        }
                        App.con.Pochta_Employees.Add(emp);
                    }
                    else
                    {
                        var user = emp.Pochta_Users?.FirstOrDefault();

                        if (user != null)
                        {
                            user.login = emp.UserLogin;
                            user.password = emp.UserPassword;
                            user.role = targetRole;
                        }
                        else if (!string.IsNullOrWhiteSpace(emp.UserLogin) || !string.IsNullOrWhiteSpace(emp.UserPassword))
                        {
                            if (emp.Pochta_Users == null) emp.Pochta_Users = new HashSet<Pochta_Users>();

                            emp.Pochta_Users.Add(new Pochta_Users
                            {
                                login = emp.UserLogin,
                                password = emp.UserPassword,
                                role = targetRole
                            });
                        }
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

        private void deleteemployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedEmployee = employeesdg.SelectedItem as Pochta_Employees;

                if (selectedEmployee == null)
                {
                    MessageBox.Show("Выберите сотрудника для увольнения");
                    return;
                }

                var user = selectedEmployee.Pochta_Users?.FirstOrDefault();
                if (user != null && user.role == 3)
                {
                    MessageBox.Show("Этот сотрудник уже уволен");
                    return;
                }

                    if (user != null)
                    {
                        user.login = null;
                        user.password = null;
                        user.role = 3;
                    }

                    selectedEmployee.dolzhnost = 3;

                    selectedEmployee.Pochta_Dolzhonst = App.con.Pochta_Dolzhonst.Find(3);

                    App.con.SaveChanges();

                    MessageBox.Show("Сотрудник успешно уволен");
                    LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
