namespace DoctorSlotAPI.Models
{
    public class FacilityDaySchedule
    {
        public string FacilityId { get; set; }
        public string Day { get; set; }
        public string DayName { get; set; }
        public int SlotDurationMinutes { get; set; }
        public List<string> Slots { get; set; }
    
    }
}
