using lab1_4.Models.ViewModels;

namespace lab1_4.Services.Interfaces;

public interface IHomeService
{
    Task<DashboardStats> GetStatsAsync();
}
