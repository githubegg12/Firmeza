namespace Firmeza.web.Models;

/// <summary>
/// ViewModel to hold the data required for the main admin dashboard.
/// </summary>
public class DashboardViewModel
{
    public int TotalProducts { get; set; }
    public int TotalClients { get; set; }
    public int TotalSales { get; set; }
}