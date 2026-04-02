using Microsoft.AspNetCore.Mvc;
using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComputerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComputerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/computer
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var computers = await _context.Computers
                                          .Include(c => c.Room)
                                          .ToListAsync();

            return Ok(computers);
        }

        // GET: api/computer/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var computer = await _context.Computers
                                         .Include(c => c.Room)
                                         .FirstOrDefaultAsync(c => c.ComputerId == id);

            if (computer == null)
                return NotFound();

            return Ok(computer);
        }

        // POST: api/computer
        [HttpPost]
        public async Task<IActionResult> Create(Computer computer)
        {
            // kiểm tra Room có tồn tại không
            var roomExists = await _context.Rooms.AnyAsync(r => r.RoomId == computer.RoomId);
            if (!roomExists)
                return BadRequest("Room không tồn tại");

            _context.Computers.Add(computer);
            await _context.SaveChangesAsync();

            return Ok(computer);
        }

        // PUT: api/computer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Computer computer)
        {
            if (id != computer.ComputerId)
                return BadRequest();

            // kiểm tra Room hợp lệ
            var roomExists = await _context.Rooms.AnyAsync(r => r.RoomId == computer.RoomId);
            if (!roomExists)
                return BadRequest("Room không tồn tại");

            _context.Entry(computer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Computers.Any(c => c.ComputerId == id))
                    return NotFound();
                else
                    throw;
            }

            return Ok(computer);
        }

        // DELETE: api/computer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var computer = await _context.Computers.FindAsync(id);

            if (computer == null)
                return NotFound();

            // nếu đang sử dụng thì không cho xóa
            if (computer.Status == "Using")
                return BadRequest("Máy đang được sử dụng, không thể xóa");

            _context.Computers.Remove(computer);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}