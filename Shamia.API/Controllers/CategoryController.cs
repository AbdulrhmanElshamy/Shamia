using Google;
using Microsoft.AspNetCore.Mvc;
using Shamia.API.Dtos.Request;
using Shamia.API.Dtos.Response;
using Shamia.API.Services.interFaces;
using System.Threading.Tasks;

namespace Shamia.API.Controllers
{
    /// <summary>
    /// Manages category-related operations and endpoints
    /// </summary>
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Helper method to handle service results
        private IActionResult HandleServiceResult<T>(
            (bool isSuccess, ErrorResponse? errorResponse, T? response) result)
        {
            if (!result.isSuccess || result.errorResponse != null)
                return BadRequest(result.errorResponse);

            return Ok(result.response);
        }

        /// <summary>
        /// Retrieves category details by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details or error response</returns>
        [HttpGet("api/category/{id:int}")]
        public async  Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Retrieves category details by URL slug
        /// </summary>
        /// <param name="slug">Category URL slug</param>
        /// <returns>Category details or error response</returns>
        [HttpGet("api/category/{slug}")]
        public async Task<IActionResult> GetCategoryBySlug(string slug)
        {
            var result = await _categoryService.GetCategoryBySlugAsync(slug);
            return HandleServiceResult(result);
        }

        //// GET: api/category/search
        //[HttpGet("api/category/search")]
        //public async Task<IActionResult> SearchCategories([FromQuery] string query)
        //{
        //    var result = await _categoryService.SearchCategoriesAsync(query);
        //    return HandleServiceResult(result);
        //}

