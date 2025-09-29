using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArisHotel
{
    public partial class MainWindow : Window
    {
        private enum DataType
        {
            Roles,
            Users,
            Rooms,
            Bookings
        }

        private DataType currentDataType = DataType.Roles;

        public MainWindow()
        {
            InitializeComponent();
            LoadUserInfo();
            LoadData();
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
            txtWelcome.Text = $"Добро пожаловать, {user.UserName}! Ваша роль: {role?.RoleName}";
        }

        private void BtnNavigation_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            // Reset button styles
            ResetButtonStyles();

            // Set current button style
            button.Background = new SolidColorBrush(Color.FromRgb(39, 174, 96)); // #FF27AE60 - зеленый для активной кнопки

            // Set current data type
            if (button == btnRoles)
                currentDataType = DataType.Roles;
            else if (button == btnUsers)
                currentDataType = DataType.Users;
            else if (button == btnRooms)
                currentDataType = DataType.Rooms;
            else if (button == btnBookings)
                currentDataType = DataType.Bookings;

            LoadData();
        }

        private void ResetButtonStyles()
        {
            btnRoles.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219)); // #FF3498DB
            btnUsers.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219)); // #FF3498DB
            btnRooms.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219)); // #FF3498DB
            btnBookings.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219)); // #FF3498DB
        }

        private void LoadData()
        {
            dataGrid.Columns.Clear();
            dataGrid.ItemsSource = null;

            switch (currentDataType)
            {
                case DataType.Roles:
                    LoadRoles();
                    break;
                case DataType.Users:
                    LoadUsers();
                    break;
                case DataType.Rooms:
                    LoadRooms();
                    break;
                case DataType.Bookings:
                    LoadBookings();
                    break;
            }
        }

        private void LoadRoles()
        {
            var roles = Session.context.Roles.ToList();
            dataGrid.ItemsSource = roles;

            // Add columns
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("RoleId"), Width = 50 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Название роли", Binding = new System.Windows.Data.Binding("RoleName"), Width = 200 });
            
            // Add action columns
            AddActionColumns();
        }

        private void LoadUsers()
        {
            var users = Session.context.Users.Include("Role").ToList();
            dataGrid.ItemsSource = users;

            // Add columns
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("UserId"), Width = 50 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Имя пользователя", Binding = new System.Windows.Data.Binding("UserName"), Width = 150 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Роль", Binding = new System.Windows.Data.Binding("Role.RoleName"), Width = 100 });
            
            // Add action columns
            AddActionColumns();
        }

        private void LoadRooms()
        {
            var rooms = Session.context.Rooms.ToList();
            dataGrid.ItemsSource = rooms;

            // Add columns
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("RoomId"), Width = 50 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Номер", Binding = new System.Windows.Data.Binding("RoomNumber"), Width = 100 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Вместимость", Binding = new System.Windows.Data.Binding("Capacity"), Width = 100 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Цена", Binding = new System.Windows.Data.Binding("Price"), Width = 100 });
            
            // Add action columns
            AddActionColumns();
        }

        private void LoadBookings()
        {
            var bookings = Session.context.Bookings.Include("Room").Include("User").ToList();
            dataGrid.ItemsSource = bookings;

            // Add columns
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("BookingId"), Width = 50 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Номер комнаты", Binding = new System.Windows.Data.Binding("Room.RoomNumber"), Width = 120 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Имя гостя", Binding = new System.Windows.Data.Binding("GuestName"), Width = 150 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата заезда", Binding = new System.Windows.Data.Binding("StartDate"), Width = 120 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата выезда", Binding = new System.Windows.Data.Binding("EndDate"), Width = 120 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Создал", Binding = new System.Windows.Data.Binding("User.UserName"), Width = 120 });
            
            // Add action columns
            AddActionColumns();
        }

        private void AddActionColumns()
        {
            // Edit column
            var editColumn = new DataGridTemplateColumn
            {
                Header = "Редактировать",
                Width = 120
            };
            var editTemplate = new DataTemplate();
            var editButton = new FrameworkElementFactory(typeof(Button));
            editButton.SetValue(Button.ContentProperty, "✏️ Редактировать");
            editButton.SetValue(Button.WidthProperty, 100.0);
            editButton.SetValue(Button.HeightProperty, 32.0);
            editButton.SetValue(Button.FontSizeProperty, 11.0);
            editButton.SetValue(Button.StyleProperty, dataGrid.FindResource("DataGridButtonStyle"));
            editButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(EditButton_Click));
            editTemplate.VisualTree = editButton;
            editColumn.CellTemplate = editTemplate;
            dataGrid.Columns.Add(editColumn);

            // Delete column
            var deleteColumn = new DataGridTemplateColumn
            {
                Header = "Удалить",
                Width = 120
            };
            var deleteTemplate = new DataTemplate();
            var deleteButton = new FrameworkElementFactory(typeof(Button));
            deleteButton.SetValue(Button.ContentProperty, "🗑️ Удалить");
            deleteButton.SetValue(Button.WidthProperty, 100.0);
            deleteButton.SetValue(Button.HeightProperty, 32.0);
            deleteButton.SetValue(Button.FontSizeProperty, 11.0);
            deleteButton.SetValue(Button.StyleProperty, dataGrid.FindResource("DeleteButtonStyle"));
            deleteButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(DeleteButton_Click));
            deleteTemplate.VisualTree = deleteButton;
            deleteColumn.CellTemplate = deleteTemplate;
            dataGrid.Columns.Add(deleteColumn);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext == null) return;

            switch (currentDataType)
            {
                case DataType.Roles:
                    var role = button.DataContext as Role;
                    if (role != null)
                    {
                        var editWindow = new RoleEditWindow(role);
                        if (editWindow.ShowDialog() == true)
                        {
                            LoadData();
                        }
                    }
                    break;
                case DataType.Users:
                    var user = button.DataContext as User;
                    if (user != null)
                    {
                        var editWindow = new UserEditWindow(user);
                        if (editWindow.ShowDialog() == true)
                        {
                            LoadData();
                        }
                    }
                    break;
                case DataType.Rooms:
                    var room = button.DataContext as Room;
                    if (room != null)
                    {
                        var editWindow = new RoomEditWindow(room);
                        if (editWindow.ShowDialog() == true)
                        {
                            LoadData();
                        }
                    }
                    break;
                case DataType.Bookings:
                    var booking = button.DataContext as Booking;
                    if (booking != null)
                    {
                        var editWindow = new BookingEditWindow(booking);
                        if (editWindow.ShowDialog() == true)
                        {
                            LoadData();
                        }
                    }
                    break;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext == null) return;

            var result = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    switch (currentDataType)
                    {
                        case DataType.Roles:
                            var role = button.DataContext as Role;
                            if (role != null)
                            {
                                Session.context.Roles.Remove(role);
                                Session.context.SaveChanges();
                            }
                            break;
                        case DataType.Users:
                            var user = button.DataContext as User;
                            if (user != null)
                            {
                                Session.context.Users.Remove(user);
                                Session.context.SaveChanges();
                            }
                            break;
                        case DataType.Rooms:
                            var room = button.DataContext as Room;
                            if (room != null)
                            {
                                Session.context.Rooms.Remove(room);
                                Session.context.SaveChanges();
                            }
                            break;
                        case DataType.Bookings:
                            var booking = button.DataContext as Booking;
                            if (booking != null)
                            {
                                Session.context.Bookings.Remove(booking);
                                Session.context.SaveChanges();
                            }
                            break;
                    }
                    LoadData();
                    MessageBox.Show("Запись успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            switch (currentDataType)
            {
                case DataType.Roles:
                    var roleWindow = new RoleEditWindow();
                    if (roleWindow.ShowDialog() == true)
                    {
                        LoadData();
                    }
                    break;
                case DataType.Users:
                    var userWindow = new UserEditWindow();
                    if (userWindow.ShowDialog() == true)
                    {
                        LoadData();
                    }
                    break;
                case DataType.Rooms:
                    var roomWindow = new RoomEditWindow();
                    if (roomWindow.ShowDialog() == true)
                    {
                        LoadData();
                    }
                    break;
                case DataType.Bookings:
                    var bookingWindow = new BookingEditWindow();
                    if (bookingWindow.ShowDialog() == true)
                    {
                        LoadData();
                    }
                    break;
            }
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
