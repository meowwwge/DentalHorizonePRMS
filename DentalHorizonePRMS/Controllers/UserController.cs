using AutoMapper;
using DentalHorizonePRMS.DTOs.User;
using DentalHorizonePRMS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalHorizonePRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper) 
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO userLoginDTO) 
        {
            var isValid = await _userRepository.ValidateCredentialsAsync(userLoginDTO.Username, userLoginDTO.Password);

            if (!isValid) return Unauthorized("Invalid username or password.");

            var user = await _userRepository.GetByUsernameAsync(userLoginDTO.Username);
            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id) 
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null) return NotFound();

            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        }
    }
}
