using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShipFlow.Infrastructure.Persistence;

/// <summary>
/// مصنع سياق قاعدة البيانات للاستخدام في وقت التصميم (Design-Time).
/// ضروري لأوامر `dotnet ef migrations` لأنها ما بتشتغل من داخل الـAPI runtime.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// ينشئ نسخة من ApplicationDbContext باستخدام Connection String ثابت للـmigrations.
    /// </summary>
    /// <param name="args">وسائط سطر الأوامر (غير مستخدمة).</param>
    /// <returns>نسخة جديدة من ApplicationDbContext.</returns>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        const string connectionString =
            "Server=(localdb)\\mssqllocaldb;Database=ShipFlowDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}