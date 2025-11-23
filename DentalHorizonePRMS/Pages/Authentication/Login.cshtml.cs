using DentalHorizonePRMS.DTOs.User;
using DentalHorizonePRMS.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalHorizonePRMS.Pages.Authentication
{
    public class LoginModel : PageModel
    {
        private readonly IUserRepository _userRepository;

        public LoginModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [BindProperty]
        public UserLoginDTO LoginDTO { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var isValid = await _userRepository.ValidateCredentialsAsync(LoginDTO.Username, LoginDTO.Password);

            if (!isValid)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
            
            return RedirectToPage("/Home/Index");
        }
    }
}
