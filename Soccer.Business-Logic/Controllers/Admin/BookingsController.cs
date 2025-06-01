using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly SoccerContext _context;

        public BookingsController(SoccerContext context)
        {
            _context = context;
        }

        // Lấy tất cả bookings (cho admin)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? fieldName = null,
            [FromQuery] string? userName = null)
        {
            try
            {
                // Validation
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var query = _context.Bookings
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Field)
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Timeslot)
                    .Include(b => b.User)
                    .AsQueryable();

                // Filters
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(b => b.Status.ToLower() == status.ToLower());
                }

                if (!string.IsNullOrEmpty(fieldName))
                {
                    query = query.Where(b => b.Schedule.Field.FieldName.Contains(fieldName));
                }

                if (!string.IsNullOrEmpty(userName))
                {
                    query = query.Where(b => b.User.FullName.Contains(userName));
                }

                // Sắp xếp theo thời gian booking mới nhất
                query = query.OrderByDescending(b => b.BookingTime);

                var totalCount = await query.CountAsync();

                var bookings = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => new BookingDto
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

                // Trả về với pagination info
                var result = new
                {
                    data = bookings,
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi lấy danh sách booking: " + ex.Message);
            }
        }

        // Lấy booking theo ID (cho admin)
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBookingById(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Field)
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Timeslot)
                    .Include(b => b.User)
                    .Where(b => b.BookingId == id)
                    .Select(b => new BookingDto
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

        // Cập nhật trạng thái booking
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var validStatuses = new[] { "Pending", "Confirmed", "Cancelled", "Completed" };
                if (!validStatuses.Contains(request.Status))
                {
                    return BadRequest("Trạng thái không hợp lệ. Các trạng thái hợp lệ: " + string.Join(", ", validStatuses));
                }

                var booking = await _context.Bookings
                    .Include(b => b.Schedule)
                    .FirstOrDefaultAsync(b => b.BookingId == id);

                if (booking == null)
                {
                    return NotFound("Không tìm thấy booking");
                }

                var oldStatus = booking.Status;
                booking.Status = request.Status;

                // Cập nhật trạng thái schedule tương ứng
                if (request.Status == "Cancelled")
                {
                    booking.Schedule.Status = "Available";
                }
                else if (request.Status == "Confirmed")
                {
                    booking.Schedule.Status = "Booked";
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Cập nhật trạng thái thành công",
                    oldStatus,
                    newStatus = request.Status
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi cập nhật trạng thái: " + ex.Message);
            }
        }

        // Hủy booking (admin)
        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Schedule)
                    .FirstOrDefaultAsync(b => b.BookingId == id);

                if (booking == null)
                {
                    return NotFound("Không tìm thấy booking");
                }

                if (booking.Status == "Cancelled")
                {
                    return BadRequest("Booking này đã được hủy");
                }

                booking.Status = "Cancelled";
                booking.Schedule.Status = "Available";

                await _context.SaveChangesAsync();

                return Ok("Hủy booking thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi hủy booking: " + ex.Message);
            }
        }

        // Thống kê booking
        [HttpGet("statistics")]
        public async Task<ActionResult> GetBookingStatistics()
        {
            try
            {
                var stats = await _context.Bookings
                    .GroupBy(b => b.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                var totalRevenue = await _context.Bookings
                    .Where(b => b.Status == "Completed")
                    .SumAsync(b => b.Price);

                var totalCommission = await _context.Bookings
                    .Where(b => b.Status == "Completed")
                    .SumAsync(b => b.Commission ?? 0);

                return Ok(new
                {
                    statusStats = stats,
                    totalRevenue,
                    totalCommission
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi lấy thống kê: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] CreateBookingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var schedule = await _context.FieldSchedules
                    .Include(s => s.Field)
                    .Include(s => s.Timeslot)
                    .FirstOrDefaultAsync(s => s.ScheduleId == request.ScheduleId && s.Status == "Available");

                if (schedule == null)
                {
                    return BadRequest("Lịch không khả dụng hoặc không tồn tại");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId && u.IsActive);
                if (user == null)
                {
                    return BadRequest("Người dùng không tồn tại hoặc không hoạt động");
                }

                var booking = new Booking
                {
                    ScheduleId = request.ScheduleId,
                    UserId = request.UserId, 
                    Price = schedule.Field.PricePerHour,
                    Commission = schedule.Field.PricePerHour * 0.1m, 
                    Status = "Pending",
                    BookingTime = DateTime.UtcNow,
                    Notes = request.Notes
                };

                schedule.Status = "Booked";
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                var response = new BookingDto
                {
                    BookingId = booking.BookingId,
                    FieldName = schedule.Field.FieldName,
                    UserName = (await _context.Users.FindAsync(booking.UserId))?.FullName ?? "Unknown",
                    Date = schedule.Date,
                    StartTime = schedule.Timeslot.StartTime,
                    EndTime = schedule.Timeslot.EndTime,
                    Price = booking.Price,
                    Commission = booking.Commission ?? 0m,
                    Status = booking.Status,
                    BookingTime = booking.BookingTime,
                    Notes = booking.Notes
                };

                return CreatedAtAction(nameof(GetBookingById), new { id = booking.BookingId }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi tạo booking: {ex.Message}");
            }
        }
    }
}
