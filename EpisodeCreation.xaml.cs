using Microsoft.EntityFrameworkCore;
using Patients.Data;
using Patients.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Patients
{
    public partial class EpisodeCreationPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        private readonly int _appointmentId;
        private Appointment _appointment;
        private Patient _patient;

        private List<Icpc2Icd10> allSymptoms = new();
        private List<Icpc2Icd10> filteredSymptoms = new();
        private List<Icpc2Icd10> selectedSymptoms = new();

        private List<Icpc2Icd10> allICPC2Diagnoses;
        private List<Icpc2Icd10> allICD10Diagnoses;

        private bool icpc2DropdownOpen = false;
        private bool icd10DropdownOpen = false;

        private Frame MainFrame => Application.Current.MainWindow.FindName("MainFrame") as Frame;

        public EpisodeCreationPage(Appointment appointment)
        {
            InitializeComponent();
            _appointmentId = appointment.Id;
            LoadAppointmentDetails();
            LoadData();
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

        private void LoadData()
        {
            allSymptoms = _context.Icpc2Icd10
                .AsEnumerable()
                .Where(s => s.Type == "symptom")
                .GroupBy(s => s.ICPC2)
                .Select(g => g.First())
                .OrderBy(s => s.ICPC2)
                .ToList();

            FilterSymptoms();


            allICPC2Diagnoses = _context.Icpc2Icd10
                .AsEnumerable()
                .Where(d => d.Type == "diagnosis")
                .GroupBy(d => d.ICPC2)
                .Select(g => g.First())
                .OrderBy(d => d.ICPC2)
                .ToList();

            allICD10Diagnoses = _context.Icpc2Icd10
                .AsEnumerable()
                .Where(d => d.Type == "diagnosis")
                .GroupBy(d => d.ICD10)
                .Select(g => g.First())
                .OrderBy(d => d.ICD10)
                .ToList();

            FilterICPC2Diagnoses();
            FilterICD10Diagnoses();
        }

        private void FilterSymptoms()
        {
            string query = SearchBox.Text.ToLower();
            filteredSymptoms = allSymptoms
                .Where(s => !selectedSymptoms.Any(sel => sel.ICPC2 == s.ICPC2) &&
                            (s.ICPC2.ToLower().Contains(query) ||
                             s.TextICPC2.ToLower().Contains(query)))
                .ToList();

            SymptomsListBox.ItemsSource = null;
            SymptomsListBox.ItemsSource = filteredSymptoms;
            SymptomsListBox.ItemsSource = filteredSymptoms
            .Select(s => new { Display = $"{s.ICPC2} – {s.TextICPC2}", Original = s })
            .ToList();

            SymptomsListBox.DisplayMemberPath = "Display";

        }

        private void SymptomSearchBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SymptomsListBox.Visibility = Visibility.Visible;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterSymptoms();
        }

        private void SymptomsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SymptomsListBox.SelectedItem is null) return;

            var selected = (dynamic)SymptomsListBox.SelectedItem;
            Icpc2Icd10 symptom = selected.Original;

            if (selectedSymptoms.Count >= 5)
            {
                MessageBox.Show("You can select up to 5 symptoms.");
                SymptomsListBox.SelectedItem = null;
                return;
            }

            selectedSymptoms.Add(symptom);
            RefreshSelectedSymptoms();
            FilterSymptoms();
        }

        private void RefreshSelectedSymptoms()
        {
            SelectedSymptomsPanel.Children.Clear();

            foreach (var symptom in selectedSymptoms)
            {
                var border = new Border
                {
                    Background = System.Windows.Media.Brushes.LightGray,
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(5),
                    Padding = new Thickness(5),
                    Child = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new TextBlock { Text = $"{symptom.ICPC2}: {symptom.TextICPC2}", Margin = new Thickness(0,0,5,0) },
                            new Button
                            {
                                Content = "✕",
                                Width = 20,
                                Height = 20,
                                Padding = new Thickness(0),
                                Tag = symptom
                            }
                        }
                    }
                };

                ((Button)((StackPanel)border.Child).Children[1]).Click += RemoveSymptom_Click;

                SelectedSymptomsPanel.Children.Add(border);
            }
        }

        private void RemoveSymptom_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Icpc2Icd10 toRemove)
            {
                selectedSymptoms.Remove(toRemove);
                RefreshSelectedSymptoms();
                FilterSymptoms();
            }
        }

        private void FilterICPC2Diagnoses()
        {
            string search = ICPC2SearchBox.Text.ToLower();

            var filtered = allICPC2Diagnoses
                .Where(d =>
                    (d.TextICPC2 ?? "").ToLower().Contains(search) ||
                    (d.ICPC2 ?? "").ToLower().Contains(search))
                .ToList();

            ICPC2ListBox.ItemsSource = filtered
                .Select(d => new { Display = $"{d.ICPC2} – {d.TextICPC2}", Original = d })
                .ToList();

            ICPC2ListBox.DisplayMemberPath = "Display";
        }

        private void FilterICD10Diagnoses()
        {
            string search = ICD10SearchBox.Text.ToLower();

            var filtered = allICD10Diagnoses
                .Where(d =>
                    (d.TextICD10 ?? "").ToLower().Contains(search) ||
                    (d.ICD10 ?? "").ToLower().Contains(search))
                .ToList();

            ICD10ListBox.ItemsSource = filtered
                .Select(d => new { Display = $"{d.ICD10} – {d.TextICD10}", Original = d })
                .ToList();

            ICD10ListBox.DisplayMemberPath = "Display";
        }

        private void ICPC2ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ICPC2ListBox.SelectedItem is null) return;

            var selected = ICPC2ListBox.SelectedItem as dynamic;
            var selectedICPC = selected?.Original?.ICPC2;

            if (selectedICPC == null) return;

             ICPC2SearchBox.Text = $"{selected.Original.ICPC2} – {selected.Original.TextICPC2}";

            var linkedICD10s = _context.Icpc2Icd10
                .AsEnumerable()
                .Where(d => d.Type == "diagnosis" && d.ICPC2 == selectedICPC)
                .GroupBy(d => d.ICD10)
                .Select(g => g.First())
                .OrderBy(d => d.ICD10)
                .ToList();

            ICD10ListBox.ItemsSource = linkedICD10s
                .Select(d => new { Display = $"{d.ICD10} – {d.TextICD10}", Original = d })
                .ToList();

            ICD10ListBox.DisplayMemberPath = "Display";

            ICPC2ListBox.Visibility = Visibility.Collapsed;
            icpc2DropdownOpen = false;
            ICPC2ListBox.SelectedItem = null;

            EpisodeNameBox.Text = selected.Display;
        }

        private void ICD10ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ICD10ListBox.SelectedItem is null) return;

            var selected = ICD10ListBox.SelectedItem as dynamic;
            var selectedICD10 = selected?.Original?.ICD10;

            if (selectedICD10 == null) return;

            ICD10SearchBox.Text = $"{selected.Original.ICD10} – {selected.Original.TextICD10}";

            var linkedICPC2s = _context.Icpc2Icd10
                .AsEnumerable()
                .Where(d => d.Type == "diagnosis" && d.ICD10 == selectedICD10)
                .GroupBy(d => d.ICPC2)
                .Select(g => g.First())
                .OrderBy(d => d.ICPC2)
                .ToList();

            ICPC2ListBox.ItemsSource = linkedICPC2s
                .Select(d => new { Display = $"{d.ICPC2} – {d.TextICPC2}", Original = d })
                .ToList();

            ICPC2ListBox.DisplayMemberPath = "Display";

            ICD10ListBox.Visibility = Visibility.Collapsed;
            icpc2DropdownOpen = false;
            ICPC2ListBox.SelectedItem = null;
        }

        private void ICPC2SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterICPC2Diagnoses();
        }

        private void ICD10SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterICD10Diagnoses();
        }

        private void ICPC2SearchBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!icpc2DropdownOpen)
            {
                ICPC2ListBox.Visibility = Visibility.Visible;
                icpc2DropdownOpen = true;
            }
        }

        private void ICD10SearchBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!icd10DropdownOpen)
            {
                ICD10ListBox.Visibility = Visibility.Visible;
                icd10DropdownOpen = true;
            }
        }

        private void MainGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Find the element clicked
            DependencyObject clickedElement = Mouse.DirectlyOver as DependencyObject;

            // If clicked outside ICPC2SearchBox and ICPC2ListBox
            if (!IsDescendantOf(clickedElement, ICPC2SearchBox) &&
                !IsDescendantOf(clickedElement, ICPC2ListBox))
            {
                ICPC2ListBox.Visibility = Visibility.Collapsed;
                icpc2DropdownOpen = false;
            }

            // If clicked outside ICD10SearchBox and ICD10ListBox
            if (!IsDescendantOf(clickedElement, ICD10SearchBox) &&
                !IsDescendantOf(clickedElement, ICD10ListBox))
            {
                ICD10ListBox.Visibility = Visibility.Collapsed;
                icd10DropdownOpen = false;
            }

            // If clicked outside ICD10SearchBox and ICD10ListBox
            if (!IsDescendantOf(clickedElement, SearchBox) &&
                !IsDescendantOf(clickedElement, SymptomsListBox))
            {
                SymptomsListBox.Visibility = Visibility.Collapsed;
            }
        }

        private bool IsDescendantOf(DependencyObject target, DependencyObject ancestor)
        {
            while (target != null)
            {
                if (target == ancestor)
                    return true;
                target = VisualTreeHelper.GetParent(target);
            }
            return false;
        }

        private void TogglePopup(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox tb && tb.Tag is string popupName)
            {
                var popup = FindName(popupName) as Popup;
                if (popup != null)
                {
                    popup.IsOpen = !popup.IsOpen;
                    e.Handled = true;
                }
            }
        }

        private void PopupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox lb && lb.SelectedItem is ListBoxItem selectedItem && lb.Tag is string textBoxName)
            {
                var tb = FindName(textBoxName) as TextBox;
                if (tb != null)
                {
                    tb.Text = selectedItem.Content.ToString();
                    var popup = LogicalTreeHelper.GetParent(lb);
                    while (popup != null && !(popup is Popup))
                        popup = LogicalTreeHelper.GetParent(popup);

                    if (popup is Popup p)
                        p.IsOpen = false;

                    lb.SelectedItem = null; // reset selection
                }
            }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var appointmentDetailPage = new AppointmentDetailPage(_appointment);
            MainFrame.Navigate(appointmentDetailPage);
        }

        private void SaveEpisodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EpisodeNameBox.Text))
            {
                MessageBox.Show("Episode name cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (selectedSymptoms.Count == 0)
            {
                MessageBox.Show("Please select at least one symptom.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var episode = new Episode
            {
                Title = EpisodeNameBox.Text,
                AppointmentId = _appointmentId,
                CreatedAt = EpisodeStartDatePicker.DisplayDate,
                EpisodeType = EpisodeTypeBox.Text,

                Symptoms = string.Join(", ", selectedSymptoms.Select(s => s.ICPC2)),
                DiagnosisICPC2 = ICPC2SearchBox.Text,
                DiagnosisICD10 = ICD10SearchBox.Text,

                DiscoveryDate = DiscoveryDatePicker.DisplayDate,
                ClinicalStatus = ClinicalStatusBox.Text,
                ReliabilityStatus = ReliabilityStatusBox.Text,
                DiseaseStage = DiseaseStageBox.Text,
                ConditionSeverity = ConditionSeverityBox.Text,
                DiseaseType = DiseaseTypeBox.Text,

            };
            _context.Episodes.Add(episode);
            _context.SaveChanges();
            MessageBox.Show("Episode saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            // Navigate back to appointment details
            Back_Click(sender, e);
        }
    }
}
