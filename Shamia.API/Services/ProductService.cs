using Microsoft.EntityFrameworkCore;
using Shamia.API.Dtos;
using Shamia.API.Dtos.Request;
using Shamia.API.Dtos.Response;
using Shamia.API.Extension;
using Shamia.API.Services.interFaces;
using Shamia.DataAccessLayer;
using Shamia.DataAccessLayer.Entities;
using System.Linq.Expressions;

namespace Shamia.API.Services
{
    /// <summary>
    /// Manages product catalog operations including creation, updates, and inventory management
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly ShamiaDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        private static readonly Expression<Func<Product, GetProductResponse>> GetProductResponseSelector =
          product => new GetProductResponse
          {
              Id = product.Id,
              Stock = product.Stock,
              Name_Ar = product.Name_Ar,
              Description_En = product.Description_En,
              Name_En = product.Name_En,
              Description_Ar = product.Description_Ar,
              Slug_Ar = product.Slug_Ar,
              Slug_En = product.Slug_En,
              Unit_Of_Measurement_Ar = product.Unit_Of_Measurement_Ar,
              Unit_Of_Measurement_En = product.Unit_Of_Measurement_En,
              Cover_Image = product.ProductsImages
                  .FirstOrDefault(pi => pi.IsCover)!.Path, // Null-forgiving operator
              ImagesPath = product.ProductsImages
                  .Where(pi => !pi.IsCover)
                  .Select(pi => pi.Path)
                  .ToList(),
              Categories = product.CategoriesProducts
                  .Select(cp => new CategoryDto
                  {
                      Id = cp.Category.Id,
                      Name_Ar = cp.Category.Name_Ar,
                      Name_En = cp.Category.Name_En
                  })
                  .ToList(),
              Quantities = product.Quantity // Ensure navigation property exists
                  .Select(x => new GetProductOptionsResponse
                  {
                      Id = x.Id,
                      Name_Ar = product.Unit_Of_Measurement_Ar + " " + x.Quantity_In_Unit,
                      Name_En = product.Unit_Of_Measurement_En + " " + x.Quantity_In_Unit,
                      Price = x.Price,
                      Offer = x.Offer,
                      Default = x.Default,
                      Quantity_In_Unit = x.Quantity_In_Unit,
                  })
                  .ToList()
          };
        public ProductService(ShamiaDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Creates a new product with images, options, and category associations
        /// </summary>
        /// <param name="request">Product creation data transfer object</param>
        /// <returns>Tuple containing success status, error message (if any), and product ID</returns>
        /// <remarks>Handles image uploads, inventory options, and category relationships</remarks>
        public async Task<(bool isSuccess, ErrorResponse? errorMessage, int? productId)> CreateProductAsync(CreateProductRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validate required fields
                if (string.IsNullOrEmpty(request.Name_Ar) || string.IsNullOrEmpty(request.Name_En))
                    return (false, new ErrorResponse().CreateErrorResponse("الإسم مطلوب في اللغة العربية واللغة الإنجليزية", "Arabic name and English name are required."), null);


                bool nameExists = await _context.Products
                    .AnyAsync(p => p.Name_Ar.ToLower() == request.Name_Ar.ToLower() || 
                                p.Name_En.ToLower() == request.Name_En.ToLower());

                if (nameExists)
                    return (false, new ErrorResponse().CreateErrorResponse("اسم المنتج موجود بالفعل", "Product name already exists."), null);


                // 2. Ensure at least one quantity is marked as default
                if (request.Quantities.Count > 0 && !request.Quantities.Any(q => q.Default))
                    request.Quantities[0].Default = true;

                // 3. Create and save the product
                var product = new Product
                {
                    Name_Ar = request.Name_Ar,
                    Slug_Ar = request.Name_Ar.GenerateSlug(),
                    Description_Ar = request.Description_Ar,
                    Name_En = request.Name_En,
                    Slug_En = request.Name_En.GenerateSlug(),
                    Description_En = request.Description_En,
                    Quantity = request.Quantities!,
                    Unit_Of_Measurement_Ar = request.Unit_Of_Measurement_Ar,
                    Unit_Of_Measurement_En = request.Unit_Of_Measurement_En,
                    Stock = request.Stock,
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync(); // Generate the ProductId

                // 4. Link categories (validate category IDs exist in a real-world scenario)
                foreach (var categoryId in request.Categories_Ids)
                {
                    _context.CategoriesProducts.Add(new CategoriesProducts
                    {
                        CategoryId = categoryId,
                        ProductId = product.Id
                    });
                }

                // 5. Add images (ensure HandleAddImages/HandleAddImage handle file storage and DB updates)
                await HandleAddImages(request.ProductImages, product.Id);
                await HandleAddImage(request.CoverImage, product.Id, isCover: true);

                // Final save and commit
                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); // All steps succeeded

                return (true, null, product.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback on failure
                                                   // Log the exception here
                return (false, new ErrorResponse().CreateErrorResponse("فشل إنشاء المنتج", ex.Message), null);
            }
        }

        /// <summary>
        /// Updates an existing product and its associated data
        /// </summary>
        /// <param name="id">Product ID to update</param>
        /// <param name="request">Product update data transfer object</param>
        /// <returns>Tuple containing success status, error message (if any), and product ID</returns>
        /// <remarks>Manages image updates, inventory changes, and category modifications</remarks>
        public async Task<(bool isSuccess, ErrorResponse? errorMessage, int? productId)> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(request.Name_Ar) || string.IsNullOrEmpty(request.Name_En))
                    return (false, new ErrorResponse().CreateErrorResponse("الإسم مطلوب في اللغة العربية واللغة الإنجليزية", "Arabic name and English name are required."), null);

