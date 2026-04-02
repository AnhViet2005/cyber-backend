using Microsoft.AspNetCore.Mvc;
using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SessionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/session
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sessions = await _context.Sessions
                                         .Include(s => s.Customer)
                                         .Include(s => s.Computer)
                                         .ToListAsync();

            return Ok(sessions);
        }

        // GET: api/session/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var session = await _context.Sessions
                                        .Include(s => s.Customer)
                                        .Include(s => s.Computer)
                                        .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null)
                return NotFound();

            return Ok(session);
        }

        // 🔥 START SESSION (bắt đầu chơi)
        [HttpPost("start")]
        public async Task<IActionResult> StartSession(Session session)
        {
            // check customer
            var customer = await _context.Customers.FindAsync(session.CustomerId);
            if (customer == null)
                return BadRequest("Customer không tồn tại");

            // check computer
            var computer = await _context.Computers.FindAsync(session.ComputerId);
            if (computer == null)
                return BadRequest("Computer không tồn tại");

            // ❗ kiểm tra máy đang dùng
            if (computer.Status == "Using")
                return BadRequest("Máy đang được sử dụng");

            session.StartTime = DateTime.Now;
            session.Status = "Playing";

            _context.Sessions.Add(session);

            // update trạng thái máy
            computer.Status = "Using";

            await _context.SaveChangesAsync();

            return Ok(session);
        }

        // 🔥 END SESSION (kết thúc chơi)
        [HttpPut("end/{id}")]
        public async Task<IActionResult> EndSession(int id)
        {
            var session = await _context.Sessions
                                        .Include(s => s.Computer)
                                        .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null)
                return NotFound();

            if (session.Status == "Done")
                return BadRequest("Session đã kết thúc");

            session.EndTime = DateTime.Now;
            session.Status = "Done";

            // update máy về Available
            session.Computer.Status = "Available";

            await _context.SaveChangesAsync();

            return Ok(session);
        }

        // PUT: api/session/1 (update bình thường nếu cần)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Session session)
        {
            if (id != session.SessionId)
                return BadRequest();

            _context.Entry(session).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(session);
        }

        // DELETE: api/session/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _context.Sessions.FindAsync(id);

            if (session == null)
                return NotFound();

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}