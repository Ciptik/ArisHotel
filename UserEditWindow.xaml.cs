using System;
using System.Linq;
using System.Windows;

namespace ArisHotel
{
    public partial class UserEditWindow : Window
    {
        private User user;
        private bool isEdit;

        public UserEditWindow()
        {
            InitializeComponent();
            isEdit = false;
            this.Title = "Добавление пользователя";
            LoadRoles();
        }

        public UserEditWindow(User userToEdit)
        {
            InitializeComponent();
            user = userToEdit;
            isEdit = true;
            this.Title = "Редактирование пользователя";
            LoadRoles();
            LoadData();
        }

        private void LoadRoles()
        {
            cmbRole.ItemsSource = Session.context.Roles.ToList();
        }

        private void LoadData()
        {
            if (user != null)
            {
                txtUserName.Text = user.UserName;
                txtPassword.Text = user.Password;
                cmbRole.SelectedValue = user.RoleId;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                MessageBox.Show("Пожалуйста, введите имя пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Пожалуйста, введите пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbRole.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, выберите роль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (isEdit)
                {
                    // Редактирование существующего пользователя
                    user.UserName = txtUserName.Text.Trim();
                    user.Password = txtPassword.Text.Trim();
                    user.RoleId = (int)cmbRole.SelectedValue;
                    Session.context.SaveChanges();
                    MessageBox.Show("Пользователь успешно обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Проверка на уникальность имени пользователя
                    var existingUser = Session.context.Users.FirstOrDefault(u => u.UserName == txtUserName.Text.Trim());
                    if (existingUser != null)
                    {
                        MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Добавление нового пользователя
                    var newUser = new User
                    {
                        UserName = txtUserName.Text.Trim(),
                        Password = txtPassword.Text.Trim(),
                        RoleId = (int)cmbRole.SelectedValue
                    };
                    Session.context.Users.Add(newUser);
                    Session.context.SaveChanges();
                    MessageBox.Show("Пользователь успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
