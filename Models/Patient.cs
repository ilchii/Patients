using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Patients.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [NotMapped]
        public string FullName { get; set; }


        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronym { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string HomeAddress { get; set; }

        public ICollection<Appointment> Appointments { get; set; }

        public int Age => DateTime.Today.Year - DateOfBirth.Year -
                         (DateOfBirth.Date > DateTime.Today.AddYears(-1 * (DateTime.Today.Year - DateOfBirth.Year)) ? 1 : 0);
    }
}
