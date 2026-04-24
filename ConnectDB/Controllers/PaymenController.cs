using Microsoft.AspNetCore.Mvc;
using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/payment
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _context.Payments
                                         .Include(p => p.Session)
                                         .ToListAsync();

            return Ok(payments);
        }

        // GET: api/payment/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _context.Payments
                                        .Include(p => p.Session)
                                        .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        // POST: api/payment
        [HttpPost]
        public async Task<IActionResult> Create(Payment payment)
        {
            var session = await _context.Sessions
                                        .Include(s => s.Customer)
                                        .FirstOrDefaultAsync(s => s.SessionId == payment.SessionId);

            if (session == null)
                return BadRequest("Session không tồn tại");

            // 🔥 TÍNH TIỀN GIỜ CHƠI (ví dụ 5000đ/giờ)
            decimal hourlyRate = 5000;

            var endTime = session.EndTime ?? DateTime.Now;
            var totalHours = (decimal)(endTime - (session.StartTime ?? endTime)).TotalHours;
            var sessionCost = Math.Ceiling(totalHours) * hourlyRate;

            // 🔥 TÍNH TIỀN ORDER
            var orderCost = await _context.OrderDetails
                                          .Where(o => o.Order.SessionId == payment.SessionId)
                                          .SumAsync(o => o.Price * o.Quantity);

            var total = sessionCost + orderCost;

            payment.TotalAmount = total;

            // 🔥 THANH TOÁN
            if (payment.PaymentMethod == "Balance")
            {
                if (session.Customer.Balance < total)
                    return BadRequest("Không đủ tiền trong tài khoản");

                session.Customer.Balance -= total;
            }

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(payment);
        }

        // DELETE: api/payment/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
                return NotFound();

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}