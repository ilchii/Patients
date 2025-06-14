using Microsoft.EntityFrameworkCore;
using Patients.Data;
using Patients.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Patients
{
    public partial class DoctorStarterWindow : Window
    {
        private readonly AppDbContext _context = new AppDbContext();
        private List<Appointment> _todayAppointments;


        public DoctorStarterWindow()
        {
            InitializeComponent();
            LoadTodaysAppointments();
        }

        private void LoadTodaysAppointments()
        {
            AppointmentsPanel.Children.Clear();

            using (var context = new AppDbContext())
            {
                DateTime today = DateTime.Today;
                var appointments = context.Appointments
                    .Include(a => a.Patient)
                    .Where(a => a.DoctorId == 1 &&
                                a.Date.Date == today)
                                //&& !a.IsCompleted) // Assuming this flag marks processed appointments
                    .OrderBy(a => a.Date)
                    .ToList();

                foreach (var appt in appointments)
                {
                    var patient = appt.Patient;
                    var stack = new StackPanel { Margin = new Thickness(0, 0, 0, 20) };

                    stack.Children.Add(new TextBlock
                    {
                        Text = $"{patient.Surname} {patient.Name} {patient.Patronym}",
                        FontWeight = FontWeights.Bold,
                        FontSize = 16
                    });

                    stack.Children.Add(new TextBlock
                    {
                        Text = $"ID: {patient.Id}",
                        FontStyle = FontStyles.Italic
                    });

                    stack.Children.Add(new TextBlock
                    {
                        Text = $"{appt.Date:HH:mm} - {appt.Date.AddMinutes(appt.DurationMinutes):HH:mm}",
                        Margin = new Thickness(0, 5, 0, 0)
                    });

                    int age = CalculateAge(patient.DateOfBirth);
                    stack.Children.Add(new TextBlock { Text = $"Age: {age}" });
                    stack.Children.Add(new TextBlock { Text = $"Address: {patient.HomeAddress}" });

                    var buttonsPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(0, 10, 0, 0)
                    };

                    var startButton = new Button
                    {
                        Content = "Start Appointment",
                        Margin = new Thickness(5, 0, 0, 0)
                    };
                    startButton.Click += (s, e) => StartAppointment(appt);

                    var cardButton = new Button
                    {
                        Content = "Medical Card",
                        Margin = new Thickness(5, 0, 0, 0)
                    };
                    cardButton.Click += (s, e) => OpenMedicalCard(patient);

                    buttonsPanel.Children.Add(startButton);
                    buttonsPanel.Children.Add(cardButton);

                    stack.Children.Add(buttonsPanel);

                    var border = new Border
                    {
                        BorderBrush = Brushes.DarkGray,
                        BorderThickness = new Thickness(1),
                        Padding = new Thickness(10),
                        Margin = new Thickness(0, 0, 0, 10),
                        CornerRadius = new CornerRadius(8),
                        Background = Brushes.WhiteSmoke,
                        Child = stack
                    };

                    AppointmentsPanel.Children.Add(border);

                    _todayAppointments = context.Appointments
                        .Include(a => a.Patient)
                        .Where(a => a.DoctorId == 1 && a.Date.Date == today)
                        .OrderBy(a => a.Date)
                        .ToList();

                    DisplayAppointments(_todayAppointments);
                }
            }
        }

        private void DisplayAppointments(List<Appointment> appointments)
        {
            AppointmentsPanel.Children.Clear();

            foreach (var appt in appointments)
            {
                var patient = appt.Patient;
                var stack = new StackPanel { Margin = new Thickness(0, 0, 0, 20) };

                stack.Children.Add(new TextBlock
                {
                    Text = $"{patient.Surname} {patient.Name} {patient.Patronym}",
                    FontWeight = FontWeights.Bold,
                    FontSize = 16
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"ID: {patient.Id}",
                    FontStyle = FontStyles.Italic
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"{appt.Date:HH:mm} - {appt.Date.AddMinutes(appt.DurationMinutes):HH:mm}",
                    Margin = new Thickness(0, 5, 0, 0)
                });

                int age = CalculateAge(patient.DateOfBirth);
                stack.Children.Add(new TextBlock { Text = $"Age: {age}" });
                stack.Children.Add(new TextBlock { Text = $"Address: {patient.HomeAddress}" });

                var buttonsPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                var startButton = new Button
                {
                    Content = "Start Appointment",
                    Margin = new Thickness(5, 0, 0, 0)
                };
                startButton.Click += (s, e) => StartAppointment(appt);

                var cardButton = new Button
                {
                    Content = "Medical Card",
                    Margin = new Thickness(5, 0, 0, 0)
                };
                cardButton.Click += (s, e) => OpenMedicalCard(patient);

                buttonsPanel.Children.Add(startButton);
                buttonsPanel.Children.Add(cardButton);

                stack.Children.Add(buttonsPanel);

                var border = new Border
                {
                    BorderBrush = Brushes.DarkGray,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 10),
                    CornerRadius = new CornerRadius(8),
                    Background = Brushes.WhiteSmoke,
                    Child = stack
                };

                AppointmentsPanel.Children.Add(border);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = SearchBox.Text.Trim().ToLower();

            var filtered = _todayAppointments
                .Where(a =>
                    a.Patient.Id.ToString().Contains(keyword) ||
                    $"{a.Patient.Surname} {a.Patient.Name} {a.Patient.Patronym}".ToLower().Contains(keyword)
                )
                .ToList();

            DisplayAppointments(filtered);
        }


        private bool isDoctorExpanded = false;

        private void DoctorButton_Click(object sender, RoutedEventArgs e)
        {
            isDoctorExpanded = !isDoctorExpanded;
            Sidebar.Width = isDoctorExpanded ? 200 : 60;
            DoctorSubMenu.Visibility = isDoctorExpanded ? Visibility.Visible : Visibility.Collapsed;
        }

        private void WorkingSpace_Click(object sender, RoutedEventArgs e)
        {
            // You’re already here, optionally refresh
        }

        private void MyPatients_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("My Patients page is not implemented yet.");
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Doctor's calendar page will be implemented soon.");
        }


        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;
            return age;
        }

        private void StartAppointment(Appointment appointment)
        {
            var page = new AppointmentDetailPage(appointment, MainFrame, ContentColumn1);
            ContentColumn1.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible; // Show the frame
            MainFrame.Navigate(page);
        }


        private void OpenMedicalCard(Patient patient)
        {
            // Open the medical card window
        }
    }
}
