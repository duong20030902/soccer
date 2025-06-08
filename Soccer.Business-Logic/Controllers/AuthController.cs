using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.Services;
using Soccer.Data_Access.Models;
using Soccer.Data_Access.Models.AuthModel;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Soccer.Business_Logic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SoccerContext _context;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(SoccerContext context, EmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        private string GenerateJwtToken(Soccer.Data_Access.Models.User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
                return Unauthorized("Email hoặc mật khẩu không đúng");

            if (!user.IsActive)
                return Unauthorized("Vui lòng xác nhận email trước khi đăng nhập");

            var token = GenerateJwtToken(user);

            // Lưu vào session
            HttpContext.Session.SetString("JwtToken", token);
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);

            return Ok(new
            {
                token = token,
                user = new { user.UserId, user.FullName, user.Email, user.RoleId },
                expiresAt = DateTime.UtcNow.AddHours(24)
            });
        }

        [HttpGet("get-token")]
        public IActionResult GetTokenFromSession()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var userId = HttpContext.Session.GetInt32("UserId");
            var userName = HttpContext.Session.GetString("UserName");

            Console.WriteLine($"GetToken called. Session ID: {HttpContext.Session.Id}");
            Console.WriteLine($"Session Data: JwtToken={token ?? "null"}, UserId={userId ?? 0}");

            if (string.IsNullOrEmpty(token) || userId == null)
            {
                return NotFound("Không tìm thấy token trong session. Vui lòng đăng nhập lại.");
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim != userId.ToString())
                {
                    return NotFound("Token không khớp với UserId. Vui lòng đăng nhập lại.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Token validation error: " + ex.Message);
                return NotFound("Token không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại.");
            }

            return Ok(new
            {
                token = token,
                userId = userId,
                userName = userName ?? "Người dùng",
                expiresAt = DateTime.UtcNow.AddHours(24)
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
                return BadRequest("Email đã tồn tại");

            var confirmationToken = Guid.NewGuid().ToString();

            var user = new Soccer.Data_Access.Models.User
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                RoleId = 2,
                PasswordHash = HashPassword(model.Password),
                Balance = 0,
                IsActive = true,
                EmailConfirmationToken = confirmationToken,
                ConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            /*var confirmationLink = $"https://localhost:7237/api/Auth/confirm-email?token={confirmationToken}";

            await _emailService.SendConfirmationEmail(model.Email, confirmationLink);*/

            return Ok(new
            {
                message = "Vui lòng kiểm tra email để xác nhận tài khoản",
                userId = user.UserId
            });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.EmailConfirmationToken == token &&
                u.ConfirmationTokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                return Redirect("https://localhost:7170/Auth/Login?error=invalid_token");
            }

            user.IsActive = true;
            user.EmailConfirmationToken = null;
            user.ConfirmationTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Redirect("https://localhost:7170/Auth/Login?verified=true");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    return Ok(new { message = "Nếu email tồn tại trong hệ thống, bạn sẽ nhận được hướng dẫn đặt lại mật khẩu." });
                }

                var resetToken = Guid.NewGuid().ToString();
                user.PasswordResetToken = resetToken;
                user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(24);

                await _context.SaveChangesAsync();

                var resetLink = $"https://localhost:7170/ForgotPW/ResetPassword?token={resetToken}";
                try
                {
                    await _emailService.SendPasswordResetEmail(model.Email, resetLink);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email sending failed: {ex.Message}");
                    return StatusCode(500, new { message = "Lỗi khi gửi email. Vui lòng thử lại sau." });
                }

                return Ok(new { message = "Nếu email tồn tại trong hệ thống, bạn sẽ nhận được hướng dẫn đặt lại mật khẩu.", token = resetToken });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ForgotPassword: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi server. Vui lòng thử lại sau." });
            }
        }

        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PasswordResetToken == token &&
                u.PasswordResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
                return BadRequest("Token không hợp lệ hoặc đã hết hạn");

            return Ok(new { valid = true, token });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PasswordResetToken == model.Token &&
                u.PasswordResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
                return BadRequest("Token không hợp lệ hoặc đã hết hạn");

            user.PasswordHash = HashPassword(model.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            var jwtToken = GenerateJwtToken(user);

            // Lưu token vào session (thay vì cookie)
            HttpContext.Session.SetString("JwtToken", jwtToken);
            HttpContext.Session.SetInt32("UserId", user.UserId);

            return Ok(new
            {
                message = "Đặt lại mật khẩu thành công. Bạn đã được đăng nhập tự động",
                token = jwtToken,
                user = new
                {
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.RoleId
                },
                expiresAt = DateTime.UtcNow.AddHours(24)
            });
        }

        [HttpGet("check-email-confirmation")]
        public async Task<IActionResult> CheckEmailConfirmation(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return NotFound("Email không tồn tại");

            return Ok(new { isConfirmed = user.IsActive });
        }

        private byte[] HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassword(string password, byte[] hash)
        {
            var hashInput = HashPassword(password);
            return hash.SequenceEqual(hashInput);
        }

        [HttpGet("validate-token")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                isValid = true,
                user = new
                {
                    userId = userId,
                    fullName = userName,
                    email = userEmail,
                    roleId = userRole
                }
            });
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetUserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var fullName = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var roleId = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            return Ok(new
            {
                UserId = userId,
                FullName = fullName,
                Email = email,
                RoleId = roleId
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                var token = HttpContext.Session.GetString("JwtToken");
                var userId = HttpContext.Session.GetInt32("UserId");
                var userName = HttpContext.Session.GetString("UserName");

                Console.WriteLine($"Logging out user: {userName} (ID: {userId}), Session ID: {HttpContext.Session.Id}");

                // Xóa tất cả dữ liệu session
                HttpContext.Session.Clear();

                return Ok(new
                {
                    message = "Đăng xuất thành công",
                    success = true,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");
                return StatusCode(500, new
                {
                    message = "Lỗi khi đăng xuất. Vui lòng thử lại.",
                    success = false
                });
            }
        }
    }
}