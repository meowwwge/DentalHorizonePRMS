using DentalHorizonePRMS.Entities;

namespace DentalHorizonePRMS.Interfaces
{
	public interface IArchivedPatientRepository
	{
		Task SoftDeleteAsync(int patientId);
		Task RestorePatientAsync(int archivedPatientId);
		Task<IEnumerable<Patient>> GetArchivedPatientsAsync();
	}
}
