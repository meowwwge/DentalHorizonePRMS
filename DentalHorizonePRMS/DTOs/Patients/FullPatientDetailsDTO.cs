namespace DentalHorizonePRMS.DTOs.Patients
{
    public class FullPatientDetailsDTO
    {
        public string Id { get; set; } = null!;
        public string PatientName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Telephone { get; set; } = null!;
        public int Age { get; set; }
        public string Occupation { get; set; } = null!;
        public string Complaint { get; set; } = null!;
        public string VisitStatus { get; set; } = null!;
        public DateTime LastVisit { get; set; }
        public DateTime NextAppointment { get; set; }
        public string PatientStatus { get; set; } = null!;
        public string Service { get; set; } = null!;
    }
}
