using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class TimeslotsController : ControllerBase
    {
        private readonly SoccerContext _context;

        public TimeslotsController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeslotDto>>> GetTimeslots()
        {
            var timeslots = await _context.Timeslots
                .Select(t => new TimeslotDto
                {
                    TimeslotID = t.TimeslotId,
                    StartTime = t.StartTime.ToTimeSpan(),
                    EndTime = t.EndTime.ToTimeSpan()
                })
                .ToListAsync();

            return Ok(timeslots);
        }

        [HttpPost]
        public async Task<ActionResult<TimeslotDto>> CreateTimeslot(CreateTimeslotRequest request)
        {
            var timeslot = new Timeslot
            {
                StartTime = TimeOnly.FromTimeSpan(request.StartTime),
                EndTime = TimeOnly.FromTimeSpan(request.EndTime)
            };

            _context.Timeslots.Add(timeslot);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTimeslots), null,
                new TimeslotDto
                {
                    TimeslotID = timeslot.TimeslotId,
                    StartTime = timeslot.StartTime.ToTimeSpan(),
                    EndTime = timeslot.EndTime.ToTimeSpan()
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeslot(int id, UpdateTimeslotRequest request)
        {
            var timeslot = await _context.Timeslots.FindAsync(id);
            if (timeslot == null) return NotFound();

            timeslot.StartTime = TimeOnly.FromTimeSpan(request.StartTime);
            timeslot.EndTime = TimeOnly.FromTimeSpan(request.EndTime);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeslot(int id)
        {
            var timeslot = await _context.Timeslots.FindAsync(id);
            if (timeslot == null) return NotFound();

            _context.Timeslots.Remove(timeslot);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
