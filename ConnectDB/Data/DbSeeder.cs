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

            // Seed/Update Services with RICH DATA for Food & Drinks
            var existingServices = await db.Services.ToListAsync();
            
            var targetServices = new List<Service>
            {
                // --- ĐỒ ĂN (Food) ---
                new Service { Name = "Mì Tôm Trứng", Category = "Food", Price = 15000, StockQuantity = 100 },
                new Service { Name = "Mì Xào Bò", Category = "Food", Price = 35000, StockQuantity = 50 },
                new Service { Name = "Mì Xào Trứng", Category = "Food", Price = 25000, StockQuantity = 50 },
                new Service { Name = "Cơm Chiên Dương Châu", Category = "Food", Price = 35000, StockQuantity = 50 },
                new Service { Name = "Cơm Rang Dưa Bò", Category = "Food", Price = 45000, StockQuantity = 40 },
                new Service { Name = "Cơm Đùi Gà Chiên", Category = "Food", Price = 45000, StockQuantity = 30 },
                new Service { Name = "Xúc Xích Đức (2 cây)", Category = "Food", Price = 20000, StockQuantity = 100 },
                new Service { Name = "Bánh Mì Patê Trứng", Category = "Food", Price = 20000, StockQuantity = 30 },
                new Service { Name = "Khoai Tây Chiên", Category = "Food", Price = 25000, StockQuantity = 50 },
                new Service { Name = "Ngô Chiên Bơ", Category = "Food", Price = 25000, StockQuantity = 50 },
                new Service { Name = "Khô Bò Lá Chanh", Category = "Food", Price = 30000, StockQuantity = 40 },
                new Service { Name = "Gà Lắc Phô Mai", Category = "Food", Price = 35000, StockQuantity = 30 },

                // --- ĐỒ UỐNG (Drinks) ---
                new Service { Name = "Sting Dâu (Chai)", Category = "Drinks", Price = 12000, StockQuantity = 200 },
                new Service { Name = "Coca Cola (Lon)", Category = "Drinks", Price = 12000, StockQuantity = 200 },
                new Service { Name = "Pepsi (Lon)", Category = "Drinks", Price = 12000, StockQuantity = 200 },
                new Service { Name = "7Up (Lon)", Category = "Drinks", Price = 12000, StockQuantity = 200 },
                new Service { Name = "Red Bull (Thái)", Category = "Drinks", Price = 20000, StockQuantity = 100 },
                new Service { Name = "Monster Energy", Category = "Drinks", Price = 45000, StockQuantity = 50 },
                new Service { Name = "Cafe Sữa Đá", Category = "Drinks", Price = 15000, StockQuantity = 100 },
                new Service { Name = "Cafe Đen Đá", Category = "Drinks", Price = 12000, StockQuantity = 100 },
                new Service { Name = "Bạc Xỉu", Category = "Drinks", Price = 20000, StockQuantity = 80 },
                new Service { Name = "Trà Đào Cam Sả", Category = "Drinks", Price = 25000, StockQuantity = 60 },
                new Service { Name = "Trà Chanh Mật Ong", Category = "Drinks", Price = 15000, StockQuantity = 150 },
                new Service { Name = "Nước Suối Aquafina", Category = "Drinks", Price = 10000, StockQuantity = 300 },
                new Service { Name = "Sữa Đậu Nành", Category = "Drinks", Price = 12000, StockQuantity = 100 },

                // --- COMBOS ---
                new Service { Name = "Combo Đêm (Net + Sting + Mì)", Category = "Combos", Price = 45000, StockQuantity = 1 },
                new Service { Name = "Combo Sáng Khoái (Cafe + Bánh Mì)", Category = "Combos", Price = 30000, StockQuantity = 1 },
                new Service { Name = "Combo Game Thủ (Monster + Gà Lắc)", Category = "Combos", Price = 70000, StockQuantity = 1 },

                // --- GIỜ CHƠI (Time) ---
                new Service { Name = "Nạp 10k Giờ Chơi", Category = "Time", Price = 10000, StockQuantity = 1 },
                new Service { Name = "Nạp 20k Giờ Chơi", Category = "Time", Price = 20000, StockQuantity = 1 },
                new Service { Name = "Nạp 50k Giờ Chơi", Category = "Time", Price = 50000, StockQuantity = 1 },
                new Service { Name = "Nạp 100k Giờ Chơi", Category = "Time", Price = 100000, StockQuantity = 1 },

                // --- THẺ CÀO (Cards) ---
                new Service { Name = "Thẻ Garena 20k", Category = "Cards", Price = 20000, StockQuantity = 100 },
                new Service { Name = "Thẻ Garena 50k", Category = "Cards", Price = 50000, StockQuantity = 50 },
                new Service { Name = "Thẻ Garena 100k", Category = "Cards", Price = 100000, StockQuantity = 20 },
                new Service { Name = "Thẻ Zing 20k", Category = "Cards", Price = 20000, StockQuantity = 100 },
                new Service { Name = "Thẻ Zing 50k", Category = "Cards", Price = 50000, StockQuantity = 50 },

                // --- GÓI GIỜ (Packages) ---
                new Service { Name = "Gói 3 Tiếng", Category = "Packages", Price = 15000, StockQuantity = 1 },
                new Service { Name = "Gói 5 Tiếng", Category = "Packages", Price = 25000, StockQuantity = 1 },
                new Service { Name = "Gói 10 Tiếng", Category = "Packages", Price = 45000, StockQuantity = 1 },
                new Service { Name = "Gói Xuyên Đêm (22h - 8h)", Category = "Packages", Price = 35000, StockQuantity = 1 }
            };

            foreach (var target in targetServices)
            {
                var existing = existingServices.FirstOrDefault(s => s.Name == target.Name);
                if (existing != null)
                {
                    existing.Category = target.Category;
                    existing.Price = target.Price;
                    existing.StockQuantity = target.StockQuantity;
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
