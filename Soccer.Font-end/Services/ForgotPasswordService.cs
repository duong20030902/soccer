using Soccer.Font_end.ViewModels;
using System.Text.Json;
using System.Text;
using System;

namespace Soccer.Font_end.Services
{
    public class ForgotPasswordService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public ForgotPasswordService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7237/api";
        }

        public async Task<(string Message, string Token)> SendResetLinkAsync(string email)
        {
            var model = new { Email = email };
            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Auth/forgot-password", content);
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"SendResetLink Response Status: {response.StatusCode}");
            Console.WriteLine($"SendResetLink Response Content: {result}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(result) || result.TrimStart().StartsWith("<"))
                    {
                        return ("Phản hồi không hợp lệ từ server (không phải JSON).", null);
                    }

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var responseObj = JsonSerializer.Deserialize<Dictionary<string, string>>(result, options);
                    var message = responseObj?.ContainsKey("message") == true ? responseObj["message"] : "Success";
                    var token = responseObj?.ContainsKey("token") == true ? responseObj["token"] : null;
                    return (message, token);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON Error: {ex.Message}");
                    return ($"Phản hồi không hợp lệ từ server: {result}", null);
                }
            }

            return ($"Lỗi {response.StatusCode}: {result}", null);
        }

        public class TokenValidationResponse
        {
            public bool Valid { get; set; }
            public string Token { get; set; }
        }

        public async Task<bool> ValidateResetTokenAsync(string token)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Auth/reset-password?token={token}");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"ValidateResetToken Response Status: {response.StatusCode}");
            Console.WriteLine($"ValidateResetToken Response Content: {content}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<TokenValidationResponse>(content, options);
                    Console.WriteLine($"ValidateResetToken Deserialized: Valid={result.Valid}, Token={result.Token}");
                    return result.Valid;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON Error in ValidateResetToken: {ex.Message}");
                    return false;
                }
            }

            Console.WriteLine($"ValidateResetToken Failed with Status: {response.StatusCode}");
            return false;
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var resetModel = new
            {
                Token = model.Token,
                NewPassword = model.NewPassword,
                ConfirmPassword = model.ConfirmPassword
            };
            var content = new StringContent(
                JsonSerializer.Serialize(resetModel),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Auth/reset-password", content);
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"ResetPassword Response Status: {response.StatusCode}");
            Console.WriteLine($"ResetPassword Response Content: {result}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var responseObj = JsonSerializer.Deserialize<Dictionary<string, string>>(result, options);
                return responseObj?.ContainsKey("message") == true ? responseObj["message"] : "Success";
            }

            return result;
        }

        public async Task<bool> CheckEmailConfirmationAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Auth/check-email-confirmation?email={email}");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"CheckEmailConfirmation Response Status: {response.StatusCode}");
            Console.WriteLine($"CheckEmailConfirmation Response Content: {content}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<Dictionary<string, bool>>(content, options);
                    return result?.ContainsKey("isConfirmed") == true && result["isConfirmed"];
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON Error in CheckEmailConfirmation: {ex.Message}");
                    return false;
                }
            }
            return false;
        }
    }
}