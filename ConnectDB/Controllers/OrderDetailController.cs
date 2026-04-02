using Microsoft.AspNetCore.Mvc;
using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderDetailController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/orderdetail
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.OrderDetails
                                     .Include(o => o.Order)
                                     .Include(o => o.Service)
                                     .ToListAsync();

            return Ok(data);
        }

        // GET: api/orderdetail/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.OrderDetails
                                     .Include(o => o.Order)
                                     .Include(o => o.Service)
                                     .FirstOrDefaultAsync(o => o.OrderDetailId == id);

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // POST: api/orderdetail
        [HttpPost]
        public async Task<IActionResult> Create(OrderDetail orderDetail)
        {
            // check Order tồn tại
            var orderExists = await _context.Orders
                                            .AnyAsync(o => o.OrderId == orderDetail.OrderId);
            if (!orderExists)
                return BadRequest("Order không tồn tại");

            // check Service tồn tại
            var serviceExists = await _context.Services
                                              .AnyAsync(s => s.ServiceId == orderDetail.ServiceId);
            if (!serviceExists)
                return BadRequest("Service không tồn tại");

            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            return Ok(orderDetail);
        }

        // PUT: api/orderdetail/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailId)
                return BadRequest();

            // check Order
            var orderExists = await _context.Orders
                                            .AnyAsync(o => o.OrderId == orderDetail.OrderId);
            if (!orderExists)
                return BadRequest("Order không tồn tại");

            // check Service
            var serviceExists = await _context.Services
                                              .AnyAsync(s => s.ServiceId == orderDetail.ServiceId);
            if (!serviceExists)
                return BadRequest("Service không tồn tại");

            _context.Entry(orderDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.OrderDetails.Any(o => o.OrderDetailId == id))
                    return NotFound();
                else
                    throw;
            }

            return Ok(orderDetail);
        }

        // DELETE: api/orderdetail/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.OrderDetails.FindAsync(id);

            if (item == null)
                return NotFound();

            _context.OrderDetails.Remove(item);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}