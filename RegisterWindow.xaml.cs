using System.Linq;
using System.Windows;

namespace ArisHotel
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            LoadRoles();
        }

        private void LoadRoles()
        {
            // Подгружаем роли из БД
            var roles = Session.context.Roles.ToList();
            cmbRoles.ItemsSource = roles;
            if (roles.Any())
                cmbRoles.SelectedIndex = 0;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtRegUsername.Text?.Trim();
            string password = txtRegPassword.Password?.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || cmbRoles.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка существования пользователя
            bool exists = Session.context.Users.Any(u => u.UserName == username);
            if (exists)
            {
                MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создаём нового пользователя
            var selectedRole = (Role)cmbRoles.SelectedItem;
            var newUser = new User
            {
                UserName = username,
                Password = password,
                RoleId = selectedRole.RoleId
            };

            Session.context.Users.Add(newUser);
            Session.context.SaveChanges();

            MessageBox.Show("Пользователь успешно создан.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
