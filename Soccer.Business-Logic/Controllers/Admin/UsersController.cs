using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;
using System.Security.Cryptography;
using System.Text;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SoccerContext _context;

        public UsersController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => new UserDto
                {
                    UserID = u.UserId,
                    RoleID = u.RoleId,
                    RoleName = u.Role.RoleName,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Balance = u.Balance,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return NotFound();

            return Ok(new UserDto
            {
                UserID = user.UserId,
                RoleID = user.RoleId,
                RoleName = user.Role.RoleName,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Balance = user.Balance,
                IsActive = user.IsActive
            });
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserRequest request)
        {
            var user = new Soccer.Data_Access.Models.User
            {
                RoleId = request.RoleID,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = HashPassword(request.Password),
                Balance = 0,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId },
                await GetUser(user.UserId));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.RoleId = request.RoleID;
            user.FullName = request.FullName;
            user.Email = request.Email;
            user.Phone = request.Phone;
            user.IsActive = request.IsActive;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/balance")]
        public async Task<IActionResult> UpdateUserBalance(int id, [FromBody] decimal amount)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Balance = amount;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private byte[] HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
