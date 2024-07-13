using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using DoctorSlotAPI.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DoctorSlotAPI.Services
{
    public class SlotService:ISlotService
    {
        private readonly HttpClient _httpClient;
        private readonly SlotServiceSettings _settings;
       
        public SlotService(HttpClient httpClient, IOptions<SlotServiceSettings> settings)
        {
            _httpClient = httpClient;
            var authToken = Encoding.ASCII.GetBytes("techuser:secretpassWord");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            _settings = settings.Value;
        }

        public async Task<Slots> GetWeeklyAvailability(string date)
        {
            var response = await _httpClient.GetAsync($"{_settings.AvailabilityUrl}/{date}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
            }
            var schedule = JsonConvert.DeserializeObject<Schedule>(content);
            var slotsObject = ConvertScheduleToSlots(schedule, date);
            return slotsObject;
        }

        public Slots ConvertScheduleToSlots(Schedule schedule, string date)
        {
            var slotmv = new Slots();
            slotmv.FacilityDaySchedules = new List<FacilityDaySchedule>();
            foreach (var day in GetWeeklySchedules(schedule))
            {
                if (day.Item2 != null)
                {
                    var Slots = new List<Slot>();
                    var slotDurationMinutes = schedule.SlotDurationMinutes;
                    var slots = GetAvailableSlots(slotDurationMinutes, day.Item2);
                    var facilityDaySchedule = new FacilityDaySchedule();
                    facilityDaySchedule.SlotDurationMinutes = slotDurationMinutes;
                    facilityDaySchedule.Day = CalculateDateFromMonday(date, day.Item1);
                    facilityDaySchedule.DayName = day.Item1;
                    facilityDaySchedule.Slots = slots;
                    facilityDaySchedule.FacilityId = schedule.Facility.FacilityId;
                    slotmv.FacilityDaySchedules.Add(facilityDaySchedule);

                }
            }

            return slotmv;
        }

        private string CalculateDateFromMonday(string date, string dayName)
        {
            int daysToAdd = dayName switch
            {
                "Monday" => 0,
                "Tuesday" => 1,
                "Wednesday" => 2,
                "Thursday" => 3,
                "Friday" => 4,
                "Saturday" => 5,
                "Sunday" => 6,
                _ => 0
            };

            DateTime dayDate = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            DateTime newDayDate = dayDate.AddDays(daysToAdd);
            return newDayDate.ToString("yyyyMMdd");

        }

        private List<(string, DaySchedule)> GetWeeklySchedules(Schedule schedule)
        {
            return new List<(string, DaySchedule)>
        {
            ("Monday", schedule.Monday),
            ("Tuesday", schedule.Tuesday),
            ("Wednesday", schedule.Wednesday),
            ("Thursday", schedule.Thursday),
            ("Friday", schedule.Friday),
            ("Saturday", schedule.Saturday),
            ("Sunday", schedule.Sunday)
        };
        }

        private List<string> GetAvailableSlots(int slotDurationMinutes, DaySchedule dayAvailability)
        {
            var availableSlots = new List<string>();
            if (dayAvailability == null || dayAvailability.WorkPeriod == null)
            {
                return availableSlots;
            }

            var startHour = new DateTime().AddHours(dayAvailability.WorkPeriod.StartHour);
            var end = new DateTime().AddHours(dayAvailability.WorkPeriod.EndHour);
            var lunchStart = new DateTime().AddHours(dayAvailability.WorkPeriod.LunchStartHour);
            var lunchEnd = new DateTime().AddHours(dayAvailability.WorkPeriod.LunchEndHour);

            while (startHour < end)
            {
                if (!(startHour >= lunchStart && startHour < lunchEnd))
                {
                    if (dayAvailability.BusySlots == null ||
                        !dayAvailability.BusySlots.Any(slot => slot.Start.ToString("HH:mm") == startHour.ToString("HH:mm")))
                    {
                        availableSlots.Add(startHour.ToString("HH:mm"));
                    }
                }
                startHour = startHour.AddMinutes(slotDurationMinutes);
            }

            return availableSlots;
        }


        public async Task<bool> TakeSlot(SlotBookingRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(_settings.TakeSlotUrl, content);
            if (!response.IsSuccessStatusCode)
            { 
                string responseBody = await response.Content.ReadAsStringAsync();
            }

            return response.IsSuccessStatusCode;
        }


    }
}
