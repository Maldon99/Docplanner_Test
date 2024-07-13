namespace DoctorSlotApp.Models
{
    public class DaySchedule
    {
        public WorkPeriod WorkPeriod { get; set; }
        public List<BusySlot> BusySlots { get; set; }
    }

}
