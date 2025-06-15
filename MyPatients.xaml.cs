using Microsoft.EntityFrameworkCore;
using Patients.Data;
using Patients.Models;
using System;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Patients
{
    public partial class MyPatients : Page
    {
        private List<Patient> _doctorPatients;

        private Frame MainFrame => Application.Current.MainWindow.FindName("MainFrame") as Frame;


        public MyPatients()
        {
            InitializeComponent();
            LoadMyPatients();
        }

        private void LoadMyPatients()
        {
            PatientPanel.Children.Clear();

            using (var context = new AppDbContext())
            {
                var patients = context.Appointments
                    .Include(a => a.Patient)
                    .Where(a => a.DoctorId == 1)
                    .Select(a => a.Patient)
                    .Distinct()
                    .ToList();

                _doctorPatients = patients;

                foreach (var patient in patients)
                {
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

                    int age = CalculateAge(patient.DateOfBirth);
                    stack.Children.Add(new TextBlock { Text = $"Age: {age}" });
                    stack.Children.Add(new TextBlock { Text = $"Address: {patient.HomeAddress}" });

                    var buttonsPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(0, 10, 0, 0)
                    };

                    var cardButton = new Button
                    {
                        Content = "Medical Card",
                        Margin = new Thickness(5, 0, 0, 0)
                    };
                    cardButton.Click += (s, e) => OpenMedicalCard(patient);

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

                    PatientPanel.Children.Add(border);

                    DisplayPatients(patients);
                }
            }
        }



        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_doctorPatients == null) return;

            var keyword = SearchBox.Text.Trim().ToLower();

            var filtered = _doctorPatients
                .Where(p =>
                    p.Id.ToString().Contains(keyword) ||
                    $"{p.Surname} {p.Name} {p.Patronym}".ToLower().Contains(keyword)
                )
                .ToList();

            DisplayPatients(filtered);
        }

        private void DisplayPatients(List<Patient> patients)
        {
            PatientPanel.Children.Clear();

            foreach (var patient in patients)
            {
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

                int age = CalculateAge(patient.DateOfBirth);
                stack.Children.Add(new TextBlock { Text = $"Age: {age}" });
                stack.Children.Add(new TextBlock { Text = $"Address: {patient.HomeAddress}" });

                var buttonsPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                var cardButton = new Button
                {
                    Content = "Medical Card",
                    Margin = new Thickness(5, 0, 0, 0)
                };
                cardButton.Click += (s, e) => OpenMedicalCard(patient);

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

                PatientPanel.Children.Add(border);
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
            var page = new AppointmentDetailPage(appointment);
            MainFrame.Navigate(page);
        }


        private void OpenMedicalCard(Patient patient)
        {
            // Open the medical card window
        }
    }
}

