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

            // Seed/Update Services with RICH DATA and IMAGES
            var existingServices = await db.Services.ToListAsync();
            
            var targetServices = new List<Service>
            {
                // --- ĐỒ ĂN (Food) ---
                new Service { Name = "Mì Tôm Trứng", Category = "Food", Price = 15000, StockQuantity = 100, ImageUrl = "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Mì Xào Bò", Category = "Food", Price = 35000, StockQuantity = 50, ImageUrl = "https://images.unsplash.com/photo-1585032226651-759b368d7246?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Mì Xào Trứng", Category = "Food", Price = 25000, StockQuantity = 50, ImageUrl = "https://images.unsplash.com/photo-1552611052-33e04de081de?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Cơm Chiên Dương Châu", Category = "Food", Price = 35000, StockQuantity = 50, ImageUrl = "https://images.unsplash.com/photo-1603133872878-684f208fb84b?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Cơm Rang Dưa Bò", Category = "Food", Price = 45000, StockQuantity = 40, ImageUrl = "https://images.unsplash.com/photo-1512058560366-cd242d458690?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Cơm Đùi Gà Chiên", Category = "Food", Price = 45000, StockQuantity = 30, ImageUrl = "https://images.unsplash.com/photo-1562967914-608f82629710?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Xúc Xích Đức (2 cây)", Category = "Food", Price = 20000, StockQuantity = 100, ImageUrl = "https://images.unsplash.com/photo-1541048612927-754c294590c8?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Bánh Mì Patê Trứng", Category = "Food", Price = 20000, StockQuantity = 30, ImageUrl = "https://images.unsplash.com/photo-1600454021970-351feb4a5142?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Khoai Tây Chiên", Category = "Food", Price = 25000, StockQuantity = 50, ImageUrl = "https://images.unsplash.com/photo-1573080496219-bb080dd4f877?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Ngô Chiên Bơ", Category = "Food", Price = 25000, StockQuantity = 50, ImageUrl = "https://images.unsplash.com/photo-1514733670139-4d87a1941d55?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Khô Bò Lá Chanh", Category = "Food", Price = 30000, StockQuantity = 40, ImageUrl = "https://images.unsplash.com/photo-1599487488170-d11ec9c172f0?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Gà Lắc Phô Mai", Category = "Food", Price = 35000, StockQuantity = 30, ImageUrl = "https://images.unsplash.com/photo-1626082927389-6cd097cdc6ec?q=80&w=2000&auto=format&fit=crop" },

                // --- ĐỒ UỐNG (Drinks) ---
                new Service { Name = "Sting Dâu (Chai)", Category = "Drinks", Price = 12000, StockQuantity = 200, ImageUrl = "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Coca Cola (Lon)", Category = "Drinks", Price = 12000, StockQuantity = 200, ImageUrl = "https://images.unsplash.com/photo-1554866585-cd94860890b7?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Pepsi (Lon)", Category = "Drinks", Price = 12000, StockQuantity = 200, ImageUrl = "https://images.unsplash.com/photo-1543253687-c931c8e01820?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "7Up (Lon)", Category = "Drinks", Price = 12000, StockQuantity = 200, ImageUrl = "https://images.unsplash.com/photo-1624517452488-04869289c4ca?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Red Bull (Thái)", Category = "Drinks", Price = 20000, StockQuantity = 100, ImageUrl = "https://images.unsplash.com/photo-1613214049841-02898dee80f9?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Monster Energy", Category = "Drinks", Price = 45000, StockQuantity = 50, ImageUrl = "https://images.unsplash.com/photo-1622543925917-763c34d1ab76?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Cafe Sữa Đá", Category = "Drinks", Price = 15000, StockQuantity = 100, ImageUrl = "https://images.unsplash.com/photo-1541167760496-162955ed8a9f?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Cafe Đen Đá", Category = "Drinks", Price = 12000, StockQuantity = 100, ImageUrl = "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Bạc Xỉu", Category = "Drinks", Price = 20000, StockQuantity = 80, ImageUrl = "https://images.unsplash.com/photo-1541167760496-162955ed8a9f?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Trà Đào Cam Sả", Category = "Drinks", Price = 25000, StockQuantity = 60, ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Trà Chanh Mật Ong", Category = "Drinks", Price = 15000, StockQuantity = 150, ImageUrl = "https://images.unsplash.com/photo-1576092768241-dec231879fc3?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Nước Suối Aquafina", Category = "Drinks", Price = 10000, StockQuantity = 300, ImageUrl = "https://images.unsplash.com/photo-1523362628745-0c100150b504?q=80&w=2000&auto=format&fit=crop" },

                // --- THẺ CÀO (Cards) ---
                new Service { Name = "Thẻ Garena 50k", Category = "Cards", Price = 50000, StockQuantity = 50, ImageUrl = "https://images.unsplash.com/photo-1580519542036-c47de6196ba5?q=80&w=2000&auto=format&fit=crop" },
                new Service { Name = "Thẻ Zing 50k", Category = "Cards", Price = 50000, StockQuantity = 50, ImageUrl = "https://images.unsplash.com/photo-1601381718415-a05fb0a261f3?q=80&w=2000&auto=format&fit=crop" }
            };

            foreach (var target in targetServices)
            {
                var existing = existingServices.FirstOrDefault(s => s.Name == target.Name);
                if (existing != null)
                {
                    existing.Category = target.Category;
                    existing.Price = target.Price;
                    existing.StockQuantity = target.StockQuantity;
                    existing.ImageUrl = target.ImageUrl; // 👈 Cập nhật ảnh online
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
