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

        public DoctorStarterWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new WorkingSpace());
        }

        private void WorkingSpace_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new WorkingSpace());
        }

        private void MyPatients_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MyPatients());
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DoctorCalendar());
        }
    }
}
