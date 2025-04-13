using Shamia.API.Dtos;
using Shamia.API.Dtos.Request;
using Shamia.API.Dtos.Response;
using Shamia.DataAccessLayer.Entities;
using System.Threading.Tasks;

namespace Shamia.API.Services.interFaces
{
    public interface ICategoryService
    {
        Task<(bool isSuccess, ErrorResponse? errorMessage, Category? response)> GetCategoryByIdAsync(int id);
        Task<(bool isSuccess, ErrorResponse? errorMessage, Category? response)> GetCategoryBySlugAsync(string slug);
        Task<(bool isSuccess, ErrorResponse? errorMessage, Category? response)> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<(bool isSuccess, ErrorResponse? errorMessage, string? response)> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto, int id);
        Task<(bool isSuccess, ErrorResponse? errorMessage, object? response)> GetAllCategoriesAsync(
            int page = 1,
            string? query = null);
        Task<(bool isSuccess, ErrorResponse? errorMessage, object? response)> DeleteCategoryAsync(int id);
    }
}