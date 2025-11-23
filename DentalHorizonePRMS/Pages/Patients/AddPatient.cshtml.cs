using DentalHorizonePRMS.DTOs.Patients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalHorizonePRMS.Pages.Patients
{
    public class AddPatientModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public FullPatientDetailsDTO CreateDTO { get; set; } = new();

        public AddPatientModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("API");
        }

        public async Task<IActionResult> OnPostAsync() 
        {
            var response = await _httpClient.PostAsJsonAsync("api/Patient/create-patient", CreateDTO);
            if (response.IsSuccessStatusCode) 
            {
                return RedirectToPage("PatientManagement/PatientManagement");
            }
            ModelState.AddModelError("", "Failed to add patient.");
            return Page();
        }
    }
}
