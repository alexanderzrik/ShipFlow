using Microsoft.EntityFrameworkCore;
using ShipFlow.Application.Common.Interfaces;
using ShipFlow.Application.Features.Dashboard.Dtos;
using ShipFlow.Domain.Enums;

namespace ShipFlow.Application.Features.Dashboard.Services;

public class DashboardService : IDashboardService
{
    private readonly IApplicationDbContext _context;

    public DashboardService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var dto = new DashboardSummaryDto();

        // جلب كل البيانات إلى الذاكرة (آمن جداً مع LocalDB القديم ويتجنب أي تعقيدات في SQL)
        var customers = await _context.Customers.AsNoTracking().ToListAsync(cancellationToken);
        var packages = await _context.Packages.AsNoTracking().ToListAsync(cancellationToken);
        var shipments = await _context.Shipments.AsNoTracking().ToListAsync(cancellationToken);
        var dbRates = await _context.CurrencyRates.AsNoTracking().ToListAsync(cancellationToken);

        // 1. الإحصائيات الأساسية
        dto.TotalCustomers = customers.Count;
        dto.TotalPackages = packages.Count;
        dto.TotalShipments = shipments.Count;
        dto.ActiveShipments = shipments.Count(s => s.Status != ShipmentStatus.Closed && s.Status != ShipmentStatus.Cancelled);

        // 2. البضائع حسب الحالة
        var packagesByStatus = packages.GroupBy(p => p.Status).ToList();
        dto.PackagesByStatus = packagesByStatus
            .Select(g => new PackageStatusCountDto { Status = g.Key.ToString(), Count = g.Count() })
            .ToList();

        dto.DeliveredPackages = packages.Count(p => p.Status == PackageStatus.Delivered);
        dto.PendingPackages = dto.TotalPackages - dto.DeliveredPackages;

        // 3. الإيرادات حسب العملة
        var revenueData = packages
            .Where(p => p.Status == PackageStatus.Delivered && p.DeliveryFee > 0)
            .GroupBy(p => p.DeliveryFeeCurrency)
            .Select(g => new { Currency = g.Key, Total = g.Sum(p => p.DeliveryFee) })
            .ToList();

        // 4. أسعار الصرف
        decimal GetRate(Currency curr)
        {
            if (curr == Currency.SYP) return 1;
            var rate = dbRates.FirstOrDefault(r => r.Currency == curr);
            if (rate != null) return rate.RateToSyp;
            return curr switch
            {
                Currency.USD => 15000,
                Currency.EUR => 16000,
                Currency.TRY => 450,
                _ => 1
            };
        }

        decimal totalSyp = 0;
        foreach (var rev in revenueData)
        {
            var inSyp = rev.Total * GetRate(rev.Currency);
            totalSyp += inSyp;
            dto.RevenueByCurrency.Add(new CurrencyRevenueDto
            {
                Currency = rev.Currency.ToString(),
                Amount = rev.Total,
                AmountInSyp = Math.Round(inSyp, 0)
            });
        }

        dto.TotalRevenueSyp = Math.Round(totalSyp, 0);
        dto.TotalRevenueUsd = Math.Round(totalSyp / GetRate(Currency.USD), 2);

        // 5. أفضل 5 زبائن (حساب في الذاكرة لضمان عدم حدوث أخطاء)
        var customerDictionary = customers.ToDictionary(c => c.Id, c => c.FullName);
        var topPackages = packages
            .GroupBy(p => p.CustomerId)
            .Select(g => new { CustomerId = g.Key, PackageCount = g.Count() })
            .OrderByDescending(x => x.PackageCount)
            .Take(5)
            .ToList();

        dto.TopCustomers = topPackages.Select(x => new TopCustomerDto
        {
            CustomerId = x.CustomerId,
            CustomerName = customerDictionary.GetValueOrDefault(x.CustomerId, "غير معروف"),
            PackageCount = x.PackageCount
        }).ToList();

        return dto;
    }
}