                var product = await _context.Products
                    .Include(p => p.ProductsImages)
                    .Include(p => p.Quantity)
                    .Include(p => p.CategoriesProducts)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                    return (false, new ErrorResponse().CreateErrorResponse("المنتج غير موجود", "Product not found."), null);

                product.Name_Ar = request.Name_Ar;
                product.Slug_Ar = request.Name_Ar.GenerateSlug();
                product.Description_Ar = request.Description_Ar;
                product.Name_En = request.Name_En;
                product.Slug_En = request.Name_En.GenerateSlug();
                product.Description_En = request.DescriptionEn;
                product.Stock = request.Stock;


                Console.WriteLine("Logging cover Image");
                Console.WriteLine(request.CoverImage);

                if (product.Quantity == null)
                    product.Quantity = new List<ProductOptions>();

                if (request.Quantities != null && request.Quantities.Any())
                {
                    var uniqueRequestQuantities = request.Quantities
                        .GroupBy(q => q.Quantity_In_Unit)
                        .Select(g => g.First())
                        .ToList();

                    foreach (var reqQuantity in uniqueRequestQuantities)
                    {
                        var existingQuantity = product.Quantity.FirstOrDefault(q => q.Quantity_In_Unit == reqQuantity.Quantity_In_Unit);

                        if (existingQuantity != null)
                        {
                            existingQuantity.Price = reqQuantity.Price;
                            existingQuantity.Offer = reqQuantity.Offer;
                            existingQuantity.Default = reqQuantity.Default;

                            _context.Entry(existingQuantity).State = EntityState.Modified;
                        }
                        else
                        {
                            var newQuantity = new ProductOptions
                            {
                                Quantity_In_Unit = reqQuantity.Quantity_In_Unit,
                                Price = reqQuantity.Price,
                                Offer = reqQuantity.Offer,
                                Default = reqQuantity.Default,
                                ProductId = product.Id
                            };

                            product.Quantity.Add(newQuantity);
                            _context.Entry(newQuantity).State = EntityState.Added;
                        }
                    }

                    var incomingQuantityUnits = uniqueRequestQuantities.Select(q => q.Quantity_In_Unit).ToList();
                    var quantitiesToRemove = product.Quantity
                        .Where(q => !incomingQuantityUnits.Contains(q.Quantity_In_Unit))
                        .ToList();

                    foreach (var qtyToRemove in quantitiesToRemove)
                    {
                        if (qtyToRemove.OrderDetailsList == null || !qtyToRemove.OrderDetailsList.Any())
                        {
                            _context.ProductsOptions.Remove(qtyToRemove);
                        }
                    }
                }


                var imagesToDelete = product.ProductsImages
                    .Where(p => !request.Product_Images_Paths.Contains(p.Path) && !p.IsCover)
                    .ToList();

                foreach (var image in imagesToDelete)
                    product.ProductsImages.Remove(image);

                if (request.CoverImage != null)
                {
                    Console.WriteLine("updatingCoverImage : " + request.CoverImage);
                    var oldCoverImage = product.ProductsImages.SingleOrDefault(pi => pi.IsCover);
                    _context.ProductsImages.Remove(oldCoverImage!);
                    await HandleAddImage(request.CoverImage, product.Id, true);
                }

