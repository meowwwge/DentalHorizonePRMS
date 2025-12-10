using DentalHorizonePRMS.Entities;

namespace DentalHorizonePRMS.DTOs.PatientMedicalHistory
{
	public class PatientVisitHistoryDTO
	{
		public int Id { get; set; }              
		public int PatientId { get; set; }        
		public string Service { get; set; } = null!;   
		public DateTime ServiceDate { get; set; }
		public decimal Debit { get; set; }       
		public decimal Credit { get; set; }
	}

}
