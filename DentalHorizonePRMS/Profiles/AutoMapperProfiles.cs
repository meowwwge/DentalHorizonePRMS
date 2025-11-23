using AutoMapper;
using DentalHorizonePRMS.DTOs.Dashboard;
using DentalHorizonePRMS.DTOs.PatientManagement;
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
            CreateMap<User, UserDTO>();

            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<UserLoginDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

			CreateMap<FullPatientDetailsDTO, Patient>()
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.PatientStatus))
				.ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PatientName))
				.ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
				.ForMember(dest => dest.Telephone, opt => opt.MapFrom(src => src.Telephone))
				.ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
				.ForMember(dest => dest.Occupation, opt => opt.MapFrom(src => src.Occupation))
				.ForMember(dest => dest.Complaint, opt => opt.MapFrom(src => src.Complaint));

			CreateMap<FullPatientDetailsDTO, PatientManagementDTO>().ReverseMap()
				.ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PatientName))
	            //.ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
	            //.ForMember(dest => dest.Occupation, opt => opt.MapFrom(src => src.Occupation))
	            .ForMember(dest => dest.Telephone, opt => opt.MapFrom(src => src.Telephone))
	            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
	            .ForMember(dest => dest.Complaint, opt => opt.MapFrom(src => src.Complaint))
	           // .ForMember(dest => dest.LastVisit, opt => opt.MapFrom(src => src.LastVisit))
	            .ForMember(dest => dest.NextAppointment, opt => opt.MapFrom(src => src.NextAppointment))
	            .ForMember(dest => dest.PatientStatus, opt => opt.MapFrom(src => src.PatientStatus))
	            .ForMember(dest => dest.VisitStatus, opt => opt.MapFrom(src => "Active"))
				.ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service));

			CreateMap<Patient, PatientDTO>().ReverseMap();


			CreateMap<PatientManagement, PatientManagementDTO>().ReverseMap();

            CreateMap<DashboardRecord, DashboardRecordDTO>().ReverseMap();

            CreateMap<PatientMedicalHistory, PatientMedicalHistoryDTO>().ReverseMap();
        }
        
    }
}
