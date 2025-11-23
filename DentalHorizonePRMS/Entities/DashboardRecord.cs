namespace DentalHorizonePRMS.Entities
{
    public class DashboardRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public DateTime DateOfVisit { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}
