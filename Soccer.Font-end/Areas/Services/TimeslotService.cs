using Soccer.Font_end.ViewModels;
using System.Text;
using System.Text.Json;

namespace Soccer.Font_end.Areas.Services
{
    public class TimeslotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public TimeslotService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiBaseUrl = "https://localhost:7237/api/admin";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Lấy tất cả timeslots
        public async Task<List<TimeslotDto>> GetAllTimeslotsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/timeslots");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var timeslots = JsonSerializer.Deserialize<List<TimeslotDto>>(json, _jsonOptions);
                    return timeslots ?? new List<TimeslotDto>();
                }

                return new List<TimeslotDto>();
            }
            catch (Exception ex)
            {
                // Log lỗi ở đây nếu cần
                throw new Exception($"Error fetching timeslots: {ex.Message}");
            }
        }

        // Lấy timeslot theo ID
        public async Task<TimeslotDto?> GetTimeslotByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/timeslots/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TimeslotDto>(json, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching timeslot: {ex.Message}");
            }
        }

        // Tạo timeslot mới
        public async Task<TimeslotDto?> CreateTimeslotAsync(CreateTimeslotRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/timeslots", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TimeslotDto>(responseJson, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating timeslot: {ex.Message}");
            }
        }

        // Cập nhật timeslot
        public async Task<bool> UpdateTimeslotAsync(int id, UpdateTimeslotRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/timeslots/{id}", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating timeslot: {ex.Message}");
            }
        }

        // Xóa timeslot
        public async Task<bool> DeleteTimeslotAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/timeslots/{id}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting timeslot: {ex.Message}");
            }
        }
    }
}
