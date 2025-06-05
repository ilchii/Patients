namespace Patients.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronym { get; set; }
        public string Specialty { get; set; }

        // Optional convenience property:
        public string ShortFullName => $"{Surname} {Name[0]}. {Patronym[0]}.";
    }

}