                await HandleAddImages(request.Product_New_Images, product.Id);

                var existingCategories = product.CategoriesProducts.ToList();
                _context.CategoriesProducts.RemoveRange(existingCategories);

                foreach (var categoryId in request.Categories_Ids)
                {
                    _context.CategoriesProducts.Add(new CategoriesProducts
                    {
                        CategoryId = categoryId,
                        ProductId = product.Id
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, null, product.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback on failure
                                                   // Log the exception here
                return (false, new ErrorResponse().CreateErrorResponse("فشل إنشاء المنتج", ex.Message), null);
            }
        }

        /// <summary>
        /// Retrieves product details by slug with similar product recommendations
        /// </summary>
        /// <param name="slug">URL-friendly product identifier</param>
        /// <returns>Tuple containing success status, error message (if any), and product data</returns>
        public async Task<(bool isSuccess, ErrorResponse? errorMessage, GetProductBySlugResponse? response)> GetProductBySlugAsync(string slug)
        {
            var product = await _context.Products
                .Include(p => p.Quantity)
                .Include(p => p.ProductsImages)
                .Include(p => p.CategoriesProducts)
                    .ThenInclude(cp => cp.Category)
                .Select(GetProductResponseSelector)
                .FirstOrDefaultAsync(p => p.Slug_En.Contains(slug) || p.Slug_Ar.Contains(slug));

            if (product == null)
                return (false, new ErrorResponse().CreateErrorResponse("المنتج غير موجود", "Product not found."), null);

            var categoryIds = product.Categories.Select(c => c.Id).ToList();

            var similarProducts = await GetSimilarProducts(product.Id, categoryIds);

            var response = new GetProductBySlugResponse
            {
                product = product,
                similarProducts = similarProducts
            };

            return (true, null, response);
        }
        private async Task<List<GetProductResponse>> GetSimilarProducts(int productId, List<int> categoryIds)
        {
            var similarProducts = await _context.Products
                .Include(p => p.Quantity)
                .Include(p => p.ProductsImages)
                .Include(p => p.CategoriesProducts)
                .ThenInclude(cp => cp.Category)
                .Where(p => p.Id != productId && p.CategoriesProducts.Any(cp => categoryIds.Contains(cp.CategoryId)) && !p.IsEnded)
                .Select(GetProductResponseSelector)
                .Take(6)
                .ToListAsync();

            return similarProducts;
        }

        public async Task<(bool isSuccess, ErrorResponse? errorMessage, List<GetProductResponse>? products)> SearchProductsAsync(string query, int? categoryId)
        {
            if (string.IsNullOrEmpty(query) && !categoryId.HasValue)
                return (false, new ErrorResponse().CreateErrorResponse("يجب تقديم استعلام البحث أو معرف الفئة", "Search query or category ID is required."), null);

            var productsQuery = _context.Products
                .Include(p => p.Quantity)
                .Include(p => p.ProductsImages)
                .Include(p => p.CategoriesProducts)
                    .ThenInclude(cp => cp.Category)
                .Where(p => !p.IsEnded);

            if (categoryId.HasValue)
                productsQuery = productsQuery.Where(p => p.CategoriesProducts.Any(cp => cp.CategoryId == categoryId.Value));

            if (!string.IsNullOrEmpty(query))
                productsQuery = productsQuery.Where(p => p.Name_Ar.Contains(query) || p.Name_En.Contains(query));

            var products = await productsQuery
                .Select(GetProductResponseSelector)
                .ToListAsync();

            if (products == null || !products.Any())
                return (false, new ErrorResponse().CreateErrorResponse("لم يتم العثور على منتجات", "No products found matching the search criteria."), null);

            return (true, null, products);
        }

        public async Task<(bool isSuccess, ErrorResponse? errorMessage, string? message)> DeleteProductAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductsImages)
                .Include(p => p.Quantity)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return (false, new ErrorResponse().CreateErrorResponse("المنتج غير موجود", $"Product with ID {id} not found."), null);

            product.IsEnded = true;

            await _context.SaveChangesAsync();

