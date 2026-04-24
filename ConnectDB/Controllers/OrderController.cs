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
                                       .Include("Session.Customer")
                                       .Include("Session.Computer")
                                       .Include("OrderDetails.Service")
                                       .OrderByDescending(o => o.OrderTime)
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
            // kiểm tra Session tồn tại (nếu có cung cấp)
            if (order.SessionId.HasValue)
            {
                var sessionExists = await _context.Sessions
                                                  .AnyAsync(s => s.SessionId == order.SessionId);

                if (!sessionExists)
                    return BadRequest("Session không tồn tại");
            }

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

            // kiểm tra Session hợp lệ (nếu có cung cấp)
            if (order.SessionId.HasValue)
            {
                var sessionExists = await _context.Sessions
                                                  .AnyAsync(s => s.SessionId == order.SessionId);

                if (!sessionExists)
                    return BadRequest("Session không tồn tại");
            }

            // Nếu chuyển trạng thái sang Completed, thực hiện trừ kho
            if (order.Status?.ToLower() == "completed")
            {
                var fullOrder = await _context.Orders
                                              .AsNoTracking()
                                              .Include(o => o.OrderDetails)
                                              .FirstOrDefaultAsync(o => o.OrderId == id);
                
                if (fullOrder != null)
                {
                    // Lấy Session và Customer để cộng giờ
                    var session = await _context.Sessions
                                                .Include(s => s.Customer)
                                                .FirstOrDefaultAsync(s => s.SessionId == fullOrder.SessionId);

                    foreach (var detail in fullOrder.OrderDetails)
                    {
                        var service = await _context.Services.FindAsync(detail.ServiceId);
                        if (service == null) continue;

                        // 1. Trừ kho (đối với hàng hóa vật lý)
                        if (service.Category != "Time" && service.Category != "Combos" && service.Category != "Packages")
                        {
                            service.StockQuantity -= detail.Quantity;
                            if (service.StockQuantity < 0) service.StockQuantity = 0; 
                        }

                        // 2. Cộng giờ chơi (đối với Giờ chơi và Gói giờ)
                        if (session != null && session.Customer != null)
                        {
                            int minutesToAdd = 0;

                            if (service.Category == "Time")
                            {
                                // Tính theo số tiền: (Giá * Số lượng) / Đơn giá giờ của Session * 60 phút
                                decimal totalMoney = detail.Price * detail.Quantity;
                                decimal hourlyRate = session.HourlyRate > 0 ? session.HourlyRate : 10000; // Mặc định 10k nếu lỗi
                                minutesToAdd = (int)(totalMoney / hourlyRate * 60);
                            }
                            else if ((service.Category == "Packages" || service.Category == "Combos") && service.DurationMinutes.HasValue)
                            {
                                // Tính theo gói cố định (Gói giờ hoặc Combo có kèm giờ)
                                minutesToAdd = service.DurationMinutes.Value * detail.Quantity;
                            }

                            if (minutesToAdd > 0)
                            {
                                session.Customer.RemainingTime += minutesToAdd;
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }

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