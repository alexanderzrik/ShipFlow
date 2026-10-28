using Microsoft.EntityFrameworkCore;
using ShipFlow.Domain.Entities;
using ShipFlow.Domain.Enums;

namespace ShipFlow.Infrastructure.Persistence;

/// <summary>
/// يملأ قاعدة البيانات ببيانات تجريبية (Seed Data) عند أول تشغيل.
/// - يُطبّق Migrations تلقائياً.
/// - يُنشئ مستخدمين (admin, operator).
/// - يُنشئ 10 زبائن، 20 بضاعة، 3 شحنات، 5 بوالص.
/// آمن للاستدعاء المتكرر (يتخطى الجداول غير الفارغة).
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// ينفّذ عملية الـSeed على الـDbContext المُمرَّر.
    /// </summary>
    /// <param name="context">سياق قاعدة البيانات.</param>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // 1) تطبيق Migrations المعلّقة (ينشئ الجداول لو غير موجودة).
        await context.Database.MigrateAsync();

        // 2) مستخدمون.
        if (!await context.Users.AnyAsync())
        {
            context.Users.AddRange(
                new User
                {
                    Username = "admin",
                    Email = "admin@shipflow.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin12345"),
                    FullName = "System Administrator",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "operator",
                    Email = "operator@shipflow.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Operator12345"),
                    FullName = "Operations Manager",
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });

            await context.SaveChangesAsync();
        }

        // 3) زبائن + بضائع + شحنات + بوالص.
        if (await context.Customers.AnyAsync())
        {
            return; // البيانات موجودة مسبقاً، لا نكرر.
        }

        var customers = new List<Customer>
        {
            new() { FullName = "Ahmed Al-Hassan", PhoneNumber = "0911111111", Address = "Damascus, Mazzeh", CreatedAt = DateTime.UtcNow.AddDays(-30) },
            new() { FullName = "Sara Mahmoud", PhoneNumber = "0922222222", Address = "Aleppo, New Aleppo", CreatedAt = DateTime.UtcNow.AddDays(-28) },
            new() { FullName = "Omar Khalil", PhoneNumber = "0933333333", Address = "Homs, Al-Insha'at", CreatedAt = DateTime.UtcNow.AddDays(-25) },
            new() { FullName = "Layla Ibrahim", PhoneNumber = "0944444444", Address = "Latakia, Al-Zira'ah", CreatedAt = DateTime.UtcNow.AddDays(-22) },
            new() { FullName = "Khaled Nassar", PhoneNumber = "0955555555", Address = "Damascus, Mezzeh 86", CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new() { FullName = "Rima Farouk", PhoneNumber = "0966666666", Address = "Aleppo, Al-Furqan", CreatedAt = DateTime.UtcNow.AddDays(-18) },
            new() { FullName = "Bashar Habib", PhoneNumber = "0977777777", Address = "Hama, Al-Arbaeen", CreatedAt = DateTime.UtcNow.AddDays(-15) },
            new() { FullName = "Dima Said", PhoneNumber = "0988888888", Address = "Tartous, Al-Rimal", CreatedAt = DateTime.UtcNow.AddDays(-12) },
            new() { FullName = "Fadi Haddad", PhoneNumber = "0999999999", Address = "Damascus, Al-Salihiyah", CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new() { FullName = "Nour Youssef", PhoneNumber = "0910101010", Address = "Aleppo, Al-Aziziyah", CreatedAt = DateTime.UtcNow.AddDays(-7) }
        };
        context.Customers.AddRange(customers);
        await context.SaveChangesAsync();

        var rnd = new Random(42); // Seed ثابت (نتائج قابلة للتكرار).

        var descriptions = new[]
        {
            "Electronics - Laptop", "Documents", "Clothing", "Books", "Food Items",
            "Furniture Parts", "Cosmetics", "Medical Supplies", "Auto Parts", "Toys"
        };
        var cities = new[] { "Damascus", "Aleppo", "Homs", "Latakia", "Hama", "Tartous" };
        var receiverNames = new[] { "Ali", "Hassan", "Rami", "Mona", "Zeina", "Karim", "Lina", "Samer" };

        var packages = new List<Package>();
        for (var i = 0; i < 20; i++)
        {
            var customer = customers[i % customers.Count];
            packages.Add(new Package
            {
                TrackingNumber = $"SHP-SEED-{i + 1:D4}",
                Description = descriptions[i % descriptions.Length],
                Weight = Math.Round((decimal)(rnd.NextDouble() * 9.5 + 0.5), 2),
                Status = PackageStatus.Received,
                ReceiverName = receiverNames[i % receiverNames.Length],
                ReceiverPhone = $"09{rnd.Next(10000000, 99999999)}",
                DestinationCity = cities[i % cities.Length],
                CustomerId = customer.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-20 + i)
            });
        }
        context.Packages.AddRange(packages);
        await context.SaveChangesAsync();

        // 3 شحنات بحالات مختلفة.
        var shipments = new List<Shipment>
        {
            new()
            {
                ShipmentNumber = "SHN-SEED-0001",
                FromCity = "Damascus", ToCity = "Aleppo",
                DriverName = "Mohammad Ali", DriverPhone = "0912345678",
                VehicleNumber = "D-12345",
                Status = ShipmentStatus.InTransit,
                DepartureDate = DateTime.UtcNow.AddDays(-3),
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new()
            {
                ShipmentNumber = "SHN-SEED-0002",
                FromCity = "Homs", ToCity = "Latakia",
                DriverName = "Fadi Samir", DriverPhone = "0923456789",
                VehicleNumber = "H-67890",
                Status = ShipmentStatus.Loading,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                ShipmentNumber = "SHN-SEED-0003",
                FromCity = "Damascus", ToCity = "Homs",
                DriverName = "Rami Khoury", DriverPhone = "0934567890",
                VehicleNumber = "D-54321",
                Status = ShipmentStatus.Arrived,
                DepartureDate = DateTime.UtcNow.AddDays(-5),
                ArrivalDate = DateTime.UtcNow.AddDays(-4),
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            }
        };
        context.Shipments.AddRange(shipments);
        await context.SaveChangesAsync();

        // تعيين بضائع للشحنات.
        for (var i = 0; i < 6; i++) packages[i].ShipmentId = shipments[0].Id;      // 6 للشحنة 1
        for (var i = 6; i < 10; i++) packages[i].ShipmentId = shipments[1].Id;    // 4 للشحنة 2
        for (var i = 10; i < 13; i++) packages[i].ShipmentId = shipments[2].Id;   // 3 للشحنة 3
        // packages[13..19] (7 بضائع) تبقى بدون شحنة.
        await context.SaveChangesAsync();

        // 5 بوالص للبضائع غير المعيَّنة (13..17).
        var billOfLadings = new List<BillOfLading>();
        for (var i = 0; i < 5; i++)
        {
            var package = packages[13 + i];
            billOfLadings.Add(new BillOfLading
            {
                BillNumber = $"BOL-SEED-{i + 1:D4}",
                IssuedAt = DateTime.UtcNow.AddDays(-10 + i),
                IssuedBy = i % 2 == 0 ? "admin" : "operator",
                DeliveryFee = Math.Round((decimal)(rnd.NextDouble() * 500 + 50), 2),
                Notes = i % 3 == 0 ? "Express delivery" : null,
                PackageId = package.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-10 + i)
            });
        }
        context.BillOfLadings.AddRange(billOfLadings);
        await context.SaveChangesAsync();
    }
}