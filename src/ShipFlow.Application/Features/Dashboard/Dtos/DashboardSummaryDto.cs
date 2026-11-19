namespace ShipFlow.Application.Features.Dashboard.Dtos;

public class DashboardSummaryDto
{
    public int TotalCustomers { get; set; }
    public int TotalPackages { get; set; }
    public int TotalShipments { get; set; }
    public int ActiveShipments { get; set; }
    public int DeliveredPackages { get; set; }
    public int PendingPackages { get; set; }

    public decimal TotalRevenueSyp { get; set; }
    public decimal TotalRevenueUsd { get; set; }

    public List<CurrencyRevenueDto> RevenueByCurrency { get; set; } = new();
    public List<PackageStatusCountDto> PackagesByStatus { get; set; } = new();
    public List<TopCustomerDto> TopCustomers { get; set; } = new();
}

public class CurrencyRevenueDto
{
    public string Currency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal AmountInSyp { get; set; }
}

public class PackageStatusCountDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TopCustomerDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int PackageCount { get; set; }
}