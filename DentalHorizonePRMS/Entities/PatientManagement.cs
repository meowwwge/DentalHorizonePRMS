namespace DentalHorizonePRMS.Entities
{
    public class PatientManagement
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public DateTime LastVisit { get; set; }
        public DateTime NextAppointment { get; set; }
        public string PatientStatus { get; set; } = null!;
        public string Service { get; set; } = null!;
      
    }
}
