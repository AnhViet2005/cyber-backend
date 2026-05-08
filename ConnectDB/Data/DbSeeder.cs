using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            if (await db.Rooms.AnyAsync()) return;

            var rooms = new List<Room>
            {
                new Room { 
                    Name = "Khu Máy Thường", 
                    Type = "Pro", 
                    PricePerHour = 5000, 
                    ImageUrl = "https://images.unsplash.com/photo-1542751371-adc38448a05e?q=80&w=2070&auto=format&fit=crop" 
                },
                new Room { 
                    Name = "Phòng VIP", 
                    Type = "VIP", 
                    PricePerHour = 10000, 
                    ImageUrl = "https://images.unsplash.com/photo-1598550476439-6847785fce6e?q=80&w=2070&auto=format&fit=crop" 
                },
                new Room { 
                    Name = "Streaming Studio", 
                    Type = "VIP", 
                    PricePerHour = 15000, 
                    ImageUrl = "https://images.unsplash.com/photo-1511512578047-dfb367046420?q=80&w=2071&auto=format&fit=crop" 
                }
            };

            await db.Rooms.AddRangeAsync(rooms);
            await db.SaveChangesAsync();

            var computers = new List<Computer>();
            
            // Seed 10 computers for Room 1
            for (int i = 1; i <= 10; i++)
            {
                computers.Add(new Computer { ComputerName = $"STD-{i:00}", Status = "Available", RoomId = rooms[0].RoomId });
            }

            // Seed 5 computers for Room 2
            for (int i = 1; i <= 5; i++)
            {
                computers.Add(new Computer { ComputerName = $"VIP-{i:00}", Status = "Available", RoomId = rooms[1].RoomId });
            }

            // Seed 2 computers for Room 3
            for (int i = 1; i <= 2; i++)
            {
                computers.Add(new Computer { ComputerName = $"STREAM-{i:00}", Status = "Available", RoomId = rooms[2].RoomId });
            }

            await db.Computers.AddRangeAsync(computers);

            // Seed some Services if none exist
            if (!await db.Services.AnyAsync())
            {
                var services = new List<Service>
                {
                    new Service { Name = "Mì Tôm Trứng", Category = "Food", Price = 15000, StockQuantity = 100 },
                    new Service { Name = "Cơm Chiên Dương Châu", Category = "Food", Price = 35000, StockQuantity = 50 },
                    new Service { Name = "Sting Dâu", Category = "Drink", Price = 12000, StockQuantity = 200 },
                    new Service { Name = "Coca Cola", Category = "Drink", Price = 12000, StockQuantity = 200 },
                    new Service { Name = "Xúc Xích Đức", Category = "Food", Price = 10000, StockQuantity = 100 }
                };
                await db.Services.AddRangeAsync(services);
            }

            await db.SaveChangesAsync();
        }
    }
}
