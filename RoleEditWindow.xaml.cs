using System;
using System.Windows;

namespace ArisHotel
{
    public partial class RoleEditWindow : Window
    {
        private Role role;
        private bool isEdit;

        public RoleEditWindow()
        {
            InitializeComponent();
            isEdit = false;
            this.Title = "Добавление роли";
        }

        public RoleEditWindow(Role roleToEdit)
        {
            InitializeComponent();
            role = roleToEdit;
            isEdit = true;
            this.Title = "Редактирование роли";
            LoadData();
        }

        private void LoadData()
        {
            if (role != null)
            {
                txtRoleName.Text = role.RoleName;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoleName.Text))
            {
                MessageBox.Show("Пожалуйста, введите название роли.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (isEdit)
                {
                    // Редактирование существующей роли
                    role.RoleName = txtRoleName.Text.Trim();
                    Session.context.SaveChanges();
                    LogService.Instance.Info("Роль — обновление", $"Id: {role.RoleId}, Name: {role.RoleName}");
                    MessageBox.Show("Роль успешно обновлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Добавление новой роли
                    var newRole = new Role
                    {
                        RoleName = txtRoleName.Text.Trim()
                    };
                    Session.context.Roles.Add(newRole);
                    Session.context.SaveChanges();
                    LogService.Instance.Info("Роль — добавление", $"Id: {newRole.RoleId}, Name: {newRole.RoleName}");
                    MessageBox.Show("Роль успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                LogService.Instance.Error("Роль — ошибка сохранения", ex.Message);
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
