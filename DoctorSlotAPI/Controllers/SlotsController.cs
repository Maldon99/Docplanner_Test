using DoctorSlotAPI.Models;
using DoctorSlotAPI.Services;
using Microsoft.AspNetCore.Mvc;
           
namespace DoctorSlotAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class SlotsController : ControllerBase
    {
        private readonly ISlotService _slotService;
        

        
        public SlotsController(ISlotService slotService)
        {
            _slotService = slotService;
        }

        [HttpGet("availability/{date}")]
        public async Task<IActionResult> GetWeeklyAvailableSlots(string date)
        {
            try
            {
                var slots = await _slotService.GetWeeklyAvailability(date);
                return Ok(slots);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        
        [HttpPost("book")]
        public async Task<IActionResult> BookSlot(SlotBookingRequest request)
        {
            try
            {
                var result = await _slotService.TakeSlot(request);
                if (result) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}