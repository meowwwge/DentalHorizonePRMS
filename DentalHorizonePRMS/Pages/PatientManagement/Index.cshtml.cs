using Microsoft.AspNetCore.Mvc.RazorPages;
using DentalHorizonePRMS.Entities; // adjust namespace if needed
using DentalHorizonePRMS.Interfaces;

namespace DentalHorizonePRMS.Pages.PatientManagement
{
	public class IndexModel : PageModel
	{
		private readonly IPatientRepository _patientRepository;

		public IndexModel(IPatientRepository patientRepository)
		{
			_patientRepository = patientRepository;
		}

		// Expose a collection of patients
		public List<Patient> Patients { get; set; } = new();

		public async Task OnGetAsync()
		{
			Patients = await _patientRepository.GetAllPatientsAsync();
		}
	}
}
