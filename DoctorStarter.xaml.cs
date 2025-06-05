using Microsoft.EntityFrameworkCore;
using Patients.Data;
using Patients.Models; // Update namespace to match your project
using System;
using System.Linq;
using System.Windows;

namespace Patients
{
    public partial class DoctorStarterWindow : Window
    {
        private readonly AppDbContext _context = new AppDbContext();

        public DoctorStarterWindow()
        {
            InitializeComponent();
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            var today = DateTime.Today;

            var appointments = _context.Appointments
                .Where(a => a.DoctorId == 1 && a.Date.Date == today)
                .ToList();

            AppointmentsListBox.ItemsSource = appointments;
        }
    }
}
