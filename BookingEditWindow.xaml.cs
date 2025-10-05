using System;
using System.Linq;
using System.Windows;

namespace ArisHotel
{
    public partial class BookingEditWindow : Window
    {
        private Booking booking;
        private bool isEdit;

        public BookingEditWindow()
        {
            InitializeComponent();
            isEdit = false;
            this.Title = "Добавление бронирования";
            LoadRooms();
        }

        public BookingEditWindow(Booking bookingToEdit)
        {
            InitializeComponent();
            booking = bookingToEdit;
            isEdit = true;
            this.Title = "Редактирование бронирования";
            LoadRooms();
            LoadData();
        }

        private void LoadRooms()
        {
            cmbRoom.ItemsSource = Session.context.Rooms.ToList();
        }

        private void LoadData()
        {
            if (booking != null)
            {
                cmbRoom.SelectedValue = booking.RoomId;
                txtGuestName.Text = booking.GuestName;
                dpStartDate.SelectedDate = booking.StartDate;
                dpEndDate.SelectedDate = booking.EndDate;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbRoom.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, выберите комнату.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtGuestName.Text))
            {
                MessageBox.Show("Пожалуйста, введите имя гостя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpStartDate.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите дату заезда.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите дату выезда.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpStartDate.SelectedDate >= dpEndDate.SelectedDate)
            {
                MessageBox.Show("Дата выезда должна быть позже даты заезда.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (isEdit)
                {
                    // Редактирование существующего бронирования
                    booking.RoomId = (int)cmbRoom.SelectedValue;
                    booking.GuestName = txtGuestName.Text.Trim();
                    booking.StartDate = dpStartDate.SelectedDate.Value;
                    booking.EndDate = dpEndDate.SelectedDate.Value;
                    Session.context.SaveChanges();
                    LogService.Instance.Info("Бронирование — обновление", $"Id: {booking.BookingId}, RoomId: {booking.RoomId}, Guest: {booking.GuestName}, {booking.StartDate:yyyy-MM-dd}..{booking.EndDate:yyyy-MM-dd}");
                    MessageBox.Show("Бронирование успешно обновлено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Добавление нового бронирования
                    var newBooking = new Booking
                    {
                        RoomId = (int)cmbRoom.SelectedValue,
                        CreatedByUserId = Session.currentUser.UserId,
                        GuestName = txtGuestName.Text.Trim(),
                        StartDate = dpStartDate.SelectedDate.Value,
                        EndDate = dpEndDate.SelectedDate.Value
                    };
                    Session.context.Bookings.Add(newBooking);
                    Session.context.SaveChanges();
                    LogService.Instance.Info("Бронирование — добавление", $"Id: {newBooking.BookingId}, RoomId: {newBooking.RoomId}, Guest: {newBooking.GuestName}, {newBooking.StartDate:yyyy-MM-dd}..{newBooking.EndDate:yyyy-MM-dd}");
                    MessageBox.Show("Бронирование успешно добавлено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                LogService.Instance.Error("Бронирование — ошибка сохранения", ex.Message);
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
