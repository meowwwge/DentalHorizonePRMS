using DentalHorizonePRMS.Entities;

namespace DentalHorizonePRMS.DTOs.PatientMedicalHistory
{
    public class PatientMedicalHistoryDTO
    {
        public int PatientId { get; set; }
        public string KnownAllergies { get; set; } = null!;
        public string CurrentMedications { get; set; } = null!;
        public string ChronicConditions { get; set; } = null!;
        public string PrimaryPhysicianOrDentist { get; set; } = null!;
        public string EmergencyContactName { get; set; } = null!;
        public int EmergencyContactNumber { get; set; } 
    }
}
