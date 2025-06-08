using Soccer.Font_end.ViewModels;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Soccer.Font_end.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7237/api";
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse> LoginAsync(LoginViewModel model)
        {
            try
            {
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

                Console.WriteLine($"Login request: {JsonConvert.SerializeObject(loginData)}");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"API Response Status: {response.StatusCode}");
                Console.WriteLine($"API Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    // Kiểm tra xem response có phải là JSON không
                    if (string.IsNullOrEmpty(responseContent) || !responseContent.Trim().StartsWith("{"))
                    {
                        return new ApiResponse
                        {
                            Success = false,
                            Message = "Server trả về dữ liệu không hợp lệ"
                        };
                    }

                    try
                    {
                        // Thử parse để kiểm tra JSON hợp lệ
                        var testParse = JsonConvert.DeserializeObject(responseContent);

                        // Lưu token nếu có
                        dynamic loginResponse = JsonConvert.DeserializeObject(responseContent);
                        if (loginResponse?.token != null)
                        {
                            string token = loginResponse.token.ToString();
                            _httpContextAccessor.HttpContext.Session.SetString("JwtToken", token);
                        }

                        _httpContextAccessor.HttpContext.Session.SetString("UserData", responseContent);

                        return new ApiResponse
                        {
                            Success = true,
                            Message = "Đăng nhập thành công!",
                            Data = loginResponse // Trả về object đã parse, không phải string
                        };
                    }
                    catch (JsonException)
                    {
                        return new ApiResponse
                        {
                            Success = false,
                            Message = "Dữ liệu từ server không đúng định dạng"
                        };
                    }
                }
                else
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = response.StatusCode == System.Net.HttpStatusCode.Unauthorized ?
                                 "Email hoặc mật khẩu không đúng" : responseContent
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login Exception: {ex.Message}");
                return new ApiResponse
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                var registerData = new
                {
                    FullName = model.FullName,
                    Email = model.RegisterEmail,
                    Phone = model.PhoneNumber,
                    Password = model.RegisterPassword
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(registerData),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Auth/register", content);
                var responseContent = await response.Content.ReadAsStringAsync();

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
                        Message = responseContent
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

        // Phương thức để gọi API với JWT token
        public async Task<string> CallProtectedApiAsync(string endpoint)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Không tìm thấy token");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{endpoint}");
            return await response.Content.ReadAsStringAsync();
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext.Session.Remove("JwtToken");
            _httpContextAccessor.HttpContext.Session.Remove("UserData");
            _httpContextAccessor.HttpContext.Session.Clear();
        }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public UserInfo User { get; set; }
    }

    public class UserInfo
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
    }
}
