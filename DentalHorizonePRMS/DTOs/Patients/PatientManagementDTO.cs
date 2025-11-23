namespace DentalHorizonePRMS.DTOs.Patients
{
	public class PatientManagementDTO
	{
		public int Id { get; set; }
		public string PatientName { get; set; } = null!;
		public string Telephone { get; set; } = null!;
		public string Address { get; set; } = null!;
		public string Complaint { get; set; } = null!;
		public DateTime? NextAppointment { get; set; }
		public string Service { get; set; } = null!;
		public string VisitStatus { get; set; } = null!;
		public string Status { get; set; } = null!;
		public string PatientStatus { get; set; } = null!;
	}
}
