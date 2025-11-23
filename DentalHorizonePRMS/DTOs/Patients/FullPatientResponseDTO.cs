using DentalHorizonePRMS.DTOs.Dashboard;
using DentalHorizonePRMS.DTOs.PatientManagement;
using DentalHorizonePRMS.DTOs.PatientMedicalHistory;
using DentalHorizonePRMS.Entities;

namespace DentalHorizonePRMS.DTOs.Patients
{
    public class FullPatientResponseDTO
    {
        public PatientDTO Patient { get; set; } = null!;
        public PatientManagementDTO? PatientManagement { get; set; } 
        public DashboardRecordDTO? DashboardRecord { get; set; }

        public PatientMedicalHistoryDTO? PatientMedicalHistory { get; set; }

    }
}
