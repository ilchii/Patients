using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patients.Models
{
    public class Episode
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EpisodeType { get; set; }

        public Appointment Appointment { get; set; }
        public string Symptoms { get; set; }
        public string DiagnosisICPC2 { get; set; }
        public string DiagnosisICD10 { get; set; }
        public DateTime DiscoveryDate { get; set; }
        public string ClinicalStatus { get; set; }
        public string ReliabilityStatus { get; set; }
        public string DiseaseStage { get; set; }
        public string ConditionSeverity { get; set; }
        public string DiseaseType { get; set; }

    }

}
