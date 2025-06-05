using Microsoft.EntityFrameworkCore;
using Patients.Data;
using Patients.Models;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Patients
{
    public partial class ReceptionistStarterWindow : Window
    {
        private bool isDoctorPanelOpen = false;
        private DateTime _currentWeekStart;
        private List<string> _timeSlots;
        private Border selectedCellBorder = null;
        private Doctor selectedDoctor;
        private DateTime? selectedAppointmentTime = null;
        private Appointment selectedExistingAppointment = null;
        private List<Patient> allPatients;
        private readonly AppDbContext _context = new AppDbContext();


        public ReceptionistStarterWindow()
        {
            InitializeComponent();
            _timeSlots = Enumerable.Range(0, 67)
                .Select(i => TimeSpan.FromMinutes(7 * 60 + i * 10).ToString(@"hh\:mm"))
                .ToList();

            _currentWeekStart = StartOfWeek(DateTime.Today);
            UpdateWeekLabel();
            BuildScheduleGrid();
            LoadDoctors();
            LoadPatients();
        }


        private void LoadDoctors()
        {
            using (var db = new AppDbContext())
            {
                var doctors = db.Doctors.ToList();
                DoctorListBox.ItemsSource = doctors;
                //DoctorListBox.DisplayMemberPath = "ShortFullName";
            }
        }


        private void LoadPatients()
        {
            using (var db = new AppDbContext())
            {
                allPatients = db.Patients.ToList();
                foreach (var p in allPatients)
                    p.FullName = $"{p.Surname} {p.Name} {p.Patronym}";

                PatientComboBox.ItemsSource = allPatients;
            }
        }


        private void DoctorsButton_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (Storyboard)FindResource(isDoctorPanelOpen ? "CollapsePanel" : "ExpandPanel");
            sb.Begin();
            isDoctorPanelOpen = !isDoctorPanelOpen;
        }

        private void DoctorListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedDoctor = DoctorListBox.SelectedItem as Doctor;
            if (selectedDoctor != null)
            {
                SelectedDoctorHeader.Text = $"Dr. {selectedDoctor.ShortFullName} — {selectedDoctor.Specialty}";
                ShowDoctorSchedule(selectedDoctor);
            }
        }


        private void UpdateWeekLabel()
        {
            var end = _currentWeekStart.AddDays(6);
            WeekLabel.Text = $"{_currentWeekStart:dd MMM} - {end:dd MMM}";
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDoctorSchedule(selectedDoctor);
        }

        private void PreviousWeek_Click(object sender, RoutedEventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(-7);
            UpdateWeekLabel();
            ShowDoctorSchedule(selectedDoctor);
        }

        private void NextWeek_Click(object sender, RoutedEventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(7);
            UpdateWeekLabel();
            ShowDoctorSchedule(selectedDoctor);
        }

        private DateTime StartOfWeek(DateTime dt)
        {
            int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }


        private void BuildScheduleGrid()
        {
            ScheduleGrid.Children.Clear();
            ScheduleGrid.RowDefinitions.Clear();

            ScheduleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            string[] days = { "Time", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            for (int col = 0; col < days.Length; col++)
            {
                var text = new TextBlock
                {
                    Text = days[col],
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(text, 0);
                Grid.SetColumn(text, col);
                ScheduleHeaderGrid.Children.Add(text);
            }

            for (int i = 0; i < _timeSlots.Count; i++)
            {
                ScheduleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });

                var timeText = new TextBlock
                {
                    Text = _timeSlots[i],
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(timeText, i + 1);
                Grid.SetColumn(timeText, 0);
                ScheduleGrid.Children.Add(timeText);

                for (int day = 1; day <= 7; day++)
                {
                    var cellBorder = new Border
                    {
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(0.5),
                        Background = Brushes.Transparent
                    };
                    cellBorder.MouseLeftButtonDown += CellBorder_MouseLeftButtonDown;
                    Grid.SetRow(cellBorder, i + 1);
                    Grid.SetColumn(cellBorder, day);
                    ScheduleGrid.Children.Add(cellBorder);
                }
            }
        }


        private void ShowDoctorSchedule(Doctor doctor)
        {
            if (doctor == null) return;

            BuildScheduleGrid();

            using (var context = new AppDbContext())
            {
                var appointments = context.Appointments
                    .Where(a => a.DoctorId == doctor.Id &&
                                a.Date >= _currentWeekStart &&
                                a.Date < _currentWeekStart.AddDays(7))
                    .Include(a => a.Patient)
                    .ToList();

                foreach (var appointment in appointments)
                {
                    int dayCol = (int)appointment.Date.DayOfWeek;
                    if (dayCol == 0) dayCol = 7;

                    int row = _timeSlots.IndexOf(appointment.Date.ToString("HH:mm")) + 1;
                    if (row < 1) continue;

                    var border = new Border
                    {
                        Background = Brushes.LightSkyBlue,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(1),
                        IsHitTestVisible = false // prevents click
                    };
                    var label = new TextBlock
                    {
                        Text = $"{appointment.Patient.Surname} {appointment.Patient.Name}",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    border.Child = label;

                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, dayCol);
                    ScheduleGrid.Children.Add(border);
                }
            }
        }


        private void CellBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedDoctor == null) return;

            Border clickedBorder = sender as Border;
            int row = Grid.GetRow(clickedBorder);
            int col = Grid.GetColumn(clickedBorder);
            if (row == 0 || col == 0) return;

            // Clear previous visual selection
            if (selectedCellBorder != null)
                selectedCellBorder.Background = Brushes.Transparent;
            selectedCellBorder = clickedBorder;

            // Determine selected date/time
            var timeStr = _timeSlots[row - 1];
            TimeSpan time = TimeSpan.Parse(timeStr);
            DateTime date = _currentWeekStart.AddDays(col - 1).Add(time);
            selectedAppointmentTime = date;

            using (var db = new AppDbContext())
            {
                var existing = db.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefault(a => a.DoctorId == selectedDoctor.Id && a.Date == selectedAppointmentTime);


                if (existing != null)
                {
                    // Show existing appointment
                    ExistingAppointmentPanel.Visibility = Visibility.Visible;
                    NewAppointmentPanel.Visibility = Visibility.Collapsed;
                    EmptyMessagePanel.Visibility = Visibility.Collapsed;

                    ExistingPatientTextBlock.Text = $"Patient: {existing.Patient.Surname} {existing.Patient.Name}";
                    selectedExistingAppointment = existing;

                    clickedBorder.Background = Brushes.LightSkyBlue;
                }
                else
                {
                    // Show new appointment form
                    ExistingAppointmentPanel.Visibility = Visibility.Collapsed;
                    NewAppointmentPanel.Visibility = Visibility.Visible;
                    EmptyMessagePanel.Visibility = Visibility.Collapsed;

                    NotesBox.Text = "";
                    selectedExistingAppointment = null;

                    clickedBorder.Background = Brushes.IndianRed;
                }
            }
        }


        private void SaveAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (PatientComboBox.SelectedItem is not Patient selectedPatient) return;

            using (var db = new AppDbContext())
            {
                var appt = new Appointment
                {
                    DoctorId = selectedDoctor.Id,
                    PatientId = selectedPatient.Id,
                    Date = selectedAppointmentTime.Value,
                    Notes = NotesBox.Text
                };
                db.Appointments.Add(appt);
                db.SaveChanges();
            }

        }


        private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (selectedExistingAppointment == null) return;

            using (var db = new AppDbContext())
            {
                var appt = db.Appointments.Find(selectedExistingAppointment.Id);
                if (appt != null)
                {
                    db.Appointments.Remove(appt);
                    db.SaveChanges();
                }
            }

            selectedExistingAppointment = null;

            ExistingAppointmentPanel.Visibility = Visibility.Collapsed;
            EmptyMessagePanel.Visibility = Visibility.Visible;

            ShowDoctorSchedule(selectedDoctor);
        }


        private void PatientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientComboBox.SelectedItem is Patient selectedPatient)
            {
                var age = DateTime.Today.Year - selectedPatient.DateOfBirth.Year;
                if (selectedPatient.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

                PatientDetailsTextBlock.Text =
                    $"Age: {age}\nDOB: {selectedPatient.DateOfBirth:yyyy-MM-dd}\nAddress: {selectedPatient.HomeAddress}";
            }
        }


        private void DoctorSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = DoctorSearchBox.Text.Trim().ToLower();

            var filteredDoctors = _context.Doctors
                .Where(d =>
                    d.Surname.ToLower().Contains(query) ||
                    d.Specialty.ToLower().Contains(query))
                .ToList();

            DoctorListBox.ItemsSource = filteredDoctors;
        }
    }
}
