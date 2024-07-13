using Moq;
using Xunit;
using DoctorSlotAPI.Controllers;
using DoctorSlotAPI.Services;
using DoctorSlotAPI.Models;
using Microsoft.Extensions.Options;

public class SlotServiceTests
{
    [Fact]
    public void ConvertScheduleToSlotsTest_GIVEN_some_slots_WHEN_I_get_Available_Slots_THEN_The_Busy_ones_are_not_in_the_list()

    {
        var settings = Options.Create(new SlotServiceSettings());
        var slotService = new SlotService(new HttpClient(), settings); 
        var schedule = new Schedule
        {
            SlotDurationMinutes = 30,
            Facility = new Facility { FacilityId = "Facility123" },
            Monday = new DaySchedule
            {
                WorkPeriod = new WorkPeriod { StartHour = 9, EndHour = 17, LunchStartHour = 12, LunchEndHour = 13 },
                BusySlots = new List<BusySlot> { new BusySlot { Start = new DateTime(1, 1, 1, 10, 0, 0) } }
            }            
        };
        string date = "20240708"; 
        var slots = slotService.ConvertScheduleToSlots(schedule, date);
        var mondaySchedule = slots.FacilityDaySchedules[0];
        var expectedSlots = new List<string>
        {
            "09:00", "09:30", "10:30", "11:00", "11:30", // 10:00 is busy
            "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", "16:00", "16:30"
        };

        Assert.Equal(expectedSlots, mondaySchedule.Slots);
    }





}
