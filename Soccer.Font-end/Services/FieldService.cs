using System.Text.Json;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Services
{
    public class FieldService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public FieldService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7237/api";

            // Cấu hình JSON options để handle property naming
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Lấy danh sách sân theo ngày và khung giờ
        public async Task<(List<FieldSearchResultViewModel>? fields, string? error)> GetAvailableFieldsAsync(DateOnly date, int timeslotId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/fieldsearch/search?date={date:yyyy-MM-dd}&timeslotId={timeslotId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var fields = JsonSerializer.Deserialize<List<FieldSearchResultViewModel>>(jsonContent, _jsonOptions);
                    return (fields, null);
                }
                else
                {
                    return (null, $"API trả về lỗi: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return (null, $"Lỗi khi gọi API: {ex.Message}");
            }
        }

        // Tìm kiếm sân theo tên
        public async Task<(List<FieldSearchResultViewModel>? fields, string? error)> SearchFieldsByNameAsync(string fieldName, DateOnly date, int timeslotId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/fieldsearch/search-by-name?fieldName={Uri.EscapeDataString(fieldName)}&date={date:yyyy-MM-dd}&timeslotId={timeslotId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var fields = JsonSerializer.Deserialize<List<FieldSearchResultViewModel>>(jsonContent, _jsonOptions);
                    return (fields, null);
                }
                else
                {
                    return (null, $"API trả về lỗi: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return (null, $"Lỗi khi gọi API: {ex.Message}");
            }
        }

        // Lấy danh sách khung giờ
        public async Task<(List<TimeslotViewModel>? timeslots, string? error)> GetTimeslotsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/fieldsearch/timeslots");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var timeslots = JsonSerializer.Deserialize<List<TimeslotViewModel>>(jsonContent, _jsonOptions);
                    return (timeslots, null);
                }
                else
                {
                    return (null, $"API trả về lỗi: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return (null, $"Lỗi khi gọi API: {ex.Message}");
            }
        }

        // Lấy danh sách sân mẫu cho trang chủ (lấy sân có sẵn hôm nay)
        public async Task<(List<FieldSearchResultViewModel>? fields, string? error)> GetSampleFieldsForHomeAsync()
        {
            try
            {
                // Lấy sân có sẵn cho ngày hôm nay và khung giờ đầu tiên
                var today = DateOnly.FromDateTime(DateTime.Now);

                // Lấy danh sách timeslots trước
                var (timeslots, timeslotError) = await GetTimeslotsAsync();
                if (timeslots == null || !timeslots.Any())
                {
                    return (null, timeslotError ?? "Không có khung giờ nào");
                }

                // Lấy khung giờ đầu tiên
                var firstTimeslot = timeslots.First();

                // Lấy danh sách sân
                return await GetAvailableFieldsAsync(today, firstTimeslot.TimeslotID);
            }
            catch (Exception ex)
            {
                return (null, $"Lỗi khi lấy danh sách sân mẫu: {ex.Message}");
            }
        }

        // Dispose HttpClient
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}