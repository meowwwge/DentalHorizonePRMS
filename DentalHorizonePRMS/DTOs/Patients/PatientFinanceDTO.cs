namespace DentalHorizonePRMS.DTOs.Patients
{
	public class PatientFinanceDTO
	{
		public int Id { get; set; }
		public string PatientName { get; set; } = null!;
		public int Age { get; set; }
		public string Occupation { get; set; } = null!;
		public string Telephone { get; set; } = null!;
		public string Address { get; set; } = null!;
		public DateTime DateOfVisit { get; set; }
		public string Complaint { get; set; } = null!;
		public decimal Debit { get; set; }
		public decimal Credit { get; set; }
		public decimal Balance => Debit - Credit;
		public string Service { get; set; } = null!;
		public string Status { get; set; } = null!;
	}
}
