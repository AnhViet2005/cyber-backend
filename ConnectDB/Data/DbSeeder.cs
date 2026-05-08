using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            // Seed Rooms & Computers
            if (!await db.Rooms.AnyAsync())
            {
                var rooms = new List<Room>
                {
                    new Room { Name = "Khu Máy Thường", Type = "Pro", PricePerHour = 5000, ImageUrl = "https://images.unsplash.com/photo-1542751371-adc38448a05e?q=80&w=2070&auto=format&fit=crop" },
                    new Room { Name = "Phòng VIP", Type = "VIP", PricePerHour = 10000, ImageUrl = "https://images.unsplash.com/photo-1598550476439-6847785fce6e?q=80&w=2070&auto=format&fit=crop" },
                    new Room { Name = "Streaming Studio", Type = "VIP", PricePerHour = 15000, ImageUrl = "https://images.unsplash.com/photo-1511512578047-dfb367046420?q=80&w=2071&auto=format&fit=crop" }
                };
                await db.Rooms.AddRangeAsync(rooms);
                await db.SaveChangesAsync();

                var computers = new List<Computer>();
                for (int i = 1; i <= 10; i++) computers.Add(new Computer { ComputerName = $"STD-{i:00}", Status = "Available", RoomId = rooms[0].RoomId });
                for (int i = 1; i <= 5; i++) computers.Add(new Computer { ComputerName = $"VIP-{i:00}", Status = "Available", RoomId = rooms[1].RoomId });
                for (int i = 1; i <= 2; i++) computers.Add(new Computer { ComputerName = $"STREAM-{i:00}", Status = "Available", RoomId = rooms[2].RoomId });
                await db.Computers.AddRangeAsync(computers);
                await db.SaveChangesAsync();
            }

            // Seed/Update Services
            var existingServices = await db.Services.ToListAsync();
            
            var targetServices = new List<Service>
            {
                // Food
                new Service { Name = "Mì Tôm Trứng", Category = "Food", Price = 15000, StockQuantity = 100 },
                new Service { Name = "Cơm Chiên Dương Châu", Category = "Food", Price = 35000, StockQuantity = 50 },
                new Service { Name = "Xúc Xích Đức", Category = "Food", Price = 10000, StockQuantity = 100 },
                new Service { Name = "Bánh Mì Patê", Category = "Food", Price = 20000, StockQuantity = 30 },
                // Drinks
                new Service { Name = "Sting Dâu", Category = "Drinks", Price = 12000, StockQuantity = 200 },
                new Service { Name = "Coca Cola", Category = "Drinks", Price = 12000, StockQuantity = 200 },
                new Service { Name = "Cafe Sữa Đá", Category = "Drinks", Price = 15000, StockQuantity = 100 },
                new Service { Name = "Trà Chanh", Category = "Drinks", Price = 10000, StockQuantity = 150 },
                // Combos
                new Service { Name = "Combo Đêm (Net + Sting + Mì)", Category = "Combos", Price = 45000, StockQuantity = 1 },
                new Service { Name = "Combo Sáng Khoái", Category = "Combos", Price = 25000, StockQuantity = 1 },
                // Time
                new Service { Name = "Nạp 10k Giờ Chơi", Category = "Time", Price = 10000, StockQuantity = 1 },
                new Service { Name = "Nạp 50k Giờ Chơi", Category = "Time", Price = 50000, StockQuantity = 1 },
                // Cards
                new Service { Name = "Thẻ Garena 20k", Category = "Cards", Price = 20000, StockQuantity = 100 },
                new Service { Name = "Thẻ Garena 50k", Category = "Cards", Price = 50000, StockQuantity = 50 },
                new Service { Name = "Thẻ Zing 20k", Category = "Cards", Price = 20000, StockQuantity = 100 },
                // Packages
                new Service { Name = "Gói 5 Tiếng", Category = "Packages", Price = 25000, StockQuantity = 1 },
                new Service { Name = "Gói 10 Tiếng", Category = "Packages", Price = 45000, StockQuantity = 1 }
            };

            foreach (var target in targetServices)
            {
                var existing = existingServices.FirstOrDefault(s => s.Name == target.Name);
                if (existing != null)
                {
                    // Update category if wrong
                    if (existing.Category != target.Category)
                    {
                        existing.Category = target.Category;
                    }
                }
                else
                {
                    await db.Services.AddAsync(target);
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
