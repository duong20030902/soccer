using Soccer.Font_end.Areas.ViewModels;
using System.Text.Json;

namespace Soccer.Font_end.Areas.Services
{
    public class BookingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public BookingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiBaseUrl = "https://localhost:7237/api";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Lấy danh sách bookings với pagination và filter
        public async Task<PaginatedBookingResponse> GetBookingsAsync(
            int page = 1,
            int pageSize = 10,
            string? status = null,
            string? fieldName = null,
            string? userName = null)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrEmpty(status))
                    queryParams.Add($"status={Uri.EscapeDataString(status)}");

                if (!string.IsNullOrEmpty(fieldName))
                    queryParams.Add($"fieldName={Uri.EscapeDataString(fieldName)}");

                if (!string.IsNullOrEmpty(userName))
                    queryParams.Add($"userName={Uri.EscapeDataString(userName)}");

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/admin/bookings?{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<PaginatedBookingResponse>(jsonString, _jsonOptions);
                    return result ?? new PaginatedBookingResponse();
                }

                return new PaginatedBookingResponse();
            }
            catch (Exception)
            {
                return new PaginatedBookingResponse();
            }
        }

        // Lấy booking theo ID
        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/admin/bookings/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<BookingDto>(jsonString, _jsonOptions);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CreateBookingAsync(CreateBookingRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/admin/bookings", content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Lỗi khi tạo booking: {response.StatusCode} - {errorContent}");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể tạo booking: {ex.Message}", ex);
            }
        }

        // THÊM METHOD CẬP NHẬT BOOKING
        public async Task<bool> UpdateBookingAsync(BookingDto booking)
        {
            try
            {
                var json = JsonSerializer.Serialize(booking, _jsonOptions);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/admin/bookings/{booking.BookingId}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Cập nhật trạng thái booking
        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string status)
        {
            try
            {
                var request = new UpdateStatusRequest { Status = status };
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/admin/bookings/{bookingId}/status", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Hủy booking
        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/admin/bookings/{bookingId}/cancel");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Lấy thống kê booking
        public async Task<BookingStatistics?> GetBookingStatisticsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/admin/bookings/statistics");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<BookingStatistics>(jsonString, _jsonOptions);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Phương thức backward compatibility
        public async Task<List<BookingDto>> GetAllBookingsAsync()
        {
            var result = await GetBookingsAsync();
            return result.Data;
        }

        public async Task<List<BookingDto>> GetBookingsByStatusAsync(string status)
        {
            var result = await GetBookingsAsync(status: status);
            return result.Data;
        }

        public async Task<List<BookingDto>> GetBookingsByDateAsync(DateOnly date)
        {
            var result = await GetBookingsAsync();
            return result.Data.Where(b => b.Date == date).ToList();
        }

        public async Task<List<ScheduleDto>> GetAvailableSchedulesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/admin/schedules?status=Available");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var schedules = JsonSerializer.Deserialize<List<ScheduleDto>>(jsonString, _jsonOptions);
                    return schedules ?? new List<ScheduleDto>();
                }

                throw new HttpRequestException($"Lỗi khi lấy danh sách lịch: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy danh sách lịch khả dụng", ex);
            }
        }
    }
}
