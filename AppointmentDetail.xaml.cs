using Microsoft.EntityFrameworkCore;
using Patients.Data;
using Patients.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Patients
{
    /// <summary>
    /// Interaction logic for AppointmentDetail.xaml
    /// </summary>
    public partial class AppointmentDetailPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly int _appointmentId;
        private Appointment _appointment;
        private Patient _patient;

        private Frame MainFrame => Application.Current.MainWindow.FindName("MainFrame") as Frame;

        public AppointmentDetailPage(Appointment appointment)
        {
            InitializeComponent();
            _appointmentId = appointment.Id;
            LoadAppointmentDetails();
            LoadEpisodes(_appointmentId);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new WorkingSpace());
        }

        private void AddEpisode_Click(object sender, RoutedEventArgs e)
        {
            var episodeCreationPage = new EpisodeCreationPage(_appointment);
            MainFrame.Navigate(episodeCreationPage);
        }

        private void LoadAppointmentDetails()
        {
            using (var context = new AppDbContext())
            {
                _appointment = context.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefault(a => a.Id == _appointmentId);
                if (_appointment != null)
                {
                    _patient = _appointment.Patient;
                    DisplayAppointmentDetails();
                }
                else
                {
                    MessageBox.Show("Appointment not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DisplayAppointmentDetails()
        {
            if (_appointment != null && _patient != null)
            {
                PatientName.Text = $"{_patient.Surname} {_patient.Name} {_patient.Patronym}";
                PatientId.Text = $"ID: {_patient.Id}";
                AppointmentTime.Text = $"{_appointment.Date:HH:mm} - {_appointment.Date.AddMinutes(_appointment.DurationMinutes):HH:mm}";
                //AppointmentNotes.Text = _appointment.Notes ?? "No notes available.";
                PatientAge.Text = $"Age: {_patient.Age}";
            }
            else
            {
                MessageBox.Show("Appointment or patient details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (_appointment != null)
            {
                //_appointment.Notes = AppointmentNotes.Text; // Assuming you have a TextBox for notes
                _appointment.Date = DateTime.Now; // Update to current time or as needed

                using (var context = new AppDbContext())
                {
                    context.Appointments.Update(_appointment);
                    context.SaveChanges();
                }

                MessageBox.Show("Appointment closed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Back_Click(sender, e);
            }
            else
            {
                MessageBox.Show("No appointment to close.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadEpisodes(int appointmentId)
        {
            EpisodesPanel.Children.Clear();

            var episodes = _context.Episodes
                .Where(e => e.AppointmentId == appointmentId)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();

            foreach (var episode in episodes)
            {
                var border = new Border
                {
                    BorderBrush = new SolidColorBrush(Colors.LightGreen),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(0, 0, 0, 10),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(10),
                    Background = Brushes.White
                };

                var stack = new StackPanel();

                stack.Children.Add(new TextBlock
                {
                    Text = episode.Title,
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 16,
                    Foreground = Brushes.Black
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"Тип: {episode.EpisodeType}",
                    Foreground = Brushes.DarkSlateGray
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"ICPC-2: {episode.DiagnosisICPC2}",
                    Foreground = Brushes.SlateGray
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"ICD-10: {episode.DiagnosisICD10}",
                    Foreground = Brushes.SlateGray
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"Створено: {episode.CreatedAt:g}",
                    FontStyle = FontStyles.Italic,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(0, 5, 0, 0)
                });

                border.Child = stack;
                EpisodesPanel.Children.Add(border);
            }
        }

    }
}
