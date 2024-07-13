namespace DoctorSlotApp.Models
{
    public class SlotMV
    {
        public List<FacilityDaySchedule> FacilityDaySchedules { get; set; } = new List<FacilityDaySchedule>();


        public override bool Equals(object obj)
        {
            if (obj is SlotMV other)
            {
                return FacilityDaySchedules.SequenceEqual(other.FacilityDaySchedules);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return FacilityDaySchedules != null ? FacilityDaySchedules.GetHashCode() : 0;
        }

    }
}
