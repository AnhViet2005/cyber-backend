using Microsoft.AspNetCore.Mvc;
using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _jwtKey = "PhimSecNhatBanVip12345678901234567890"; // Đồng bộ với Program.cs

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Customers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);

            if (user == null)
                return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu" });

            // 🛑 KIỂM TRA SỐ DƯ (Chỉ áp dụng cho User, Admin vẫn cho log)
            if (user.Role?.RoleName == "User" && user.Balance <= 0 && user.RemainingTime <= 0)
                return BadRequest(new { message = "Bạn đã hết thời gian vui lòng nạp tại quầy" });

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.CustomerId,
                    user.Username,
                    user.Fullname,
                    user.Balance,
                    user.RemainingTime,
                    Role = user.Role?.RoleName
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // 🛑 KIỂM TRA INPUT
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest(new { message = "Username và Password không được để trống!" });

                // 🛑 KIỂM TRA TRÙNG TÊN ĐĂNG NHẬP
                var existingUser = await _context.Customers
                    .AnyAsync(u => u.Username == request.Username);

                if (existingUser)
                    return BadRequest(new { message = "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác!" });

                var newUser = new Customer
                {
                    Username = request.Username.Trim(),
                    Password = request.Password,
                    Fullname = request.Fullname ?? request.Username,
                    Balance = 0,
                    RemainingTime = 30, // 🎁 Tặng 30 phút cho người mới
                    RoleId = request.RoleId != 0 ? request.RoleId : 2, // Mặc định là User (ID=2)
                    CreatedDate = DateTime.UtcNow
                };

                _context.Customers.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đăng ký thành công!" });
            }
            catch (Exception ex)
            {
                // Trả về lỗi chi tiết để debug
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message, detail = ex.InnerException?.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var user = await _context.Customers.FindAsync(userId);

            if (user == null) return NotFound();

            return Ok(user);
        }

        private string GenerateJwtToken(Customer user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.CustomerId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public int RoleId { get; set; } // 1: Admin, 2: User
    }
}
