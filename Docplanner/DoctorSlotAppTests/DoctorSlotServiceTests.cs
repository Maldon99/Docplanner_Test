using DoctorSlotApp.Models;
using DoctorSlotApp.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DoctorSlotAppTests
{
    public class DoctorSlotServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IOptions<DoctorSlotServiceSettings>> _apiUrlsOptionsMock;
        private readonly DoctorSlotServiceSettings _apiUrls;

        public DoctorSlotServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _apiUrls = new DoctorSlotServiceSettings
            {
                AvailabilitySlotsUrl = "https://localhost:7016/api/Slots/availability/",
                BookSlotUrl = "https://localhost:7016/api/Slots/book"
            };
            _apiUrlsOptionsMock = new Mock<IOptions<DoctorSlotServiceSettings>>();
            _apiUrlsOptionsMock.Setup(x => x.Value).Returns(_apiUrls);
        }

        [Fact]
        public async Task GetWeeklyAvailabilityTest_GIVEN_some_slots_WHEN_I_Get_availability_THEN_I_get_SlotMV()
        {
            var date = "20240708";
            var expectedSlots = new SlotMV() { FacilityDaySchedules = new List<FacilityDaySchedule>() };
            var facility1 = new FacilityDaySchedule() { FacilityId="facility1234"}; 
            expectedSlots.FacilityDaySchedules.Add(facility1);
            var jsonResponse = JsonConvert.SerializeObject(expectedSlots);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    var responseMessage = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(jsonResponse)
                    };
                    return responseMessage;
                });

            var service = new DoctorSlotService(_httpClient, _apiUrlsOptionsMock.Object);
            var result = await service.GetWeeklyAvailability(date);

            Assert.Equal(expectedSlots.FacilityDaySchedules[0].FacilityId, result.FacilityDaySchedules[0].FacilityId);
        }

        [Fact]
        public async Task TakeSlotTest_GIVEN_a_slot_WHEN_takeSlot_with_good_request_THEN_I_get_True ()
        {
            var request = new SlotBookingRequest();
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    var responseMessage = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                    return responseMessage;
                });

            var service = new DoctorSlotService(_httpClient, _apiUrlsOptionsMock.Object);
            var result = await service.TakeSlot(request);

            Assert.True(result);
        }

        [Fact]
        public async Task TakeSlotTest_GIVEN_a_slot_WHEN_takeSlot_with_badRequest_THEN_I_get_false()
        {
            var request = new SlotBookingRequest();
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    var responseMessage = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    return responseMessage;
                });

            var service = new DoctorSlotService(_httpClient, _apiUrlsOptionsMock.Object);
            var result = await service.TakeSlot(request);

            Assert.False(result);
        }
    }
}
