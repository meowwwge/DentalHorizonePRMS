namespace DentalHorizonePRMS.DTOs.Patients
{
	public class PatientDTO
	{
		public string PatientName { get; set; } = null!;
		public string Address { get; set; } = null!;
		public string Telephone { get; set; } = null!;
		public int Age { get; set; }
		public string Occupation { get; set; } = null!;
		public string Status { get; set; } = null!;
		public string Complaint { get; set; } = null!;
		public DateTime LastVisit { get; set; }
	}
}
