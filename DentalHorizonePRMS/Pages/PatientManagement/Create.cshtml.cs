using DentalHorizonePRMS.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalHorizonePRMS.Pages.PatientManagement
{
    public class CreateModel : PageModel
    {
        private readonly IPatientManagementRepository _patientManagementRepository;
        public CreateModel(IPatientManagementRepository patientManagementRepository)
        {
			_patientManagementRepository = patientManagementRepository;

		}
    }
}
