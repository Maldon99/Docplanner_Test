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

namespace DoctorSlotApp.Tests.Services
{
    public class DoctorSlotServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IOptions<ApiUrls>> _apiUrlsOptionsMock;
        private readonly ApiUrls _apiUrls;

        public DoctorSlotServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _apiUrls = new ApiUrls
            {
                AvailabilitySlotsUrl = "https://localhost:7016/api/Slots/availability/",
                BookSlotUrl = "https://localhost:7016/api/Slots/book"
            };
            _apiUrlsOptionsMock = new Mock<IOptions<ApiUrls>>();
            _apiUrlsOptionsMock.Setup(x => x.Value).Returns(_apiUrls);
        }

        [Fact]
        public async Task GetWeeklyAvailability_ReturnsSlots()
        {
            var date = "20240708";
            var expectedSlots = new SlotMV();
            var jsonResponse = JsonConvert.SerializeObject(expectedSlots);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
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

            Assert.NotNull(result);
            Assert.Equal(expectedSlots, result);
        }

        [Fact]
        public async Task TakeSlot_ReturnsTrue_WhenSuccessful()
        {
            var request = new SlotBookingRequest();
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
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
        public async Task TakeSlot_ReturnsFalse_WhenFailed()
        {
            var request = new SlotBookingRequest();
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
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
