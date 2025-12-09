using System.ComponentModel.DataAnnotations;

namespace DentalHorizonePRMS.DTOs.Patients
{
	public class PatientCreateDTO
	{
		[Required]
		public string PatientName { get; set; } = null!;
		[Required]
		public string Address { get; set; } = null!;
		[Required]
		public string Telephone { get; set; } = null!;
		[Required]
		public int Age { get; set; }
		[Required]
		public string Occupation { get; set; } = null!;
		[Required]
		public string Complaint { get; set; } = null!;
		[Required]
		public DateTime DateOfVisit { get; set; }
		public DateTime? NextAppointment { get; set; }
		[Required]
		public string Service { get; set; } = null!;
		
		public string? Status { get; set; } = null!;
		[Required]
		public string VisitStatus { get; set; } = null!;
		[Required]
		public string PatientStatus { get; set; } = null!;
		[Required]
		public decimal Debit { get; set; }
		[Required]
		public decimal Credit { get; set; }
	}
}
