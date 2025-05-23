﻿using Shamia.API.Dtos;
using Shamia.API.Dtos.Request;
using Shamia.API.Dtos.Response;
using Shamia.API.Services.interFaces;
using Shamia.DataAccessLayer.Entities;
using Shamia.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Shamia.API.Extension;
using System.Text.RegularExpressions;
using Azure;

namespace Shamia.API.Services
{
    /// <summary>
    /// Provides category management services including CRUD operations and product associations
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ShamiaDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public CategoryService(ShamiaDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Retrieves a category by its unique identifier including associated products
        /// </summary>
        /// <param name="id">The category ID to search for</param>
        /// <returns>Tuple containing success status, error message (if any), and category data</returns>
        public async Task<(bool isSuccess, ErrorResponse? errorMessage, Category? response)> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.CategoriesProducts)
                    .ThenInclude(cp => cp.Product)
                .SingleOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return (false, new ErrorResponse().CreateErrorResponse("Category not found.", "الفئة غير موجودة."), null);

            return (true, null, category);
        }

        public async Task<(bool isSuccess, ErrorResponse? errorMessage, Category? response)> GetCategoryBySlugAsync(string slug)
        {
            var category = await _context.Categories
                .Include(c => c.CategoriesProducts)
                    .ThenInclude(cp => cp.Product)
                .SingleOrDefaultAsync(c => c.Slug_En == slug || c.Slug_Ar == slug);

            if (category == null)
                return (false, new ErrorResponse().CreateErrorResponse("Category not found.", "الفئة غير موجودة."), null);

            return (true, null, category);
        }

        /// <summary>
        /// Creates a new category with localized names and image
        /// </summary>
        /// <param name="createCategoryDto">Category creation data transfer object</param>
        /// <returns>Tuple containing success status, error message (if any), and created category</returns>
        public async Task<(bool isSuccess, ErrorResponse? errorMessage, Category? response)> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            // Extension By Hand
            var imagePath = await createCategoryDto.Image.SaveImageAsync(_fileStorageService);

            var category = new Category
            {
                Name_En = createCategoryDto.Name_En,
                // string Extension {GenerateSlug}
                Slug_En = createCategoryDto.Name_En.GenerateSlug(),
                Slug_Ar = createCategoryDto.Name_Ar.GenerateSlug(),
                Name_Ar = createCategoryDto.Name_Ar,
                Color = createCategoryDto.Color,
                ImagePath = imagePath
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return (true, null, category);
        }

        public async Task<(bool isSuccess, ErrorResponse? errorMessage, string? response)> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto, int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return (false, new ErrorResponse().CreateErrorResponse("Category not found.", "الفئة غير موجودة."), null);

            category.Name_En = updateCategoryDto.Name_En;
            category.Name_Ar = updateCategoryDto.Name_Ar;
            category.Color = updateCategoryDto.Color;
            category.Slug_En = updateCategoryDto.Name_En.GenerateSlug();
            category.Slug_Ar = updateCategoryDto.Name_Ar.GenerateSlug();

            if (updateCategoryDto.Image != null)
                category.ImagePath = await updateCategoryDto.Image.SaveImageAsync(_fileStorageService);

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return (true, null, "Updated successfully.");
        }

        public async Task<(bool isSuccess, ErrorResponse? errorMessage, object? response)> GetAllCategoriesAsync(
            int page = 1,
            string? query = null)
        {
            if (page < 1)
                return (false, new ErrorResponse().CreateErrorResponse("Page must be greater than 0.", "يجب أن يكون رقم الصفحة أكبر من 0."), null);

            const int pageSize = 10;
            var categoriesQuery = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                categoriesQuery = categoriesQuery
                    .Where(c => c.Name_Ar.Contains(query) || c.Name_En.Contains(query));
            }

            var totalCount = await categoriesQuery.CountAsync();

            var categories = await categoriesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new GetCategoryResponse
                {
                    Id = c.Id,
                    Name_Ar = c.Name_Ar,
                    Name_En = c.Name_En,
                    Slug_Ar = c.Slug_Ar,
                    Slug_En = c.Slug_En,
                    Color = c.Color,
                    Image = c.ImagePath,
                    Number_Of_Products = c.CategoriesProducts.Count,
                })
                .ToListAsync();

            var response = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                items = categories
            };

            return (true, null, categories);
        }

        public async Task<(bool isSuccess, ErrorResponse? errorMessage, object? response)> DeleteCategoryAsync(int id)
        {
            var categoryResult = await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    Category = c,
                    ProductCount = c.CategoriesProducts.Count
                })
                .FirstOrDefaultAsync();

            if (categoryResult == null || categoryResult.ProductCount > 0)
                return (false, new ErrorResponse().CreateErrorResponse("Could not delete the category because it is connected with products.", "لا يمكن حذف الفئة لأنها مرتبطة بمنتجات."), null);

            _context.Categories.Remove(categoryResult.Category);
            await _context.SaveChangesAsync();

            return (true, null, "Deleted successfully.");
        }
    }
}