        // POST: api/category/create
        [HttpPost("api/category/create")]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryDto createCategoryDto)
        {
            var result = await _categoryService.CreateCategoryAsync(createCategoryDto);
            if (!result.isSuccess)
                return BadRequest(result.errorMessage);

            return CreatedAtAction(nameof(GetCategoryById), new { id = result.response?.Id }, result.response);
        }

        // PUT: api/category/{id}
        [HttpPost("api/category/{id}")]
        public async Task<IActionResult> UpdateCategory([FromForm] UpdateCategoryDto updateCategoryDto, int id)
        {
            var result = await _categoryService.UpdateCategoryAsync(updateCategoryDto, id);
            return HandleServiceResult(result);
        }

        // GET: api/categories
        [HttpGet("api/categories")]
        public async Task<IActionResult> GetAllCategories(
            [FromQuery] int page = 1,
            [FromQuery] string? query = null)
        {
            var result = await _categoryService.GetAllCategoriesAsync(page, query);
            return HandleServiceResult(result);
        }

        // DELETE: api/category/{id}
        [HttpPost("api/category/delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// /////////////////////////////
        /// </summary>


        //// Get Category by ID
        //[HttpGet("api/category/{id:int}")]
        //public async Task<IActionResult> GetCategoryById(int id)
        //{
        //    var category = await _context.Categories
        //        .Include(c => c.CategoriesProducts).ThenInclude(cp => cp.Product)
        //        .FirstOrDefaultAsync(c => c.Id == id);

        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(category);
        //}

        //[HttpGet("api/category/{slug}")]
        //public async Task<IActionResult> GetCategoryBySlug(string slug)
        //{
        //    var category = await _context.Categories
        //        .Include(c => c.CategoriesProducts)
        //        .ThenInclude(cp => cp.Product)
        //        .FirstOrDefaultAsync(c => c.Slug_En == slug || c.Slug_Ar == slug);

        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(category);
        //}

        //[HttpGet("api/category/search")]
        //public async Task<IActionResult> SearchCategories([FromQuery] string query)
        //{
        //    query = query.ToLower();
        //    if (string.IsNullOrEmpty(query))
        //        return BadRequest("Search query is required.");


        //    // Search for categories by Name_Ar or Name_En
        //    var categories = await _context.Categories
        //        .Include(c => c.CategoriesProducts)
        //            .ThenInclude(cp => cp.Product)
        //                .ThenInclude(p => p.ProductsImages)
        //        .Where(c => c.Name_Ar.Contains(query) || c.Name_En.Contains(query))
        //        .Select(c => new
        //        {
        //            c.Id,
        //            c.Name_Ar,
        //            c.Name_En,
        //            c.Slug_Ar,
        //            c.Slug_En,
        //            Products = c.CategoriesProducts.Select(cp => new
        //            {
        //                cp.Product.Id,
        //                cp.Product.Name_Ar,
        //                cp.Product.Name_En,
        //                cp.Product.Slug_Ar,
        //                cp.Product.Slug_En,
        //                CoverImage = cp.Product.ProductsImages.FirstOrDefault(pi => pi.IsCover == true)!.Path
        //            }).ToList()
        //        })
        //        .ToListAsync();

        //    if (categories == null || !categories.Any())
        //    {
        //        return NotFound("No categories found matching the search criteria.");
        //    }

        //    return Ok(categories);
        //}
        //// Create Category
        //[HttpPost("api/category/create")]
        //public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryDto createCategoryDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }


        //    var imagePath = "";
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        await createCategoryDto.Image.CopyToAsync(memoryStream); // Copy the image to a memory stream
        //        var imageBytes = memoryStream.ToArray(); // Convert the image to a byte array

        //        // Save the image using the file storage service
        //        imagePath = await _fileStorageService.SaveFileAsync(imageBytes);

        //        // Add the image path to the list of ProductImage objects
        //    }
        //    var category = new Category
        //    {
        //        Name_En = createCategoryDto.Name_En,
        //        Slug_En = GenerateSlug(createCategoryDto.Name_En),
        //        Slug_Ar = GenerateSlug(createCategoryDto.Name_Ar),
        //        Name_Ar = createCategoryDto.Name_Ar,
        //        Color = createCategoryDto.Color,
        //        ImagePath = imagePath
        //    };

        //    _context.Categories.Add(category);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        //}

        //// Update Category
        //[HttpPut("api/category/{id}")]
        //public async Task<IActionResult> UpdateCategory([FromForm] UpdateCategoryDto updateCategoryDto, int id)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var category = await _context.Categories.FindAsync(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    category.Name_En = updateCategoryDto.Name_En;
        //    category.Name_Ar = updateCategoryDto.Name_Ar;
        //    category.Color = updateCategoryDto.Color;
        //    category.Slug_En = GenerateSlug(updateCategoryDto.Name_En);
        //    category.Slug_Ar = GenerateSlug(updateCategoryDto.Name_Ar);

        //    if (updateCategoryDto.Image != null)
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await updateCategoryDto.Image.CopyToAsync(memoryStream); // Copy the image to a memory stream
        //            var imageBytes = memoryStream.ToArray(); // Convert the image to a byte array

        //            // Save the image using the file storage service
        //            category.ImagePath = await _fileStorageService.SaveFileAsync(imageBytes);

        //            // Add the image path to the list of ProductImage objects
        //        }
        //    }

        //    _context.Categories.Update(category);
        //    await _context.SaveChangesAsync();

        //    return Ok("Updated Succesfully");
        //}
        //[HttpGet("api/categories")]
        //public async Task<IActionResult> GetAllCategories(
        //    [FromQuery] int page = 1,          // Page number (default: 1)
        //    [FromQuery] string? query = null)  // Optional search query for Name_Ar or Name_En
        //{
        //    // Validate page
        //    if (page < 1)
        //    {
        //        return BadRequest("Page must be greater than 0.");
        //    }

        //    // Hardcoded page size
        //    const int pageSize = 10; // Number of items per page

        //    // Base query
        //    var categoriesQuery = _context.Categories.AsQueryable();

        //    // Apply search filter if query is provided
        //    if (!string.IsNullOrEmpty(query))
        //    {
        //        categoriesQuery = categoriesQuery
        //            .Where(c => c.Name_Ar.Contains(query) || c.Name_En.Contains(query));
        //    }

        //    // Get the total count of categories (for pagination)
        //    var totalCount = await categoriesQuery.CountAsync();

        //    // Apply pagination
        //    var categories = await categoriesQuery
        //        .Skip((page - 1) * pageSize) // Skip items on previous pages
        //        .Take(pageSize)              // Take items for the current page
        //        .Select(c => new GetCategoryResponse
        //        {
        //            Id = c.Id,
        //            Name_Ar = c.Name_Ar,
        //            Name_En = c.Name_En,
        //            Slug_Ar = c.Slug_Ar,
        //            Slug_En = c.Slug_En,
        //            Color = c.Color,
        //            Image = c.ImagePath,
        //            Number_Of_Products = c.CategoriesProducts.Count,
        //        })
        //        .ToListAsync();

        //    // Create a paginated response
        //    var response = new
        //    {
        //        TotalCount = totalCount,
        //        Page = page,
        //        PageSize = pageSize, // Include the hardcoded page size in the response
        //        Categories = categories
        //    };

        //    return Ok(categories);
        //}

        //[HttpDelete("api/category/{id}")]
        //public async Task<IActionResult> DeleteProductById(int id)
        //{
        //    try
        //    {
        //        var categoryResult = await _context.Categories
        //                .Where(c => c.Id == id) // Filter by category ID
        //                .Select(c => new
        //                {
        //                    Category = c,
        //                    ProductCount = c.CategoriesProducts.Count // Count related products
        //                })
        //                .FirstOrDefaultAsync();

        //        if (categoryResult == null || categoryResult.ProductCount <= 0)
        //            return NotFound("Couldnt Delete the category becuase it connected with product");


        //        await _context.SaveChangesAsync();

        //        return Ok("Deleted Successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception (optional)
        //        // _logger.LogError(ex, "An error occurred while deleting the category.");

        //        return BadRequest("Could not delete the category.");
        //    }
        //}



        //private static string GenerateSlug(string name)
        //{
        //    if (string.IsNullOrEmpty(name))
        //        return null;

        //    // Convert to lowercase
        //    var slug = name.ToLower();

        //    // Replace spaces with hyphens
        //    slug = slug.Replace(" ", "-");

        //    // Remove special characters (allow Arabic, English, digits, and hyphens)
        //    slug = Regex.Replace(slug, @"[^\p{IsArabic}a-z0-9\-]", "");

        //    return slug;
        //}
    }
}
