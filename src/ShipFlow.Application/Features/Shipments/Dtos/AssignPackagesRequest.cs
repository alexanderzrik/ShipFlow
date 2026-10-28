namespace ShipFlow.Application.Features.Shipments.Dtos;

/// <summary>
/// بيانات طلب إضافة مجموعة بضائع إلى شحنة.
/// يُرسَل عبر body في POST /api/shipments/{id}/packages.
/// </summary>
public class AssignPackagesRequest
{
    /// <summary>
    /// قائمة معرّفات البضائع المراد إضافتها للشحنة.
    /// يجب أن تحتوي عنصراً واحداً على الأقل، ولا تحتوي معرّفات مكررة.
    /// </summary>
    public List<int> PackageIds { get; set; } = new();
}