using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly SoccerContext _context;

        public SchedulesController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetSchedules([FromQuery] string? status = null)
        {
            try
            {
                var query = _context.FieldSchedules
                    .Include(s => s.Field)
                    .Include(s => s.Timeslot)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(s => s.Status.ToLower() == status.ToLower());
                }

                var schedules = await query
                    .Select(s => new ScheduleDto
                    {
                        ScheduleId = s.ScheduleId,
                        FieldId = s.FieldId,
                        FieldName = s.Field.FieldName,
                        Date = s.Date,
                        TimeslotId = s.TimeslotId,
                        StartTime = s.Timeslot.StartTime,
                        EndTime = s.Timeslot.EndTime,
                        Status = s.Status
                    })
                    .ToListAsync();

                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi lấy danh sách lịch: {ex.Message}");
            }
        }
    }
}
