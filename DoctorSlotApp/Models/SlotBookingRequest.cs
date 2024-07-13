namespace DoctorSlotApp.Models
{
    public class SlotBookingRequest
    {
        //public string SlotId { get; set; }
        //public string PatientName { get; set; }
        //public string PatientEmail { get; set; }
        public string FacilityId { get; set; }  
        public string Start { get; set; } // Start timestamp in the format YYYY-MM-dd HH:mm:ss
        public string End { get; set; } // End timestamp in the format YYYY-MM-dd HH:mm:ss
        public string Comments { get; set; } // Additional instructions for the doctor
        public PatientInfo Patient { get; set; } // Patient information
    }

}
