using System.Linq;
using System.Windows;

namespace ArisHotel
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            var user = Session.currentUser;
            if (user == null)
            {
                txtWelcome.Text = "Пользователь не найден.";
                return;
            }

            var role = Session.context.Roles.Find(user.RoleId);
            txtWelcome.Text = $"Добро пожаловать, {user.UserName}!\nВаша роль: {role?.RoleName}";
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Session.currentUser = null; // обнуляем текущего пользователя
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
