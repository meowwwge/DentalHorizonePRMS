namespace DentalHorizonePRMS.DTOs.ArchivedPatients
{
	public class ArchivedPatientDTO
	{
		public int Id { get; set; }
		public string PatientName { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string Telephone { get; set; } = string.Empty;
		public int Age { get; set; }
		public string Occupation { get; set; } = string.Empty;
		public string Complaint { get; set; } = string.Empty;
		public DateTime DateOfVisit { get; set; }
		public DateTime? NextAppointment { get; set; }
		public string Service { get; set; } = string.Empty;
		public string PatientStatus { get; set; } = "Inactive";
		public decimal Balance { get; set; }
	}
}
