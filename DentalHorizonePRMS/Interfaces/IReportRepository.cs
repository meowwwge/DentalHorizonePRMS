using DentalHorizonePRMS.Entities;

namespace DentalHorizonePRMS.Interfaces
{
	public interface IReportRepository
	{
		Task<IEnumerable<Patient>> GetPatientsByMonthYearAsync(int month, int year);
	}
}
