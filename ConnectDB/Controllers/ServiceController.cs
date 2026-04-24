using Microsoft.AspNetCore.Mvc;
using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/service
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _context.Services
                                         .Include(s => s.AreaPrices)
                                         .ToListAsync();
            return Ok(services);
        }

        // GET: api/service/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                return NotFound();

            return Ok(service);
        }

        // POST: api/service
        [HttpPost]
        public async Task<IActionResult> Create(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return Ok(service);
        }

        // PUT: api/service/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Service service)
        {
            if (id != service.ServiceId)
                return BadRequest();

            _context.Entry(service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Services.Any(s => s.ServiceId == id))
                    return NotFound();
                else
                    throw;
            }

            return Ok(service);
        }

        // DELETE: api/service/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                return NotFound();

            // ❗ Không cho xóa nếu đang được dùng trong OrderDetail
            var isUsed = await _context.OrderDetails
                                       .AnyAsync(o => o.ServiceId == id);

            if (isUsed)
                return BadRequest("Service đang được sử dụng, không thể xóa");

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}