using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;
using System.Security.Claims;

namespace Soccer.Business_Logic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly SoccerContext _context;

        public BookingController(SoccerContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking(CreateBookingRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Kiểm tra schedule có tồn tại và available không
                var schedule = await _context.FieldSchedules
                    .Include(fs => fs.Field)
                    .Include(fs => fs.Timeslot)
                    .FirstOrDefaultAsync(fs => fs.ScheduleId == request.ScheduleId);

                if (schedule == null)
                {
                    return NotFound("Không tìm thấy lịch sân");
                }

                if (schedule.Status != "Available")
                {
                    return BadRequest("Sân này đã được đặt hoặc không khả dụng");
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized("Vui lòng đăng nhập để đặt sân");
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound("Không tìm thấy thông tin người dùng");
                }

                decimal price = schedule.Field.PricePerHour;
                decimal commission = price * 0.1m;

                var booking = new Booking
                {
                    ScheduleId = request.ScheduleId,
                    UserId = userId.Value,
                    Price = price,
                    Commission = commission,
                    Status = "Pending",
                    BookingTime = DateTime.Now,
                    Notes = request.Notes
                };

                _context.Bookings.Add(booking);

                // Cập nhật status của schedule từ Available thành Booked
                schedule.Status = "Available";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new BookingResponseDto
                {
                    BookingId = booking.BookingId,
                    FieldName = schedule.Field.FieldName,
                    UserName = user.FullName,
                    Date = schedule.Date,
                    StartTime = schedule.Timeslot.StartTime,
                    EndTime = schedule.Timeslot.EndTime,
                    Price = booking.Price,
                    Commission = booking.Commission ?? 0m,
                    Status = booking.Status,
                    BookingTime = booking.BookingTime,
                    Notes = booking.Notes
                };

                return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Có lỗi xảy ra khi đặt sân. Vui lòng thử lại sau.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponseDto>> GetBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Field)
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Timeslot)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.BookingId == id);

                if (booking == null)
                {
                    return NotFound("Không tìm thấy booking");
                }

                var response = new BookingResponseDto
                {
                    BookingId = booking.BookingId,
                    FieldName = booking.Schedule.Field.FieldName,
                    UserName = booking.User.FullName,
                    Date = booking.Schedule.Date,
                    StartTime = booking.Schedule.Timeslot.StartTime,
                    EndTime = booking.Schedule.Timeslot.EndTime,
                    Price = booking.Price,
                    Commission = booking.Commission ?? 0m,
                    Status = booking.Status,
                    BookingTime = booking.BookingTime,
                    Notes = booking.Notes
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi lấy thông tin booking");
            }
        }

        [HttpGet("my-bookings")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetMyBookings(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized("Vui lòng đăng nhập");
                }

                var query = _context.Bookings
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Field)
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Timeslot)
                    .Include(b => b.User)
                    .Where(b => b.UserId == userId.Value);

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(b => b.Status == status);
                }

                var bookings = await query
                    .OrderByDescending(b => b.BookingTime)
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
                return StatusCode(500, "Có lỗi xảy ra khi lấy danh sách booking");
            }
        }

        // THÊM MỚI: API để cập nhật status booking với logic tự động sync FieldSchedule
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var validStatuses = new[] { "Pending", "Confirmed", "Cancelled", "Completed" };
                if (!validStatuses.Contains(request.Status))
                {
                    return BadRequest("Trạng thái không hợp lệ");
                }

                var booking = await _context.Bookings
                    .Include(b => b.Schedule)
                    .FirstOrDefaultAsync(b => b.BookingId == id);

                if (booking == null)
                {
                    return NotFound("Không tìm thấy booking");
                }

                // Lưu trạng thái cũ để so sánh
                var oldStatus = booking.Status;

                // Cập nhật status booking
                booking.Status = request.Status;

                // Logic tự động cập nhật FieldSchedule status
                await UpdateFieldScheduleStatus(booking, oldStatus, request.Status);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok("Cập nhật trạng thái booking thành công");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Có lỗi xảy ra khi cập nhật trạng thái booking");
            }
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Debug: Kiểm tra booking có tồn tại không
                var bookingExists = await _context.Bookings.AnyAsync(b => b.BookingId == id);
                if (!bookingExists)
                {
                    return NotFound($"Không tìm thấy booking với ID: {id}");
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized("Vui lòng đăng nhập");
                }

                // Debug: Lấy booking để kiểm tra thông tin
                var booking = await _context.Bookings
                    .Include(b => b.Schedule)
                    .FirstOrDefaultAsync(b => b.BookingId == id);

                if (booking == null)
                {
                    return NotFound($"Không tìm thấy booking với ID: {id}");
                }

                // SỬA LỖI: Thay Forbid() bằng Unauthorized() hoặc BadRequest()
                if (booking.UserId != userId.Value)
                {
                    // Kiểm tra xem user hiện tại có phải admin không
                    var currentUser = await _context.Users.FindAsync(userId.Value);
                    if (currentUser == null || currentUser.RoleId != 1)
                    {
                        return StatusCode(403, new
                        {
                            message = "Bạn không có quyền hủy booking này",
                            bookingUserId = booking.UserId,
                            currentUserId = userId.Value,
                            userRole = currentUser?.RoleId
                        });
                    }
                }

                if (booking.Status == "Cancelled")
                {
                    return BadRequest("Booking đã được hủy trước đó");
                }

                if (booking.Status == "Completed")
                {
                    return BadRequest("Không thể hủy booking đã hoàn thành");
                }

                var oldStatus = booking.Status;
                booking.Status = "Cancelled";

                // Tự động cập nhật FieldSchedule status
                await UpdateFieldScheduleStatus(booking, oldStatus, "Cancelled");

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Hủy booking thành công" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Có lỗi xảy ra khi hủy booking", error = ex.Message });
            }
        }



        // THÊM MỚI: Helper method để tự động cập nhật FieldSchedule status
        private async Task UpdateFieldScheduleStatus(Booking booking, string oldStatus, string newStatus)
        {
            // Logic cập nhật FieldSchedule status dựa trên booking status
            if (newStatus == "Confirmed")
            {
                // Nếu booking được confirm -> FieldSchedule = Booked
                booking.Schedule.Status = "Booked";
            }
            else if (newStatus == "Cancelled" || newStatus == "Pending" || newStatus == "Completed")
            {
                // Nếu booking bị cancel, pending, hoặc completed -> kiểm tra xem có booking khác confirmed không
                var hasOtherConfirmedBooking = await _context.Bookings
                    .AnyAsync(b => b.ScheduleId == booking.ScheduleId &&
                                  b.BookingId != booking.BookingId &&
                                  b.Status == "Confirmed");

                if (!hasOtherConfirmedBooking)
                {
                    // Nếu không có booking nào khác được confirmed -> FieldSchedule = Available
                    booking.Schedule.Status = "Available";
                }
                // Nếu vẫn có booking khác confirmed -> giữ nguyên status Booked
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            // Hardcode cho test
            return 1;
        }
    }

    // THÊM MỚI: Request model cho update status
    public class UpdateBookingStatusRequest
    {
        public string Status { get; set; } = null!;
    }
}
