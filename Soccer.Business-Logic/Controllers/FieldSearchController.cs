using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldSearchController : ControllerBase
    {
        private readonly SoccerContext _context;

        public FieldSearchController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FieldSearchResultDto>>> SearchFields(
            [FromQuery] DateOnly date,
            [FromQuery] int timeslotId)
        {
            try
            {
                var availableFields = await _context.FieldSchedules
                    .Include(fs => fs.Field)
                        .ThenInclude(f => f.FieldImages)
                    .Include(fs => fs.Timeslot)
                    .Where(fs => fs.Date == date &&
                                fs.TimeslotId == timeslotId &&
                                fs.Status == "Available")
                    .Select(fs => new FieldSearchResultDto
                    {
                        ScheduleId = fs.ScheduleId,
                        FieldId = fs.FieldId,
                        FieldName = fs.Field.FieldName,
                        Location = fs.Field.Location,
                        PricePerHour = fs.Field.PricePerHour,
                        Date = fs.Date,
                        StartTime = fs.Timeslot.StartTime,
                        EndTime = fs.Timeslot.EndTime,
                        Status = fs.Status,
                        ImageUrls = fs.Field.FieldImages.Select(i => i.ImageUrl).ToList()
                    })
                    .OrderBy(f => f.FieldName)
                    .ToListAsync();

                return Ok(availableFields);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi tìm kiếm sân");
            }
        }

        [HttpGet("search-by-name")]
        public async Task<ActionResult<IEnumerable<FieldSearchResultDto>>> SearchFieldsByName(
            [FromQuery] string fieldName,
            [FromQuery] DateOnly date,
            [FromQuery] int timeslotId)
        {
            try
            {
                var availableFields = await _context.FieldSchedules
                    .Include(fs => fs.Field)
                        .ThenInclude(f => f.FieldImages)
                    .Include(fs => fs.Timeslot)
                    .Where(fs => fs.Date == date &&
                                fs.TimeslotId == timeslotId &&
                                fs.Status == "Available" &&
                                fs.Field.FieldName.Contains(fieldName))
                    .Select(fs => new FieldSearchResultDto
                    {
                        ScheduleId = fs.ScheduleId,
                        FieldId = fs.FieldId,
                        FieldName = fs.Field.FieldName,
                        Location = fs.Field.Location,
                        PricePerHour = fs.Field.PricePerHour,
                        Date = fs.Date,
                        StartTime = fs.Timeslot.StartTime,
                        EndTime = fs.Timeslot.EndTime,
                        Status = fs.Status,
                        ImageUrls = fs.Field.FieldImages.Select(i => i.ImageUrl).ToList()
                    })
                    .OrderBy(f => f.FieldName)
                    .ToListAsync();

                return Ok(availableFields);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi tìm kiếm sân theo tên");
            }
        }

        [HttpGet("timeslots")]

        public async Task<ActionResult<IEnumerable<TimeslotDto>>> GetAvailableTimeslots()
        {
            try
            {
                // Lấy dữ liệu TimeOnly trước
                var timeslots = await _context.Timeslots
                    .OrderBy(t => t.StartTime)
                    .ToListAsync();

                // Chuyển đổi sang DTO sau khi đã lấy dữ liệu về bộ nhớ
                var result = timeslots.Select(t => new TimeslotDto
                {
                    TimeslotID = t.TimeslotId,
                    StartTime = t.StartTime.ToTimeSpan(),
                    EndTime = t.EndTime.ToTimeSpan()
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi lấy danh sách khung giờ");
            }
        }
        // Thêm vào FieldSearchController hoặc tạo FieldController mới
        [HttpGet("{id}")]
        public async Task<ActionResult<FieldDetailDto>> GetFieldDetails(int id)
        {
            try
            {
                var field = await _context.Fields
                    .Include(f => f.FieldImages)
                    .FirstOrDefaultAsync(f => f.FieldId == id);

                if (field == null)
                {
                    return NotFound("Không tìm thấy sân");
                }

                var result = new FieldDetailDto
                {
                    FieldId = field.FieldId,
                    FieldName = field.FieldName,
                    Location = field.Location,
                    PricePerHour = field.PricePerHour,
                    Size = "7 người", // hoặc lấy từ database
                    GrassType = "Cỏ nhân tạo", // hoặc lấy từ database
                    Lighting = "Đầy đủ", // hoặc lấy từ database
                    Quality = "Tốt", // hoặc lấy từ database
                    Description = "",
                    Images = field.FieldImages.Select(i => i.ImageUrl).ToList(),
                    MainImage = field.FieldImages.FirstOrDefault()?.ImageUrl ?? "",
                    IsAvailable = true, // Logic kiểm tra availability
                    Facilities = new List<string> { "Phòng thay đồ", "Bãi đỗ xe", "Nước uống" }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi lấy chi tiết sân");
            }
        }

        [HttpGet("search-all")]
        public async Task<ActionResult<IEnumerable<FieldSearchResultDto>>> SearchAllFields(
    [FromQuery] DateOnly date,
    [FromQuery] string? fieldName = null)
        {
            try
            {
                var query = _context.FieldSchedules
                    .Include(fs => fs.Field)
                        .ThenInclude(f => f.FieldImages)
                    .Include(fs => fs.Timeslot)
                    .Where(fs => fs.Date == date && fs.Status == "Available");

                // Nếu có tên sân, thêm điều kiện lọc
                if (!string.IsNullOrEmpty(fieldName))
                {
                    query = query.Where(fs => fs.Field.FieldName.Contains(fieldName));
                }

                var availableFields = await query
                    .Select(fs => new FieldSearchResultDto
                    {
                        ScheduleId = fs.ScheduleId,
                        FieldId = fs.FieldId,
                        FieldName = fs.Field.FieldName,
                        Location = fs.Field.Location,
                        PricePerHour = fs.Field.PricePerHour,
                        Date = fs.Date,
                        StartTime = fs.Timeslot.StartTime,
                        EndTime = fs.Timeslot.EndTime,
                        Status = fs.Status,
                        ImageUrls = fs.Field.FieldImages.Select(i => i.ImageUrl).ToList(),
                        TimeslotId = fs.TimeslotId
                    })
                    .OrderBy(f => f.StartTime)
                    .ThenBy(f => f.FieldName)
                    .ToListAsync();

                return Ok(availableFields);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra khi tìm kiếm tất cả sân");
            }
        }

    }
}
