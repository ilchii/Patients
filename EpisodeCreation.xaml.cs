using Microsoft.EntityFrameworkCore;
using Patients.Data;
using Patients.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Patients
{
    public partial class EpisodeCreationPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        private List<Icpc2Icd10> allSymptoms = new();
        private List<Icpc2Icd10> allDiagnoses = new();

        private List<Icpc2Icd10> allICPC2Diagnoses;
        private List<Icpc2Icd10> allICD10Diagnoses;


        private List<Icpc2Icd10> filteredSymptoms = new();
        private List<Icpc2Icd10> selectedSymptoms = new();
        private Icpc2Icd10 selectedDiagnosis;


        public EpisodeCreationPage(Appointment appointment, Frame parentFrame)
        {
            InitializeComponent();
            LoadData();
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

            //allDiagnoses = _context.Icpc2Icd10
            //    .AsEnumerable()
            //    .Where(d => d.Type == "diagnosis")
            //    .GroupBy(d => d.ICPC2)
            //    .Select(g => g.First())
            //    .OrderBy(d => d.ICPC2)
            //    .ToList();

            //FilterDiagnoses();


            allICPC2Diagnoses = _context.Icpc2Icd10
                .AsEnumerable()
                .Where(d => d.Type == "diagnosis")
                .GroupBy(d => d.ICPC2)
                .Select(g => g.First())
                .OrderBy(d => d.ICPC2)
                .ToList();

            // Load and group ICD-10 diagnoses
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

        // Diagnosis selection
        //private void DiagnosisSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    FilterDiagnoses();
        //}

        //private void FilterDiagnoses()
        //{
        //    string search = DiagnosisSearchBox.Text.ToLower();

        //    var filtered = allDiagnoses
        //        .Where(d =>
        //            (d.TextICPC2 ?? "").ToLower().Contains(search) ||
        //            (d.ICPC2 ?? "").ToLower().Contains(search))
        //        .Select(d => new
        //        {
        //            Display = $"{d.ICPC2} – {d.TextICPC2}",
        //            Original = d
        //        })
        //        .ToList();

        //    DiagnosisListBox.ItemsSource = filtered;
        //    DiagnosisListBox.DisplayMemberPath = "Display";
        //}

        //private void DiagnosisListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (DiagnosisListBox.SelectedItem is null) return;

        //    var selected = (dynamic)DiagnosisListBox.SelectedItem;
        //    selectedDiagnosis = selected.Original;

        //    // Show diagnosis in search bar & text block
        //    DiagnosisSearchBox.Text = $"{selectedDiagnosis.ICPC2} – {selectedDiagnosis.TextICPC2}";
        //    SelectedDiagnosisTextBlock.Text = $"Selected: {selectedDiagnosis.ICPC2} – {selectedDiagnosis.TextICPC2}";

        //    DiagnosisListBox.SelectedItem = null;
        //    FilterDiagnoses();
        //}


        // ICPC-2 Diagnosis selection

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

            var selected = ICPC2ListBox.SelectedItem as dynamic; // Explicitly cast to dynamic
            var selectedICPC = selected?.Original?.ICPC2; // Use null-conditional operator to avoid dynamic issues

            if (selectedICPC == null) return;

            // Show diagnosis in search bar
            ICPC2SearchBox.Text = $"{selected.Original.ICPC2} – {selected.Original.TextICPC2}";

            // Filter ICD10 diagnoses to only those linked to the selected ICPC2
            var linkedICD10s = _context.Icpc2Icd10
                .AsEnumerable() // Convert to IEnumerable to avoid expression tree issues
                .Where(d => d.Type == "diagnosis" && d.ICPC2 == selectedICPC)
                .GroupBy(d => d.ICD10)
                .Select(g => g.First())
                .OrderBy(d => d.ICD10)
                .ToList();

            ICD10ListBox.ItemsSource = linkedICD10s
                .Select(d => new { Display = $"{d.ICD10} – {d.TextICD10}", Original = d })
                .ToList();

            ICD10ListBox.DisplayMemberPath = "Display";
        }

        private void ICD10ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ICD10ListBox.SelectedItem is null) return;

            var selected = ICD10ListBox.SelectedItem as dynamic; // Explicitly cast to dynamic
            var selectedICD10 = selected?.Original?.ICD10; // Use null-conditional operator to avoid dynamic issues

            if (selectedICD10 == null) return;

            ICD10SearchBox.Text = $"{selected.Original.ICD10} – {selected.Original.TextICD10}";

            // Filter ICPC2 diagnoses to only those linked to the selected ICD10
            var linkedICPC2s = _context.Icpc2Icd10
                .AsEnumerable() // Convert to IEnumerable to avoid expression tree issues
                .Where(d => d.Type == "diagnosis" && d.ICD10 == selectedICD10)
                .GroupBy(d => d.ICPC2)
                .Select(g => g.First())
                .OrderBy(d => d.ICPC2)
                .ToList();

            ICPC2ListBox.ItemsSource = linkedICPC2s
                .Select(d => new { Display = $"{d.ICPC2} – {d.TextICPC2}", Original = d })
                .ToList();

            ICPC2ListBox.DisplayMemberPath = "Display";
        }

        private void ICPC2SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterICPC2Diagnoses();
        }

        private void ICD10SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterICD10Diagnoses();
        }
    }
}
