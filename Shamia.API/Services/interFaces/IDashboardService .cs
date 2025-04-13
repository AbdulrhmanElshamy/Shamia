using Shamia.API.Dtos.Response;
using System.Threading.Tasks;

namespace Shamia.API.Services
{
    public interface IDashboardService
    {
        // Get dashboard statistics
        Task<(bool isSuccess, ErrorResponse? errorResponse, DashboardStatisticsResponse? response)> GetDashboardStatisticsAsync(int? year, int? month);

        // Export dashboard statistics to Excel
        Task<(bool isSuccess, ErrorResponse? errorResponse, byte[]? fileBytes)> ExportDashboardStatisticsToExcelAsync(int? year, int? month);

        // Export users to Excel
        Task<(bool isSuccess, ErrorResponse? errorResponse, byte[]? fileBytes)> ExportUsersToExcelAsync();

        // Export products to Excel
        Task<(bool isSuccess, ErrorResponse? errorResponse, byte[]? fileBytes)> ExportProductsToExcelAsync();

        // Export orders to Excel
        Task<(bool isSuccess, ErrorResponse? errorResponse, byte[]? fileBytes)> ExportOrdersToExcelAsync();
    }
}