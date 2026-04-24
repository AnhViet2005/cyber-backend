using Microsoft.AspNetCore.Mvc;
using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/customer
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Customers.ToListAsync();
            return Ok(data);
        }

        // GET: api/customer/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        // POST: api/customer
        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok(customer);
        }

        // PUT: api/customer/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Customer updatedCustomer)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            // Kiểm tra nếu đổi username thì xem có trùng không
            if (!string.IsNullOrWhiteSpace(updatedCustomer.Username) && updatedCustomer.Username != customer.Username)
            {
                var usernameExists = await _context.Customers.AnyAsync(c => c.Username == updatedCustomer.Username);
                if (usernameExists)
                {
                    return BadRequest(new { message = "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác." });
                }
                customer.Username = updatedCustomer.Username;
            }

            // Cập nhật các trường
            customer.Fullname = updatedCustomer.Fullname;
            customer.Balance = updatedCustomer.Balance;
            customer.RemainingTime = updatedCustomer.RemainingTime;
            customer.RoleId = updatedCustomer.RoleId;

            // Không ghi đè password nếu bên frontend không gửi (do JsonIgnore)
            if (!string.IsNullOrEmpty(updatedCustomer.Password) && updatedCustomer.Password != "******")
            {
                customer.Password = updatedCustomer.Password;
            }

            await _context.SaveChangesAsync();
            return Ok(customer);
        }

        // DELETE: api/customer/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}