            return (true, null, "Deleted Successfully");
        }

        public async Task<(bool isSuccess, ErrorResponse? errorMessage, List<GetProductResponse>? products)> GetProductsByCategorySlugAsync(string slug, int page)
        {
            if (page < 1)
                return (false, new ErrorResponse().CreateErrorResponse("يجب أن تكون الصفحة أكبر من 0", "Page must be greater than 0."), null);

            var pageSize = 12;

            var query = _context.Products
                .Include(p => p.ProductsImages)
                .Include(p => p.Quantity)
                .Include(p => p.CategoriesProducts)
                    .ThenInclude(cp => cp.Category)
                .Where(p => (p.CategoriesProducts.Select(cp => cp.Category.Slug_En).FirstOrDefault() == slug ||
                             p.CategoriesProducts.Select(cp => cp.Category.Slug_Ar).FirstOrDefault() == slug) &&
                            !p.IsEnded);

            var totalCount = await query.CountAsync();

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(GetProductResponseSelector)
                .ToListAsync();

            if (products == null || products.Count == 0)
                return (true, null, products);
            var pagedResponse = PagedList<GetProductResponse>.Create(products, page, pageSize, totalCount);

            return (true, null, products);
        }

        /// <summary>
        /// Retrieves randomly selected products as "most selling" placeholder
        /// </summary>
        /// <returns>Tuple containing success status, error message (if any), and product list</returns>
        /// <remarks>Currently returns random products - should be updated with actual sales data</remarks>
        public async Task<(bool isSuccess, ErrorResponse? errorMessage, List<GetProductResponse>? products)> GetMostSellingProductsAsync()
        {
            try
            {
                var randomProducts = await _context.Products
                    .Include(p => p.ProductsImages)
                    .Include(p => p.Quantity)
                    .Include(p => p.CategoriesProducts)
                        .ThenInclude(cp => cp.Category)
                    .Where(p => !p.IsEnded)
                    .OrderBy(p => Guid.NewGuid())
                    .Take(4)
                    .Select(GetProductResponseSelector)
                    .ToListAsync();

                return (true, null, randomProducts);
            }
            catch (Exception ex)
            {
                return (false, new ErrorResponse().CreateErrorResponse("حدث خطأ أثناء جلب المنتجات الأكثر مبيعًا", "An error occurred while fetching most selling products."), null);
            }
        }

        /// <summary>
        /// Retrieves a paginated list of products with filtering options
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="categoryId">Optional category filter</param>
        /// <param name="query">Optional search query filter</param>
        /// <returns>Tuple containing success status, error message (if any), and paged product list</returns>
        public async Task<(bool isSuccess, ErrorResponse? errorMessage, PagedList<GetProductResponse>? products)> GetAllProductsAsync(
            int page = 1,
            int? categoryId = null,
            string? query = null)
        {
            try
            {
                //if (page < 1)
                //    return (false, new ErrorResponse().CreateErrorResponse("يجب أن تكون الصفحة أكبر من 0", "Page must be greater than 0."), null);

                var pageSize = 12;

                var productsQuery = _context.Products
                    .Include(p => p.ProductsImages)
                    .Include(p => p.Quantity)
                    .Include(p => p.CategoriesProducts)
                        .ThenInclude(cp => cp.Category)
                    .Where(p => !p.IsEnded);

                if (categoryId.HasValue)
                    productsQuery = productsQuery.Where(p => p.CategoriesProducts.Any(cp => cp.CategoryId == categoryId.Value));

                if (!string.IsNullOrEmpty(query))
                    productsQuery = productsQuery.Where(p => p.Name_Ar.Contains(query) || p.Name_En.Contains(query));

                var totalCount = await productsQuery.CountAsync();

                var products = await productsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(GetProductResponseSelector)
                    .ToListAsync();

                var pagedResponse = PagedList<GetProductResponse>.Create(products, page, pageSize, totalCount);

                return (true, null, pagedResponse);
            }
            catch (Exception ex)
            {
                return (false, new ErrorResponse().CreateErrorResponse("حدث خطأ أثناء جلب المنتجات", "An error occurred while fetching products."), null);
            }
        }


        private async Task HandleAddImage(IFormFile img, int productId, bool isCover = false)
        {
            var imagePath = await img.SaveImageAsync(_fileStorageService);
            _context.ProductsImages.Add(new ProductImage
            {
                Path = imagePath,
                IsCover = isCover,
                ProductId = productId
            });
        }

        private async Task HandleAddImages(List<IFormFile> productImages, int productId)
        {
            foreach (var img in productImages)
                await HandleAddImage(img, productId);
        }
    }
}