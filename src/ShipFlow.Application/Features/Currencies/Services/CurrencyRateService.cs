using Microsoft.EntityFrameworkCore;
using ShipFlow.Application.Common.Interfaces;
using ShipFlow.Application.Features.Currencies.Dtos;
using ShipFlow.Domain.Entities;
using ShipFlow.Domain.Enums;

namespace ShipFlow.Application.Features.Currencies.Services;

/// <summary>
/// تنفيذ خدمة إدارة أسعار العملات.
/// </summary>
public class CurrencyRateService : ICurrencyRateService
{
    private readonly IApplicationDbContext _context;

    public CurrencyRateService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CurrencyRateDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // 1. جلب العملات الموجودة فعلياً في قاعدة البيانات
        var dbRates = await _context.CurrencyRates
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // 2. تجهيز القائمة النهائية (نبدأ بالليرة السورية لأنها ثابتة دائماً)
        var result = new List<CurrencyRateDto>
        {
            new() { Id = 0, Currency = Currency.SYP, RateToSyp = 1, UpdatedAt = DateTime.UtcNow }
        };

        // 3. القيم الافتراضية للعملات (تُستخدم إذا لم تكن العملة موجودة في قاعدة البيانات بعد)
        var defaultRates = new Dictionary<Currency, decimal>
        {
            { Currency.USD, 15000 },
            { Currency.EUR, 16000 },
            { Currency.TRY, 450 }
        };

        // 4. دمج قاعدة البيانات مع القيم الافتراضية
        foreach (var curr in defaultRates.Keys.OrderBy(c => c))
        {
            var dbRate = dbRates.FirstOrDefault(r => r.Currency == curr);
            
            if (dbRate != null)
            {
                // العملة موجودة في قاعدة البيانات (تم تحديثها سابقاً)
                result.Add(new CurrencyRateDto
                {
                    Id = dbRate.Id,
                    Currency = dbRate.Currency,
                    RateToSyp = dbRate.RateToSyp,
                    UpdatedAt = dbRate.UpdatedAt
                });
            }
            else
            {
                // العملة غير موجودة في قاعدة البيانات (لم تُحدّث بعد)، نعرض القيمة الافتراضية
                result.Add(new CurrencyRateDto
                {
                    Id = 0,
                    Currency = curr,
                    RateToSyp = defaultRates[curr],
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        return result;
    }

    public async Task<CurrencyRateDto> UpdateAsync(UpdateCurrencyRateRequest request, CancellationToken cancellationToken = default)
    {
        // لا يمكن تغيير سعر الليرة السورية (دائماً 1)
        if (request.Currency == Currency.SYP)
        {
            throw new FluentValidation.ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("Currency", "لا يمكن تغيير سعر صرف الليرة السورية (دائماً 1).")
            });
        }

        var rate = await _context.CurrencyRates
            .FirstOrDefaultAsync(c => c.Currency == request.Currency, cancellationToken);

        if (rate is null)
        {
            rate = new CurrencyRate
            {
                Currency = request.Currency,
                RateToSyp = request.RateToSyp,
                UpdatedAt = DateTime.UtcNow
            };
            _context.CurrencyRates.Add(rate);
        }
        else
        {
            rate.RateToSyp = request.RateToSyp;
            rate.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new CurrencyRateDto
        {
            Id = rate.Id,
            Currency = rate.Currency,
            RateToSyp = rate.RateToSyp,
            UpdatedAt = rate.UpdatedAt
        };
    }
}