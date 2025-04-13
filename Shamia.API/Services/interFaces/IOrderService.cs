using System.Threading.Tasks;
using Shamia.API.Dtos.Response;
using Shamia.API.Dtos.Request;
using Shamia.API.Dtos;

namespace Shamia.API.Services
{
    public interface IOrderService
    {
        Task<(bool isSuccess, ErrorResponse? errorMessage, GetOrderDataResponse? result)> GetOrderByIdAsync(string id);
        Task<(bool isSuccess, ErrorResponse? errorMessage, string? paymentUrl)> CreateOrderAsync(CreateOrderRequest orderCreateDto, string userId);
        Task<(bool isSuccess, ErrorResponse? errorMessage, UpdateStatusResponse? result)> UpdateOrderStatusAsync(string id, UpdateStatusRequest request);
        Task<(bool isSuccess, ErrorResponse? errorMessage, PagedList<GetOrderDataResponse>? result)> GetAllOrdersAsync(string? status, string? userName, int page, string? userId = null);
    }
}