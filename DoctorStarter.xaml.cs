using Microsoft.EntityFrameworkCore;
using Patients.Data;
using Patients.Models; // Update namespace to match your project
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

        public DoctorStarterWindow()
        {
            InitializeComponent();
            LoadTodaysAppointments();
            //LoadAppointments();
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
                }
            }
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
            // Open the appointment page
        }

        private void OpenMedicalCard(Patient patient)
        {
            // Open the medical card window
        }


        /*        private void LoadAppointments()
                {
                    var today = DateTime.Today;

                    var appointments = _context.Appointments
                        .Where(a => a.DoctorId == 1 && a.Date.Date == today)
                        .ToList();

                    AppointmentsListBox.ItemsSource = appointments;
                }*/
    }
}
