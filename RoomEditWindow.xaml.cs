using System;
using System.Linq;
using System.Windows;

namespace ArisHotel
{
    public partial class RoomEditWindow : Window
    {
        private Room room;
        private bool isEdit;

        public RoomEditWindow()
        {
            InitializeComponent();
            isEdit = false;
            this.Title = "Добавление номера";
        }

        public RoomEditWindow(Room roomToEdit)
        {
            InitializeComponent();
            room = roomToEdit;
            isEdit = true;
            this.Title = "Редактирование номера";
            LoadData();
        }

        private void LoadData()
        {
            if (room != null)
            {
                txtRoomNumber.Text = room.RoomNumber;
                txtCapacity.Text = room.Capacity.ToString();
                txtPrice.Text = room.Price.ToString();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
            {
                MessageBox.Show("Пожалуйста, введите номер комнаты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtCapacity.Text, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректную вместимость (положительное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Пожалуйста, введите корректную цену (неотрицательное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (isEdit)
                {
                    // Редактирование существующего номера
                    room.RoomNumber = txtRoomNumber.Text.Trim();
                    room.Capacity = capacity;
                    room.Price = price;
                    Session.context.SaveChanges();
                    LogService.Instance.Info("Номер — обновление", $"Id: {room.RoomId}, Number: {room.RoomNumber}, Capacity: {room.Capacity}, Price: {room.Price}");
                    MessageBox.Show("Номер успешно обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Проверка на уникальность номера комнаты
                    var existingRoom = Session.context.Rooms.FirstOrDefault(r => r.RoomNumber == txtRoomNumber.Text.Trim());
                    if (existingRoom != null)
                    {
                        MessageBox.Show("Комната с таким номером уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        LogService.Instance.Warn("Номер — отказ добавления", $"Дубликат номера: {txtRoomNumber.Text.Trim()}");
                        return;
                    }

                    // Добавление нового номера
                    var newRoom = new Room
                    {
                        RoomNumber = txtRoomNumber.Text.Trim(),
                        Capacity = capacity,
                        Price = price
                    };
                    Session.context.Rooms.Add(newRoom);
                    Session.context.SaveChanges();
                    LogService.Instance.Info("Номер — добавление", $"Id: {newRoom.RoomId}, Number: {newRoom.RoomNumber}, Capacity: {newRoom.Capacity}, Price: {newRoom.Price}");
                    MessageBox.Show("Номер успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                LogService.Instance.Error("Номер — ошибка сохранения", ex.Message);
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
