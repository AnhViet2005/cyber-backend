using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SupportController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Support/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<SupportMessage>>> GetCustomerMessages(int customerId)
        {
            var messages = await _context.SupportMessages
                .Where(m => m.CustomerId == customerId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return Ok(messages);
        }

        // GET: api/Support/active-chats
        [HttpGet("active-chats")]
        public async Task<ActionResult<IEnumerable<object>>> GetActiveChats()
        {
            // Get all customers who have sent at least one message, with unread count
            var chats = await _context.SupportMessages
                .Include(m => m.Customer)
                .GroupBy(m => m.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    CustomerName = g.FirstOrDefault().Customer.Fullname,
                    Username = g.FirstOrDefault().Customer.Username,
                    LastMessage = g.OrderByDescending(m => m.Timestamp).FirstOrDefault().Message,
                    LastTimestamp = g.OrderByDescending(m => m.Timestamp).FirstOrDefault().Timestamp,
                    UnreadCount = g.Count(m => m.Sender == "User" && !m.IsRead)
                })
                .OrderByDescending(c => c.LastTimestamp)
                .ToListAsync();

            return Ok(chats);
        }

        // POST: api/Support
        [HttpPost]
        public async Task<ActionResult<SupportMessage>> SaveMessage(SupportMessage message)
        {
            message.Timestamp = DateTime.UtcNow;
            _context.SupportMessages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(message);
        }

        // PUT: api/Support/mark-read/{customerId}
        [HttpPut("mark-read/{customerId}")]
        public async Task<IActionResult> MarkAsRead(int customerId, [FromQuery] string reader)
        {
            // reader is either "Admin" (marking User messages as read) or "User" (marking Admin messages as read)
            var senderToMark = reader == "Admin" ? "User" : "Admin";

            var unreadMessages = await _context.SupportMessages
                .Where(m => m.CustomerId == customerId && m.Sender == senderToMark && !m.IsRead)
                .ToListAsync();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true });
        }
    }
}
