using Soccer.Font_end.ViewModels;
using System.Text.Json;

namespace Soccer.Font_end.Areas.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserService(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = "https://localhost:7237/api";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/admin/users");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<UserDto>>(jsonString, _jsonOptions) ?? new List<UserDto>();
            }
            catch (Exception ex)
            {
                // Log exception
                return new List<UserDto>();
            }
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/admin/users/{id}");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserDto>(jsonString, _jsonOptions);
            }
            catch (Exception ex)
            {
                // Log exception
                return null;
            }
        }

        public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/admin/users", content);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserDto>(jsonString, _jsonOptions);
            }
            catch (Exception ex)
            {
                // Log exception
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/admin/users/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log exception
                return false;
            }
        }

        public async Task<bool> UpdateUserBalanceAsync(int id, decimal amount)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(amount, _jsonOptions);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/admin/users/{id}/balance", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log exception
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/admin/users/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log exception
                return false;
            }
        }
    }
}
