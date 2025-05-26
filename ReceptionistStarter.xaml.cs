using System.Windows;
using System.Windows.Media.Animation;
using System.Linq;
using Patients.Data;
using Patients.Models;
using System.Windows.Controls;
using System.Windows.Media;


namespace Patients
{
    public partial class ReceptionistStarterWindow : Window
    {
        private bool isDoctorPanelOpen = false;

        public ReceptionistStarterWindow()
        {
            InitializeComponent();
            LoadDoctors();
        }

        private void LoadDoctors()
        {
            using (var db = new AppDbContext())
            {
                var doctors = db.Doctors.ToList();
                DoctorListBox.ItemsSource = doctors;
                DoctorListBox.DisplayMemberPath = "Name";
            }
        }

        private void DoctorsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isDoctorPanelOpen)
            {
                Storyboard expand = (Storyboard)FindResource("ExpandPanel");
                expand.Begin();
                isDoctorPanelOpen = true;
            }
            else
            {
                Storyboard collapse = (Storyboard)FindResource("CollapsePanel");
                collapse.Begin();
                isDoctorPanelOpen = false;
            }
        }

        private void DoctorListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDoctor = DoctorListBox.SelectedItem as Doctor;
            if (selectedDoctor != null)
            {
                SelectedDoctorHeader.Text = $"Dr. {selectedDoctor.Name} — {selectedDoctor.Specialty}";
                ShowDoctorSchedule(selectedDoctor);
            }
        }

        private void ShowDoctorSchedule(Doctor doctor)
        {
            if (doctor == null) return;

            BuildScheduleGrid();

            // Load from DB
            using (var context = new AppDbContext())
            {
                var appointments = context.Appointments
                    .Where(a => a.DoctorId == doctor.Id && a.Date >= _currentWeekStart && a.Date < _currentWeekStart.AddDays(7))
                    .ToList();

                foreach (var appointment in appointments)
                {
                    int dayCol = (int)appointment.Date.DayOfWeek;
                    if (dayCol == 0) dayCol = 7; // Sunday to column 7

                    int row = _timeSlots.IndexOf(appointment.Date.ToString("HH:mm")) + 1;
                    if (row < 1) continue;

                    var border = new Border
                    {
                        Background = Brushes.LightSkyBlue,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(1)
                    };
                    var label = new TextBlock
                    {
                        Text = appointment.PatientName,
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

        private DateTime _currentWeekStart = DateTime.Today;
        private List<string> _timeSlots = Enumerable.Range(0, 67)
            .Select(i => TimeSpan.FromMinutes(7 * 60 + i * 10).ToString(@"hh\:mm"))
            .ToList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _currentWeekStart = StartOfWeek(DateTime.Today);
            UpdateWeekLabel();
            BuildScheduleGrid();
        }

        private void UpdateWeekLabel()
        {
            var end = _currentWeekStart.AddDays(6);
            WeekLabel.Text = $"{_currentWeekStart:dd MMM} - {end:dd MMM}";
        }

        private void PreviousWeek_Click(object sender, RoutedEventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(-7);
            UpdateWeekLabel();
            ShowDoctorSchedule(DoctorListBox.SelectedItem as Doctor);
        }

        private void NextWeek_Click(object sender, RoutedEventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(7);
            UpdateWeekLabel();
            ShowDoctorSchedule(DoctorListBox.SelectedItem as Doctor);
        }

        private static DateTime StartOfWeek(DateTime dt)
        {
            int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        private void BuildScheduleGrid()
        {
            // Clear everything except headers
            ScheduleGrid.RowDefinitions.Clear();
            ScheduleGrid.Children.Clear();

            // Header row
            ScheduleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Day headers
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
                ScheduleGrid.Children.Add(text);
            }

            // Time slots
            for (int i = 0; i < _timeSlots.Count; i++)
            {
                ScheduleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });

                // Time label
                var timeText = new TextBlock
                {
                    Text = _timeSlots[i],
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(timeText, i + 1);
                Grid.SetColumn(timeText, 0);
                ScheduleGrid.Children.Add(timeText);
            }
        }
    }

}
