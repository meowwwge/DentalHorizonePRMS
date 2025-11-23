namespace DentalHorizonePRMS.DTOs.Dashboard
{
	public class UpcomingAppointmentsDTO
	{
		public int Id { get; set; }
		public string PatientName { get; set; } = null!;
		public int Age { get; set; }
		public string Telephone { get; set; } = null!;
		public DateTime? NextAppointment { get; set; }
		public string Complaint { get; set; } = null!;
		public string Service { get; set; } = null!;
		public string Status { get; set; } = null!;
	}
}
