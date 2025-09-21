using System.Linq;
using System.Windows;

namespace ArisHotel
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text?.Trim();
            string password = txtPassword.Password?.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите имя пользователя и пароль.", "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = Session.context.Users
                        .FirstOrDefault(u => u.UserName == username && u.Password == password);

            if (user == null)
            {
                MessageBox.Show("Неверное имя пользователя или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Устанавливаем текущего пользователя
            Session.currentUser = user;

            // Открываем главное окно
            var main = new MainWindow();
            main.Show();

            this.Close();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var reg = new RegisterWindow { Owner = this };
            reg.ShowDialog();
        }
    }
}
