namespace DoctorSlotAPI.Models
{
    public class Slot
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}
