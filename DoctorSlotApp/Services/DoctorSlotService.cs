using DoctorSlotApp.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace DoctorSlotApp.Services
{
    public class DoctorSlotService
    {
        private readonly HttpClient _httpClient;
        private readonly DoctorSlotService _doctorSlotService;
        private readonly DoctorSlotServiceSettings _apiUrls;

        public DoctorSlotService(HttpClient httpClient, IOptions<DoctorSlotServiceSettings> apiUrls)
        {
            _httpClient = httpClient;
            var authToken = Encoding.ASCII.GetBytes("techuser:secretpassWord");
             _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            _apiUrls = apiUrls.Value;
        }
        public DoctorSlotService()
        {
        
        }

        public async Task<SlotMV> GetWeeklyAvailability(string date)
        {
            var response = await _httpClient.GetAsync($"{_apiUrls.AvailabilitySlotsUrl}{date}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
            }
            var slotsMV = JsonConvert.DeserializeObject<SlotMV>(content);
            return slotsMV;


        }

        public async Task<bool> TakeSlot(SlotBookingRequest request)
        {
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrls.BookSlotUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
            }
            return response.IsSuccessStatusCode;
        }
    }

}
