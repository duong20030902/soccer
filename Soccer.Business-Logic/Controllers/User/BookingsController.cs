using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.User
{
    [Route("api/user/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly SoccerContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookingsController(SoccerContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Tạo booking mới
        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] CreateBookingRequest request)
        {
            try
            {
                // Validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Lấy UserId từ token 
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized("Vui lòng đăng nhập để đặt sân");
                }

                // Kiểm tra user có tồn tại và active không
                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null || !user.IsActive)
                {
                    return BadRequest("Tài khoản không hợp lệ hoặc đã bị khóa");
                }

                // Kiểm tra schedule có tồn tại và available không
                var schedule = await _context.FieldSchedules
                    .Include(s => s.Field)
                    .Include(s => s.Timeslot)
                    .FirstOrDefaultAsync(s => s.ScheduleId == request.ScheduleId);

                if (schedule == null)
                {
                    return NotFound("Không tìm thấy lịch sân");
                }

                if (schedule.Status != "Available")
                {
                    return BadRequest("Lịch sân này không khả dụng");
                }

                // Kiểm tra xem schedule đã được book chưa
                var existingBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.ScheduleId == request.ScheduleId &&
                                             b.Status != "Cancelled");

                if (existingBooking != null)
                {
                    return BadRequest("Lịch sân này đã được đặt");
                }

                // Kiểm tra xem schedule có trong quá khứ không
                var scheduleDateTime = schedule.Date.ToDateTime(schedule.Timeslot.StartTime);
                if (scheduleDateTime <= DateTime.Now)
                {
                    return BadRequest("Không thể đặt sân trong quá khứ");
                }

                // Tính toán giá và commission
                var duration = schedule.Timeslot.EndTime - schedule.Timeslot.StartTime;
                var price = schedule.Field.PricePerHour * (decimal)duration.TotalHours;

                // Kiểm tra số dư tài khoản
                if (user.Balance < price)
                {
                    return BadRequest("Số dư tài khoản không đủ để đặt sân");
                }

                // Sử dụng transaction để đảm bảo data consistency
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Tạo booking mới
                    var booking = new Booking
                    {
                        ScheduleId = request.ScheduleId,
                        UserId = userId.Value,
                        BookingTime = DateTime.Now,
                        Price = price,
                        Status = "Pending", // Trạng thái chờ xác nhận
                        Notes = request.Notes
                    };

                    _context.Bookings.Add(booking);

                    // Cập nhật trạng thái schedule
                    schedule.Status = "Booked";

                    // Trừ tiền từ tài khoản user (tùy thuộc vào business logic)
                    // user.Balance -= price;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Lấy thông tin booking vừa tạo để trả về
                    var createdBooking = await GetBookingDtoById(booking.BookingId);

                    return CreatedAtAction(nameof(GetBookingById),
                        new { id = booking.BookingId }, createdBooking);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi tạo booking: " + ex.Message);
            }
        }

        // Lấy booking của user hiện tại
        [HttpGet("my-bookings")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetMyBookings(
            [FromQuery] string? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validation
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                // Lấy UserId từ token
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized("Vui lòng đăng nhập để xem booking");
                }

                var query = _context.Bookings
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Field)
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Timeslot)
                    .Include(b => b.User)
                    .Where(b => b.UserId == userId.Value);

                // Filter theo status nếu có
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(b => b.Status.ToLower() == status.ToLower());
                }

                // Sắp xếp theo thời gian booking mới nhất
                query = query.OrderByDescending(b => b.BookingTime);

                // Pagination
                var bookings = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => new BookingResponseDto
                    {
                        BookingId = b.BookingId,
                        FieldName = b.Schedule.Field.FieldName,
                        UserName = b.User.FullName,
                        Date = b.Schedule.Date,
                        StartTime = b.Schedule.Timeslot.StartTime,
                        EndTime = b.Schedule.Timeslot.EndTime,
                        Price = b.Price,
                        Commission = b.Commission ?? 0m,
                        Status = b.Status,
                        BookingTime = b.BookingTime,
                        Notes = b.Notes
                    })
                    .ToListAsync();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi lấy danh sách booking: " + ex.Message);
            }
        }

        // Lấy booking theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponseDto>> GetBookingById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var booking = await GetBookingDtoById(id, userId.Value);
                if (booking == null)
                {
                    return NotFound("Không tìm thấy booking");
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi lấy thông tin booking: " + ex.Message);
            }
        }

        // Hủy booking (chỉ cho phép user hủy booking của mình)
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelMyBooking(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var booking = await _context.Bookings
                    .Include(b => b.Schedule)
                    .FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId.Value);

                if (booking == null)
                {
                    return NotFound("Không tìm thấy booking");
                }

                if (booking.Status == "Cancelled")
                {
                    return BadRequest("Booking này đã được hủy");
                }

                if (booking.Status == "Completed")
                {
                    return BadRequest("Không thể hủy booking đã hoàn thành");
                }

                // Kiểm tra thời gian hủy (ví dụ: chỉ cho phép hủy trước 2 giờ)
                var scheduleDateTime = booking.Schedule.Date.ToDateTime(booking.Schedule.Timeslot.StartTime);
                if (scheduleDateTime <= DateTime.Now.AddHours(2))
                {
                    return BadRequest("Không thể hủy booking trong vòng 2 giờ trước giờ đặt");
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    booking.Status = "Cancelled";
                    booking.Schedule.Status = "Available"; // Trả lại trạng thái available cho schedule

                    // Hoàn tiền nếu cần
                    // var user = await _context.Users.FindAsync(userId.Value);
                    // user.Balance += booking.Price;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok("Hủy booking thành công");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi hủy booking: " + ex.Message);
            }
        }

        // Helper methods
        private int? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

        private async Task<BookingResponseDto?> GetBookingDtoById(int bookingId, int? userId = null)
        {
            var query = _context.Bookings
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Field)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Timeslot)
                .Include(b => b.User)
                .Where(b => b.BookingId == bookingId);

            if (userId.HasValue)
            {
                query = query.Where(b => b.UserId == userId.Value);
            }

            return await query
                .Select(b => new BookingResponseDto
                {
                    BookingId = b.BookingId,
                    FieldName = b.Schedule.Field.FieldName,
                    UserName = b.User.FullName,
                    Date = b.Schedule.Date,
                    StartTime = b.Schedule.Timeslot.StartTime,
                    EndTime = b.Schedule.Timeslot.EndTime,
                    Price = b.Price,
                    Commission = b.Commission ?? 0m,
                    Status = b.Status,
                    BookingTime = b.BookingTime,
                    Notes = b.Notes
                })
                .FirstOrDefaultAsync();
        }
    }
}
