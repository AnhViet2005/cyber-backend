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
 
        // GET: api/session/active/1
        [HttpGet("active/{customerId}")]
        public async Task<IActionResult> GetActiveSession(int customerId)
        {
            var session = await _context.Sessions
                                        .Include(s => s.Customer)
                                        .Include(s => s.Computer)
                                        .ThenInclude(c => c.Room)
                                        .OrderByDescending(s => s.StartTime)
                                        .FirstOrDefaultAsync(s => s.CustomerId == customerId && s.Status == "Playing");

            if (session == null)
                return NotFound("Không tìm thấy phiên chơi đang hoạt động");

            return Ok(session);
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
        public async Task<IActionResult> StartSession([FromBody] Session session)
        {
            Console.WriteLine($"Starting session for Customer: {session.CustomerId}, Computer: {session.ComputerId}");
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

            try 
            {
                session.StartTime = DateTime.Now;
                session.Status = "Playing";

                _context.Sessions.Add(session);

                // update trạng thái máy
                computer.Status = "Using";

                await _context.SaveChangesAsync();
                return Ok(session);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống khi lưu session: {ex.Message} {ex.InnerException?.Message}");
            }
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
        // 🔥 HEARTBEAT (gọi mỗi phút để trừ tiền/giờ)
        [HttpPost("heartbeat")]
        public async Task<IActionResult> Heartbeat()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int customerId = int.Parse(userIdClaim.Value);

            var session = await _context.Sessions
                                        .Include(s => s.Customer)
                                        .Include(s => s.Computer)
                                        .FirstOrDefaultAsync(s => s.CustomerId == customerId && s.Status == "Playing");

            if (session == null)
                return NotFound(new { message = "Phiên chơi không hoạt động" });

            var customer = session.Customer;
            if (customer == null) return BadRequest("Không tìm thấy khách hàng");

            // 1. Ưu tiên trừ RemainingTime (giờ tặng/mua trước)
            if (customer.RemainingTime > 0)
            {
                customer.RemainingTime -= 1; // Trừ 1 phút
                if (customer.RemainingTime < 0) customer.RemainingTime = 0;
            }
            else
            {
                // 2. Nếu hết RemainingTime, trừ Balance theo HourlyRate
                // Tiền mỗi phút = HourlyRate / 60
                decimal costPerMinute = session.HourlyRate / 60;
                
                if (customer.Balance >= costPerMinute)
                {
                    customer.Balance -= costPerMinute;
                }
                else
                {
                    // 3. Nếu hết cả tiền, tự động kết thúc session
                    session.EndTime = DateTime.Now;
                    session.Status = "Done";
                    session.Computer.Status = "Available";

                    await _context.SaveChangesAsync();
                    return Ok(new { status = "LoggedOut", message = "Hết thời gian sử dụng!" });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { 
                status = "Active", 
                remainingTime = customer.RemainingTime, 
                balance = customer.Balance 
            });
        }
    }
}