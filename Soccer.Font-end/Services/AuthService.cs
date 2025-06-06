using Newtonsoft.Json;
using Soccer.Font_end.ViewModels;
using System.Text;
using System.Text.Json;

namespace Soccer.Font_end.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public AuthService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7237/api";
        }

        public async Task<ApiResponse> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                // Chuyển đổi từ RegisterViewModel sang model API yêu cầu
                var registerData = new
                {
                    FullName = model.FullName,
                    Email = model.RegisterEmail,
                    Phone = model.PhoneNumber, // Lưu ý: backend sử dụng "Phone", không phải "PhoneNumber"
                    Password = model.RegisterPassword
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(registerData),
                    Encoding.UTF8,
                    "application/json"
                );

                Console.WriteLine("Register request: " + JsonConvert.SerializeObject(registerData));
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Auth/register", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Register response: " + response.StatusCode + " - " + responseContent);

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse
                    {
                        Success = true,
                        Message = "Vui lòng kiểm tra email để xác nhận tài khoản",
                        Data = responseContent
                    };
                }
                else
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = responseContent // Hiển thị lỗi từ API
                    };
                }

            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse> LoginAsync(LoginViewModel model)
        {
            try
            {
                // Chuyển đổi từ LoginViewModel sang model API yêu cầu
                var loginData = new
                {
                    Email = model.Email,
                    Password = model.Password
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(loginData),
                    Encoding.UTF8,
                    "application/json"
                );
                Console.WriteLine("login request: " + JsonConvert.SerializeObject(loginData));
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {responseContent}");
                if (response.IsSuccessStatusCode)
                {
                    var userData = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    return new ApiResponse
                    {
                        Success = true,
                        Message = "Đăng nhập thành công!",
                        Data = userData
                    };
                }
                else
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = responseContent.Contains("Unauthorized") ?
                                 "Email hoặc mật khẩu không đúng" : responseContent
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }

    public class UserSession
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
    }
}