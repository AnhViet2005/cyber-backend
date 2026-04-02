using Microsoft.AspNetCore.Mvc;
using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/room
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _context.Rooms
                                      .Include(r => r.Computers)
                                      .ToListAsync();

            return Ok(rooms);
        }

        // GET: api/room/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _context.Rooms
                                     .Include(r => r.Computers)
                                     .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return NotFound();

            return Ok(room);
        }

        // POST: api/room
        [HttpPost]
        public async Task<IActionResult> Create(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return Ok(room);
        }

        // PUT: api/room/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Room room)
        {
            if (id != room.RoomId)
                return BadRequest();

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Rooms.Any(r => r.RoomId == id))
                    return NotFound();
                else
                    throw;
            }

            return Ok(room);
        }

        // DELETE: api/room/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms
                                     .Include(r => r.Computers)
                                     .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return NotFound();

            // ❗ Không cho xóa nếu còn máy
            if (room.Computers != null && room.Computers.Count > 0)
                return BadRequest("Phòng vẫn còn máy, không thể xóa");

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}