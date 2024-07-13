using Moq;
using Xunit;
using DoctorSlotAPI.Controllers;
using DoctorSlotAPI.Services;
using DoctorSlotAPI.Models;
using Microsoft.AspNetCore.Mvc;

public class SlotsControllerTests
{
    [Fact]
    public async Task GIVEN_the_GetWeeklyAvailability_THEN_Returns_OkObjectResult_WHEN_Service_Succeeds()
    {
        var mockSlotService = new Mock<ISlotService>();
        var controller = new SlotsController(mockSlotService.Object);
        var date = "2024-07-10"; 
        var expectedSlots = new Slots();
        mockSlotService.Setup(s => s.GetWeeklyAvailability(date))
                    .ReturnsAsync(expectedSlots);

        var result = await controller.GetWeeklyAvailableSlots(date);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualSlots = Assert.IsType<Slots>(okResult.Value);
        Assert.Equal(expectedSlots, actualSlots);

    }

    [Fact]
    public async Task GIVEN_the_GetWeeklyAvailability_THEN_Returns_BadRequestResult_WHEN_Service_Fails()
    {
        var mockSlotService = new Mock<ISlotService>();
        var controller = new SlotsController(mockSlotService.Object);
        var date = "2024-07-10";
        mockSlotService.Setup(s => s.GetWeeklyAvailability(date))
            .ThrowsAsync(new Exception("Service failure"));

        var result = await controller.GetWeeklyAvailableSlots(date);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorMessage = Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal("Service failure", errorMessage);
    }

    [Fact]
    public async Task GIVEN_the_BookSlot_THEN_Returns_OkResult_WHEN_Service_Succeeds()
    {
        var mockSlotService = new Mock<ISlotService>();
        var controller = new SlotsController(mockSlotService.Object);
        var request = new SlotBookingRequest(); 
        mockSlotService.Setup(s => s.TakeSlot(request)).ReturnsAsync(true);
        var result = await controller.BookSlot(request);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task GIVEN_the_BookSlot_THEN_Returns_BadRequest_WHEN_Service_Fails()
    {
        var mockSlotService = new Mock<ISlotService>();
        var controller = new SlotsController(mockSlotService.Object);
        var request = new SlotBookingRequest(); 
        mockSlotService.Setup(s => s.TakeSlot(request)).ReturnsAsync(false);
        var result = await controller.BookSlot(request);

        Assert.IsType<BadRequestResult>(result);
    }

       
}
