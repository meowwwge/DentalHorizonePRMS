using AutoMapper;
using DentalHorizonePRMS.DTOs.Dashboard;
using DentalHorizonePRMS.DTOs.PatientMedicalHistory;
using DentalHorizonePRMS.DTOs.Patients;
using DentalHorizonePRMS.DTOs.User;
using DentalHorizonePRMS.Entities;

namespace DentalHorizonePRMS.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() 
        {
			CreateMap<PatientCreateDTO, Patient>().ReverseMap();
			CreateMap<Patient, PatientDTO>().ReverseMap();
			CreateMap<Patient, PatientManagementDTO>().ReverseMap();
			CreateMap<Patient, PatientFinanceDTO>().ReverseMap();
			CreateMap<User, UserDTO>().ReverseMap();
			CreateMap<PatientMedicalHistory, PatientMedicalHistoryDTO>().ReverseMap();
		}

	}
}
