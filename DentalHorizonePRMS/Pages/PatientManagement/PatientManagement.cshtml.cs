using DentalHorizonePRMS.DTOs.PatientManagement;
using DentalHorizonePRMS.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalHorizonePRMS.Pages.PatientManagement
{
    public class PatientManagementModel : PageModel
    {
        private readonly IPatientManagementRepository _managementRepository;

        public List<PatientManagementDTO> Patients { get; set; } = new();

        public PatientManagementModel(IPatientManagementRepository managementRepository)
        {
            _managementRepository = managementRepository;
        }

        public async Task OnGetAsync() 
        {
            Patients = await _managementRepository.GetAllAsync();
        }
    }
}
