using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Data_Access.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Soccer.Business_Logic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SoccerContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(SoccerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("Invalid user ID or token invalid.");
                }

                var user = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Addresses)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var userProfile = new
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Balance = user.Balance,
                    IsActive = user.IsActive,
                    RoleName = user.Role?.RoleName,
                    Addresses = user.Addresses.Select(a => new
                    {
                        AddressId = a.AddressId,
                        RecipientName = a.RecipientName,
                        StreetAddress = a.StreetAddress,
                        CityProvince = a.CityProvince,
                        PostalCode = a.PostalCode
                    }).ToList()
                };

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCurrentUserProfile: {ex.Message}");
                return StatusCode(500, new { message = "Server error occurred" });
            }
        }

        [HttpPut("update/profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            try
            {
                // Log request body for debugging
                Console.WriteLine($"UpdateProfile request body: {JsonConvert.SerializeObject(model)}");

                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("Invalid user ID or token invalid.");
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Validate and update user details
                if (model.FullName != null && !string.IsNullOrWhiteSpace(model.FullName))
                {
                    if (model.FullName.Trim() == "string")
                    {
                        return BadRequest(new { message = "Họ tên không được là giá trị mặc định 'string'." });
                    }
                    user.FullName = model.FullName.Trim();
                }

                if (model.Email != null && !string.IsNullOrWhiteSpace(model.Email))
                {
                    if (model.Email.Trim() == "string" || !new EmailAddressAttribute().IsValid(model.Email))
                    {
                        return BadRequest(new { message = "Email không hợp lệ." });
                    }
                    if (await _context.Users.AnyAsync(u => u.Email == model.Email && u.UserId != userId))
                    {
                        return BadRequest(new { message = "Email đã được sử dụng bởi người dùng khác." });
                    }
                    user.Email = model.Email.Trim();
                }

                if (model.Phone != null && !string.IsNullOrWhiteSpace(model.Phone))
                {
                    if (model.Phone.Trim() == "string")
                    {
                        return BadRequest(new { message = "Số điện thoại không được là giá trị mặc định 'string'." });
                    }
                    if (!System.Text.RegularExpressions.Regex.IsMatch(model.Phone, @"^\+?\d{10,}$"))
                    {
                        return BadRequest(new { message = "Số điện thoại phải có ít nhất 10 chữ số." });
                    }
                    user.Phone = model.Phone.Trim();
                }

                if (model.FullName == null && model.Email == null && model.Phone == null)
                {
                    return BadRequest(new { message = "Không có thông tin nào được cung cấp để cập nhật." });
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Cập nhật hồ sơ thành công." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateProfile: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi server khi cập nhật hồ sơ." });
            }
        }

        [HttpPut("address")]
        [Authorize]
        public async Task<IActionResult> UpdateAddress([FromBody] List<AddressModel> addresses)
        {
            try
            {
                // Log request body for debugging
                Console.WriteLine($"UpdateAddress request body: {JsonConvert.SerializeObject(addresses)}");

                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("Invalid user ID or token invalid.");
                }

                var user = await _context.Users
                    .Include(u => u.Addresses)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                if (addresses == null || !addresses.Any())
                {
                    return BadRequest(new { message = "Không có địa chỉ nào được cung cấp." });
                }

                foreach (var address in addresses)
                {
                    if (string.IsNullOrWhiteSpace(address.RecipientName) ||
                        string.IsNullOrWhiteSpace(address.StreetAddress) ||
                        string.IsNullOrWhiteSpace(address.CityProvince) ||
                        string.IsNullOrWhiteSpace(address.PostalCode) ||
                        address.RecipientName?.Trim() == "string" ||
                        address.StreetAddress?.Trim() == "string" ||
                        address.CityProvince?.Trim() == "string" ||
                        address.PostalCode?.Trim() == "string")
                    {
                        return BadRequest(new { message = "Thông tin địa chỉ không đầy đủ hoặc chứa giá trị không hợp lệ." });
                    }

                    if (address.AddressId > 0)
                    {
                        var existingAddress = user.Addresses.FirstOrDefault(a => a.AddressId == address.AddressId);
                        if (existingAddress != null)
                        {
                            if (existingAddress.UserId != userId)
                            {
                                return Unauthorized("Không có quyền chỉnh sửa địa chỉ này.");
                            }
                            existingAddress.RecipientName = address.RecipientName.Trim();
                            existingAddress.StreetAddress = address.StreetAddress.Trim();
                            existingAddress.CityProvince = address.CityProvince.Trim();
                            existingAddress.PostalCode = address.PostalCode.Trim();
                        }
                        else
                        {
                            return BadRequest(new { message = $"Địa chỉ với ID {address.AddressId} không tồn tại." });
                        }
                    }
                    else
                    {
                        user.Addresses.Add(new Address
                        {
                            UserId = userId.Value,
                            RecipientName = address.RecipientName.Trim(),
                            StreetAddress = address.StreetAddress.Trim(),
                            CityProvince = address.CityProvince.Trim(),
                            PostalCode = address.PostalCode.Trim()
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Cập nhật địa chỉ thành công." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAddress: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi server khi cập nhật địa chỉ." });
            }
        }

        [HttpDelete("profile/address/{addressId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("Invalid user ID or token invalid.");
                }

                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);

                if (address == null)
                {
                    return NotFound("Address not found or not owned by user.");
                }

                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Xóa địa chỉ thành công." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAddress: {ex.Message}");
                return StatusCode(500, new { message = "Server error occurred" });
            }
        }

        private int? GetUserIdFromToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                return null;
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
                if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
                {
                    return userId;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Token validation error in GetUserIdFromToken: " + ex.Message);
                return null;
            }
        }
    }

    public class UpdateProfileModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class AddressModel
    {
        public int AddressId { get; set; }
        public string RecipientName { get; set; }
        public string StreetAddress { get; set; }
        public string CityProvince { get; set; }
        public string PostalCode { get; set; }
    }
}
