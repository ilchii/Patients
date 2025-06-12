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
        private Frame _parentFrame;

        public AppointmentDetailPage(Appointment appointment, Frame parentFrame)
        {
            InitializeComponent();
            _appointmentId = appointment.Id;
            _parentFrame = parentFrame;
            LoadAppointmentDetails();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _parentFrame.Visibility = Visibility.Collapsed;
            _parentFrame.Content = null;
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
                NavigationService.GoBack(); // Navigate back after closing the appointment
            }
            else
            {
                MessageBox.Show("No appointment to close.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddEpisode_Click(object sender, RoutedEventArgs e)
        {
            // Logic to add an episode (not implemented in this example)
            MessageBox.Show("Add Episode functionality is not yet implemented.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
