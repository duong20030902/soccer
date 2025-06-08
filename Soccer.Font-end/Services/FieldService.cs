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

        /* // Lấy tất cả sân cho tất cả khung giờ
         public async Task<(List<FieldSearchResultViewModel>? fields, string? error)> GetAllAvailableFieldsAsync(DateOnly date)
         {
             try
             {
                 // Lấy danh sách timeslots trước
                 var (timeslots, timeslotError) = await GetTimeslotsAsync();
                 if (timeslots == null || !timeslots.Any())
                 {
                     return (null, timeslotError ?? "Không có khung giờ nào");
                 }

                 var allFields = new List<FieldSearchResultViewModel>();

                 // Lặp qua tất cả khung giờ
                 foreach (var timeslot in timeslots)
                 {
                     var (fields, error) = await GetAvailableFieldsAsync(date, timeslot.TimeslotID);
                     if (fields != null && fields.Any())
                     {
                         allFields.AddRange(fields);
                     }
                 }

                 // Loại bỏ sân trùng lặp (nếu có) dựa trên FieldId và ScheduleId
                 var uniqueFields = allFields
                     .GroupBy(f => new { f.FieldId, f.ScheduleId })
                     .Select(g => g.First())
                     .ToList();

                 return (uniqueFields, null);
             }
             catch (Exception ex)
             {
                 return (null, $"Lỗi khi lấy tất cả sân: {ex.Message}");
             }
         }*/

        // Thêm method search tổng quát
        public async Task<(List<FieldSearchResultViewModel>? fields, string? error)> SearchAllFieldsAsync(DateOnly date, string? fieldName = null)
        {
            try
            {
                var url = $"{_apiBaseUrl}/fieldsearch/search-all?date={date:yyyy-MM-dd}";
                if (!string.IsNullOrEmpty(fieldName))
                {
                    url += $"&fieldName={Uri.EscapeDataString(fieldName)}";
                }

                var response = await _httpClient.GetAsync(url);

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

        // Cải thiện method GetAllAvailableFieldsAsync để sử dụng endpoint mới
        public async Task<(List<FieldSearchResultViewModel>? fields, string? error)> GetAllAvailableFieldsAsync(DateOnly date, string? fieldName = null)
        {
            try
            {
                return await SearchAllFieldsAsync(date, fieldName);
            }
            catch (Exception ex)
            {
                return (null, $"Lỗi khi lấy tất cả sân: {ex.Message}");
            }
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
        // Cập nhật phương thức GetSampleFieldsForHomeAsync
        public async Task<(List<FieldSearchResultViewModel>? fields, string? error)> GetSampleFieldsForHomeAsync()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                return await GetAllAvailableFieldsAsync(today);
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

        public async Task<ApiResponse<FieldDetailViewModel>> GetFieldDetailsAsync(int fieldId)
        {
            try
            {
                // SỬA: Đổi từ "/field/" thành "/fieldsearch/"
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/fieldsearch/{fieldId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var fieldDetail = JsonSerializer.Deserialize<FieldDetailViewModel>(jsonContent, _jsonOptions);
                    return new ApiResponse<FieldDetailViewModel>
                    {
                        Success = true,
                        Data = fieldDetail
                    };
                }

                return new ApiResponse<FieldDetailViewModel>
                {
                    Success = false,
                    Message = $"API trả về lỗi: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<FieldDetailViewModel>
                {
                    Success = false,
                    Message = $"Lỗi khi gọi API: {ex.Message}"
                };
            }
        }
    }

    public class FieldDetailViewModel
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal PricePerHour { get; set; }
        public string PriceDisplay => $"{PricePerHour:N0} VNĐ";
        public string Size { get; set; } = string.Empty;
        public string GrassType { get; set; } = string.Empty;
        public string Lighting { get; set; } = string.Empty;
        public string Quality { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
        public string MainImage { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public List<string> Facilities { get; set; } = new();
        public string RegularPrice => $"{PricePerHour:N0} VNĐ";
        public string PeakPrice => $"{PricePerHour * 1.2m:N0} VNĐ";
        public string WeekendPrice => $"{PricePerHour * 1.5m:N0} VNĐ";
    }
}