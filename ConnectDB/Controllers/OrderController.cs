using Microsoft.AspNetCore.Mvc;
using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/order
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _context.Orders
                                       .Include(o => o.Session)
                                       .ToListAsync();

            return Ok(orders);
        }

        // GET: api/order/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.Session)
                                      .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // POST: api/order
        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            // kiểm tra Session tồn tại
            var sessionExists = await _context.Sessions
                                              .AnyAsync(s => s.SessionId == order.SessionId);

            if (!sessionExists)
                return BadRequest("Session không tồn tại");

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        // PUT: api/order/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Order order)
        {
            if (id != order.OrderId)
                return BadRequest();

            // kiểm tra Session hợp lệ
            var sessionExists = await _context.Sessions
                                              .AnyAsync(s => s.SessionId == order.SessionId);

            if (!sessionExists)
                return BadRequest("Session không tồn tại");

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(o => o.OrderId == id))
                    return NotFound();
                else
                    throw;
            }

            return Ok(order);
        }

        // DELETE: api/order/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}