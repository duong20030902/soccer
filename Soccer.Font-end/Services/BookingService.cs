using System.Text;
using System.Text.Json;

namespace Soccer.Font_end.Services
{
    public class BookingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public BookingService(IConfiguration configuration)
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

        public async Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(CreateBookingRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Booking", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var booking = JsonSerializer.Deserialize<BookingResponseDto>(responseContent, _jsonOptions);
                    return new ApiResponse<BookingResponseDto>
                    {
                        Success = true,
                        Data = booking,
                        Message = "Đặt sân thành công"
                    };
                }

                return new ApiResponse<BookingResponseDto>
                {
                    Success = false,
                    Message = ExtractErrorMessage(responseContent) ?? "Có lỗi xảy ra khi đặt sân"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<BookingResponseDto>
                {
                    Success = false,
                    Message = "Có lỗi xảy ra. Vui lòng thử lại sau."
                };
            }
        }

        public async Task<ApiResponse<BookingResponseDto>> GetBookingAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Booking/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var booking = JsonSerializer.Deserialize<BookingResponseDto>(responseContent, _jsonOptions);
                    return new ApiResponse<BookingResponseDto>
                    {
                        Success = true,
                        Data = booking
                    };
                }

                return new ApiResponse<BookingResponseDto>
                {
                    Success = false,
                    Message = ExtractErrorMessage(responseContent) ?? "Không tìm thấy booking"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<BookingResponseDto>
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin booking"
                };
            }
        }

        public async Task<ApiResponse<PagedResult<BookingResponseDto>>> GetMyBookingsAsync(int page = 1, int pageSize = 10, string? status = null)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrEmpty(status))
                {
                    queryParams.Add($"status={status}");
                }

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Booking/my-bookings?{queryString}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<PagedResult<BookingResponseDto>>(responseContent, _jsonOptions);
                    return new ApiResponse<PagedResult<BookingResponseDto>>
                    {
                        Success = true,
                        Data = result
                    };
                }

                return new ApiResponse<PagedResult<BookingResponseDto>>
                {
                    Success = false,
                    Message = ExtractErrorMessage(responseContent) ?? "Có lỗi xảy ra khi lấy danh sách booking"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<BookingResponseDto>>
                {
                    Success = false,
                    Message = "Có lỗi xảy ra. Vui lòng thử lại sau."
                };
            }
        }

        public async Task<ApiResponse<string>> UpdateBookingStatusAsync(int id, string status)
        {
            try
            {
                var request = new { Status = status };
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/Booking/{id}/status", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<string>
                    {
                        Success = true,
                        Message = "Cập nhật trạng thái thành công"
                    };
                }

                return new ApiResponse<string>
                {
                    Success = false,
                    Message = ExtractErrorMessage(responseContent) ?? "Có lỗi xảy ra khi cập nhật trạng thái"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Có lỗi xảy ra. Vui lòng thử lại sau."
                };
            }
        }

        public async Task<ApiResponse<string>> CancelBookingAsync(int id)
        {
            try
            {
                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/Booking/{id}/cancel", null);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<string>
                    {
                        Success = true,
                        Message = "Hủy booking thành công"
                    };
                }

                return new ApiResponse<string>
                {
                    Success = false,
                    Message = ExtractErrorMessage(responseContent) ?? "Có lỗi xảy ra khi hủy booking"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Có lỗi xảy ra. Vui lòng thử lại sau."
                };
            }
        }

        private string? ExtractErrorMessage(string responseContent)
        {
            try
            {
                using var document = JsonDocument.Parse(responseContent);

                // Thử lấy message từ các field khác nhau
                if (document.RootElement.TryGetProperty("message", out var messageElement))
                {
                    return messageElement.GetString();
                }

                if (document.RootElement.TryGetProperty("title", out var titleElement))
                {
                    return titleElement.GetString();
                }

                // Nếu là string trực tiếp
                if (document.RootElement.ValueKind == JsonValueKind.String)
                {
                    return document.RootElement.GetString();
                }
            }
            catch
            {
                // Nếu không parse được JSON, trả về content gốc
                return responseContent;
            }

            return null;
        }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal Price { get; set; }
        public decimal Commission { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime BookingTime { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateBookingRequest
    {
        public int ScheduleId { get; set; }
        public int UserId { get; set; }
        public string? Notes { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
