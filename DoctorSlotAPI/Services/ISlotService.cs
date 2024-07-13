using DoctorSlotAPI.Models;

namespace DoctorSlotAPI.Services
{
    public interface ISlotService
    {
        Task<Slots> GetWeeklyAvailability(string date);
        Task<bool> TakeSlot(SlotBookingRequest request);
    }
}
