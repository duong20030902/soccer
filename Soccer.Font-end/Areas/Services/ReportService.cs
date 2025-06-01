using Soccer.Font_end.ViewModels;
using System.Text.Json;

namespace Soccer.Font_end.Areas.Services
{
    public class ReportService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public ReportService(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = "https://localhost:7237/api";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Get Dashboard Data
        public async Task<DashboardDto?> GetDashboardAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/admin/reports/dashboard");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<DashboardDto>(jsonContent, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log exception here
                Console.WriteLine($"Error getting dashboard: {ex.Message}");
                return null;
            }
        }

        // Get Sales Report
        public async Task<SalesReportDto?> GetSalesReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var queryString = "";
                if (startDate.HasValue || endDate.HasValue)
                {
                    var queryParams = new List<string>();
                    if (startDate.HasValue)
                        queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                    if (endDate.HasValue)
                        queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

                    queryString = "?" + string.Join("&", queryParams);
                }

                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/admin/reports/sales-report{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<SalesReportDto>(jsonContent, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting sales report: {ex.Message}");
                return null;
            }
        }

        // Get Inventory Report
        public async Task<List<InventoryReportDto>?> GetInventoryReportAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/admin/reports/inventory-report");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<InventoryReportDto>>(jsonContent, _jsonOptions);
                }

                return new List<InventoryReportDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting inventory report: {ex.Message}");
                return new List<InventoryReportDto>();
            }
        }

        // Dispose HttpClient properly
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
