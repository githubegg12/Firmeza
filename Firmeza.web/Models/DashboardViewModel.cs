namespace Firmeza.web.Models;

/// <summary>
/// View model for dashboard information
/// </summary>
public class DashboardViewModel
{
    public int TotalClients { get; set; }
    public int TotalProducts { get; set; }
    public int TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
    public DateTime LastUpdated { get; set; }
}