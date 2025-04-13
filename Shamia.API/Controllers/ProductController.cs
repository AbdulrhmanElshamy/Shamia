using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Shamia.API.Dtos;
using Shamia.API.Dtos.Request;
using Shamia.API.Dtos.Response;
using Shamia.API.Services.interFaces;
using Shamia.DataAccessLayer;
using Shamia.DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shamia.API.Controllers
{
    /// <summary>
    /// Manages product catalog operations and endpoints
    /// </summary>
    public class ProductController : ControllerBase
    {
        private readonly ShamiaDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IProductService _productService;

        public ProductController(ShamiaDbContext context, IFileStorageService fileStorageService, IProductService productService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _productService = productService;
        }


        //[HttpPost("api/product/create")]
        //public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
        //{
        //    if (string.IsNullOrEmpty(request.Name_Ar) || string.IsNullOrEmpty(request.Name_En))
        //        return BadRequest("Arabic name and English name are required.");

        //    var quantities = new List<ProductOptions>();
        //    var form = await Request.ReadFormAsync();

        //    // Loop through form fields to find quantities
        //    foreach (var key in form.Keys)
        //    {
        //        if (key.StartsWith("Quantities"))
        //        {
        //            // Parse the JSON string into a Quantity object
        //            var quantityJson = form[key];
        //            var quantity = JsonConvert.DeserializeObject<ProductOptions>(quantityJson);
        //            quantities.Add(quantity);
        //        }
        //    }

        //    // Create the product entity
        //    var product = new Product
        //    {
        //        Name_Ar = request.Name_Ar,
        //        Slug_Ar = GenerateSlug(request.Name_Ar),
        //        Description_Ar = request.Description_Ar,
        //        Name_En = request.Name_En,
        //        Slug_En = GenerateSlug(request.Name_En),
        //        Description_En = request.Description_En,
        //        Quantity = quantities,
        //    };

        //    // Save the product to the database
        //    _context.Products.Add(product);
        //    await _context.SaveChangesAsync();

        //    // Process dynamic category fields (e.g., Category0, Category1, etc.)
        //    foreach (var key in form.Keys)
        //    {
        //        if (key.StartsWith("Category"))
        //        {
        //            if (int.TryParse(form[key], out int categoryId))
        //            {
        //                _context.CategoriesProducts.Add(new CategoriesProducts
        //                {
        //                    CategoryId = categoryId,
        //                    ProductId = product.Id
        //                });
        //            }
        //        }
        //    }

        //    // Handle cover image
        //    if (request.CoverImage != null && request.CoverImage.Length > 0)
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await request.CoverImage.CopyToAsync(memoryStream);
        //            var imageBytes = memoryStream.ToArray();
        //            var imagePath = await _fileStorageService.SaveFileAsync(imageBytes);

        //            _context.ProductsImages.Add(new ProductImage
        //            {
        //                Path = imagePath,
        //                IsCover = true,
        //                ProductId = product.Id
        //            });
        //        }
        //    }

        //    // Handle additional images
        //    for (int i = 0; ; i++)
        //    {
        //        var imageFieldName = $"Images{i}"; // e.g., Image0, Image1, etc.
        //        if (!Request.Form.Files.Any(f => f.Name == imageFieldName))
        //        {
        //            break; // Stop when no more images are found
        //        }

        //        var image = Request.Form.Files.First(f => f.Name == imageFieldName);

        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await image.CopyToAsync(memoryStream);
        //            var imageBytes = memoryStream.ToArray();
        //            var imagePath = await _fileStorageService.SaveFileAsync(imageBytes);

        //            _context.ProductsImages.Add(new ProductImage
        //            {
        //                Path = imagePath,
        //                IsCover = false,
        //                ProductId = product.Id
        //            });
        //        }
        //    }

        //    await _context.SaveChangesAsync();

        //    return Ok(new { ProductId = product.Id });
        //}

        ////[HttpPut("api/product/{id}")]
        ////public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductRequest request)
        ////{
        ////    var product = await _context.Products
        ////        .Include(p => p.ProductsImages)
        ////        .Include(p => p.Quantity)
        ////        .Include(p => p.CategoriesProducts)
        ////        .FirstOrDefaultAsync(p => p.Id == id);

        ////    if (product == null)
        ////    {
        ////        return NotFound("Product not found.");
        ////    }

        ////    if (string.IsNullOrEmpty(request.Name_Ar) || string.IsNullOrEmpty(request.Name_En))
        ////    {
        ////        return BadRequest("Arabic name and English name are required.");
        ////    }

        ////    // Update product details
        ////    product.Name_Ar = request.Name_Ar;
        ////    product.Slug_Ar = GenerateSlug(request.Name_Ar);
        ////    product.Description_Ar = request.Description_Ar;
        ////    product.Name_En = request.Name_En;
        ////    product.Slug_En = GenerateSlug(request.Name_En);
        ////    product.Description_En = request.DescriptionEn;

        ////    // Update quantities
        ////    if (request.Quantities != null)
        ////    {
        ////         _context.ProductsOptions.RemoveRange(product.Quantity);

        ////        foreach (var quantity in request.Quantities)
        ////        {
        ////            product.Quantity.Add(new ProductOptions
        ////            {
        ////                Price = quantity.Price,
        ////                Offer = quantity.Offer,
        ////                Default = quantity.Default,
        ////                Quantity_In_Unit = quantity.Quantity_In_Unit,
        ////                ProductId = product.Id
        ////            });
        ////        }
        ////    }

        ////    // Handle cover image
        ////    if (request.CoverImage != null && request.CoverImage.Length > 0)
        ////    {
        ////        // Delete the old cover image
        ////        var oldCoverImage = product.ProductsImages.FirstOrDefault(pi => pi.IsCover);
        ////        if (oldCoverImage != null)
        ////        {
        ////            _context.ProductsImages.Remove(oldCoverImage);
        ////        }

        ////        // Add the new cover image
        ////        using (var memoryStream = new MemoryStream())
        ////        {
        ////            await request.CoverImage.CopyToAsync(memoryStream);
        ////            var imageBytes = memoryStream.ToArray();
        ////            var imagePath = await _fileStorageService.SaveFileAsync(imageBytes);

        ////            _context.ProductsImages.Add(new ProductImage
        ////            {
        ////                Path = imagePath,
        ////                IsCover = true,
        ////                ProductId = product.Id
        ////            });
        ////        }
        ////    }

        ////    // Handle additional images
        ////    var existingImages = product.ProductsImages.Where(pi => !pi.IsCover).ToList();
        ////    _context.ProductsImages.RemoveRange(existingImages);

        ////    for (int i = 0; ; i++)
        ////    {
        ////        var imageFieldName = $"Image{i}";
        ////        if (!Request.Form.Files.Any(f => f.Name == imageFieldName))
        ////        {
        ////            break; // Stop when no more images are found
        ////        }

        ////        var image = Request.Form.Files.First(f => f.Name == imageFieldName);

        ////        using (var memoryStream = new MemoryStream())
        ////        {
        ////            await image.CopyToAsync(memoryStream);
        ////            var imageBytes = memoryStream.ToArray();
        ////            var imagePath = await _fileStorageService.SaveFileAsync(imageBytes);

        ////            _context.ProductsImages.Add(new ProductImage
        ////            {
        ////                Path = imagePath,
        ////                IsCover = false,
        ////                ProductId = product.Id
        ////            });
        ////        }
        ////    }

        ////    // Update categories
        ////    var form = await Request.ReadFormAsync();
        ////    var existingCategories = product.CategoriesProducts.ToList();
        ////    _context.CategoriesProducts.RemoveRange(existingCategories);

        ////    foreach (var key in form.Keys)
        ////    {
        ////        if (key.StartsWith("Category"))
        ////        {
        ////            if (int.TryParse(form[key], out int categoryId))
        ////            {
        ////                _context.CategoriesProducts.Add(new CategoriesProducts
        ////                {
        ////                    CategoryId = categoryId,
        ////                    ProductId = product.Id
        ////                });
        ////            }
        ////        }
        ////    }

        ////    await _context.SaveChangesAsync();

        ////    return Ok(new { ProductId = product.Id });
        ////}

        //[HttpGet("api/product/{slug}")]
        //public async Task<IActionResult> GetProductBySlug(string slug)
        //{
        //    // Find the product by slug
        //    var product = await _context.Products
        //        .Include(p => p.Quantity)
        //        .Include(p => p.ProductsImages)
        //        .Include(p => p.CategoriesProducts)
        //            .ThenInclude(cp => cp.Category)
        //        .FirstOrDefaultAsync(p => p.Slug_En.Contains(slug)  || p.Slug_Ar.Contains(slug));

        //    if (product == null)
        //    {
        //        return NotFound("Product not found.");
        //    }

        //    // Map the product to the response DTO
        //    var productDto = new GetProductResponse
        //    {
        //        Id = product.Id,
        //        Stock = product.Stock,
        //        Name_Ar = product.Name_Ar,
        //        Description_En = product.Description_En,
        //        Name_En = product.Name_En,
        //        Description_Ar = product.Description_Ar,
        //        Slug_Ar = product.Slug_Ar,
        //        Slug_En = product.Slug_En,
        //        Cover_Image = product.ProductsImages.FirstOrDefault(pi => pi.IsCover == true)?.Path!,
        //        ImagesPath = product.ProductsImages.Where(pi => pi.IsCover == false).Select(pi => pi.Path).ToList(),
        //        Categories = product.CategoriesProducts
        //            .Select(cp => new CategoryDto // Map each category to CategoryDto
        //            {
        //                Id = cp.Category.Id,
        //                Name_Ar = cp.Category.Name_Ar,
        //                Name_En = cp.Category.Name_En
        //            }).ToList(),
        //        Quantities = await _context.ProductsOptions

        //            .Where(po => po.ProductId == product.Id)
        //            .Select(x => new GetProductOptionsResponse
        //            {
        //                Id = x.Id,
        //                Name_Ar = product.Unit_Of_Measurement_Ar + " " + x.Quantity_In_Unit,
        //                Name_En = product.Unit_Of_Measurement_En + " " + x.Quantity_In_Unit,
        //                Price = x.Price,
        //                Offer = x.Offer,
        //                Quantitiy_In_Unit = x.Quantity_In_Unit,
        //                Default = x.Default,
        //            })
        //            .ToListAsync()
        //    };

        //    // Get similar products (up to 6)
        //    var similarProducts = await _context.Products
        //        .Include(p => p.Quantity)
        //        .Include(p => p.ProductsImages)
        //        .Include(p => p.CategoriesProducts)
        //            .ThenInclude(cp => cp.Category)
        //        .Where(p => p.CategoriesProducts.Any(cp => cp.CategoryId == product.CategoriesProducts.First().CategoryId) && p.Id != product.Id) // Same category, exclude current product
        //        .Take(6) // Limit to 6 products
        //        .Select(p => new GetProductResponse
        //        {
        //            Id = p.Id,
        //            Name_Ar = p.Name_Ar,
        //            Description_En = p.Description_En,
        //            Name_En = p.Name_En,
        //            Description_Ar = p.Description_Ar,
        //            Slug_Ar = p.Slug_Ar,
        //            Slug_En = p.Slug_En,
        //            Cover_Image = p.ProductsImages.FirstOrDefault(pi => pi.IsCover == true)!.Path,
        //            ImagesPath = p.ProductsImages.Where(pi => pi.IsCover == false).Select(pi => pi.Path).ToList(),
        //            Categories = p.CategoriesProducts
        //            .Select(cp => new CategoryDto // Map each category to CategoryDto
        //            {
        //                Id = cp.Category.Id,
        //                Name_Ar = cp.Category.Name_Ar,
        //                Name_En = cp.Category.Name_En
        //            }).ToList(),
        //            Quantities = _context.ProductsOptions
        //                .Where(po => po.ProductId == p.Id)
        //                .Select(x => new GetProductOptionsResponse
        //                {
        //                    Id = x.Id,
        //                    Name_Ar = p.Unit_Of_Measurement_Ar,
        //                    Name_En = p.Unit_Of_Measurement_En,
        //                    Quantitiy_In_Unit =  x.Quantity_In_Unit,
        //                    Price = x.Price,
        //                    Offer = x.Offer,
        //                    Default = x.Default,
        //                })
        //                .ToList()
        //        }).ToListAsync();

        //    // Create the final response object
        //    var response = new
        //    {
        //        product = productDto,
        //        similarProducts
        //    };

        //    return Ok(response);
        //}

        //[HttpGet("api/product/search")]
        //public async Task<IActionResult> SearchProducts([FromQuery] string query, [FromQuery] int? categoryid)
        //{
        //    if (string.IsNullOrEmpty(query) && !categoryid.HasValue)
        //    {
        //        return BadRequest("Search query or category ID is required.");
        //    }

        //    // Base query
        //    var productsQuery = _context.Products
        //        .Include(p => p.Quantity)
        //        .Include(p => p.ProductsImages)
        //        .Include(p => p.CategoriesProducts)
        //            .ThenInclude(cp => cp.Category)
        //        .Where(p => !p.IsEnded); // Only active products

        //    // Filter by category if categoryid is provided
        //    if (categoryid.HasValue)
        //    {
        //        productsQuery = productsQuery
        //            .Where(p => p.CategoriesProducts.Any(cp => cp.CategoryId == categoryid.Value));
        //    }

        //    // Filter by search query if provided
        //    if (!string.IsNullOrEmpty(query))
        //    {
        //        productsQuery = productsQuery
        //            .Where(p => p.Name_Ar.Contains(query) || p.Name_En.Contains(query));
        //    }

        //    // Execute the query and map to GetProductResponse
        //    var products = await productsQuery
        //        .Select(p => new GetProductResponse
        //        {
        //            Id = p.Id,
        //            Stock = p.Stock,
        //            Name_Ar = p.Name_Ar,
        //            Description_En = p.Description_En,
        //            Name_En = p.Name_En,
        //            Description_Ar = p.Description_Ar,
        //            Slug_Ar = p.Slug_Ar,
        //            Slug_En = p.Slug_En,
        //            Cover_Image = p.ProductsImages.FirstOrDefault(pi => pi.IsCover == true)!.Path,
        //            ImagesPath = p.ProductsImages.Where(pi => pi.IsCover == false).Select(pi => pi.Path).ToList(),
        //            Categories = p.CategoriesProducts
        //                .Select(cp => new CategoryDto
        //                {
        //                    Id = cp.Category.Id,
        //                    Name_Ar = cp.Category.Name_Ar,
        //                    Name_En = cp.Category.Name_En
        //                })
        //                .ToList(),
        //            Quantities = _context.ProductsOptions
        //                .Where(po => po.ProductId == p.Id)
        //                .Select(x => new GetProductOptionsResponse
        //                {
        //                    Id = x.Id,
        //                    Name_Ar = p.Unit_Of_Measurement_Ar + " " + x.Quantity_In_Unit,
        //                    Name_En = p.Unit_Of_Measurement_En + " " + x.Quantity_In_Unit,
        //                    Quantitiy_In_Unit = x.Quantity_In_Unit,
        //                    Price = x.Price,
        //                    Offer = x.Offer,
        //                    Default = x.Default,
        //                })
        //                .ToList()
        //        })
        //        .ToListAsync();

        //    if (products == null || !products.Any())
        //        return NotFound("No products found matching the search criteria.");

        //    return Ok(products);
        //}

        //[HttpDelete("api/product/{id}")]
        //public async Task<IActionResult> DeleteProduct(int id)
        //{

        //    // Check if the product exists
        //    var product = await _context.Products
        //        .Include(p => p.ProductsImages) // Include related images
        //        .Include(p => p.Quantity) // Include related quantities
        //        .FirstOrDefaultAsync(p => p.Id == id);

        //    if (product == null)
        //        return NotFound($"Product with ID {id} not found.");

        //    product.IsEnded = true;

        //    await _context.SaveChangesAsync();

        //    return Ok("Deleted Successfully");

        //}

        //[HttpGet("api/products/category/{slug}")]
        //public async Task<IActionResult> GetProductsByCategorySlug(string slug, [FromQuery] int page = 1)
        //{
        //    // Validate page
        //    if (page < 1)
        //        return BadRequest("Page must be greater than 0.");

        //    var pageSize = 12;

        //    // Query products that match the category slug (either Slug_En or Slug_Ar)
        //    var query = _context.Products
        //        .Include(p => p.ProductsImages)
        //        .Include(p => p.Quantity)
        //        .Include(p => p.CategoriesProducts)
        //            .ThenInclude(cp => cp.Category)// Include related images
        //        .Include(p => p.Quantity)
        //        .Where(p => p.CategoriesProducts.Select(cp => cp.Category.Slug_En).FirstOrDefault() == slug ||
        //                    p.CategoriesProducts.Select(cp => cp.Category.Slug_Ar).FirstOrDefault() == slug && 
        //                !p.IsEnded
        //        );

        //    // Get the total count of products matching the slug
        //    var totalCount = await query.CountAsync();

        //    // Get paginated products
        //    var products = await query
        //        .Skip((page - 1) * pageSize) // Skip items on previous pages
        //        .Take(pageSize) // Take items for the current page
        //        .Select(p => new GetProductResponse
        //        {
        //            Id = p.Id,
        //            Stock = p.Stock,
        //            Name_Ar = p.Name_Ar,
        //            Description_En = p.Description_En,
        //            Name_En = p.Name_En,
        //            Slug_Ar = p.Slug_Ar,
        //            Slug_En = p.Slug_En,
        //            Categories = p.CategoriesProducts
        //            .Select(cp => new CategoryDto // Map each category to CategoryDto
        //            {
        //                Id = cp.Category.Id,
        //                Name_Ar = cp.Category.Name_Ar,
        //                Name_En = cp.Category.Name_En
        //            }).ToList(),
        //            Description_Ar = p.Description_Ar,
        //            Cover_Image = p.ProductsImages.FirstOrDefault(pi => pi.IsCover == true).Path,
        //            ImagesPath = p.ProductsImages.Where(pi => pi.IsCover == false).Select(pi => pi.Path).ToList(),
        //            Quantities = p.Quantity.Select(q => new GetProductOptionsResponse
        //            {
        //                Id = q.Id,
        //                Name_Ar = p.Unit_Of_Measurement_Ar + " " +  q.Quantity_In_Unit,
        //                Name_En = p.Unit_Of_Measurement_En + " " + q.Quantity_In_Unit ,
        //                Quantitiy_In_Unit = q.Quantity_In_Unit,
        //                Price = q.Price,
        //                Offer = q.Offer,
        //                Default = q.Default,
        //            }).ToList()
        //        }).ToListAsync();

        //    // Create a paginated response
        //    var pagedResponse = PagedList<GetProductResponse>.Create(products, page, pageSize, totalCount);

        //    return Ok(new { items = pagedResponse.Items });
        //}

        //[HttpGet("api/products/mostselling")]
        //public async Task<IActionResult> GetMostSellingProducts()
        //{
        //    // الحصول على 4 منتجات عشوائية من جدول Products
        //    var randomProducts = await _context.Products
        //        .Include(p => p.ProductsImages)
        //        .Include(p => p.Quantity)
        //        .Include(p => p.CategoriesProducts)
        //            .ThenInclude(cp => cp.Category)
        //        .Include(p => p.Quantity)
        //        .OrderBy(p => Guid.NewGuid()) // ترتيب عشوائي
        //        .Take(4) // إرجاع 4 منتجات فقط
        //        .Select(p => new GetProductResponse
        //        {
        //            Id = p.Id,
        //            Stock = p.Stock,
        //            Name_Ar = p.Name_Ar,
        //            Description_En = p.Description_En,
        //            Name_En = p.Name_En,
        //            Slug_Ar = p.Slug_Ar,
        //            Slug_En = p.Slug_En,
        //            Categories = p.CategoriesProducts
        //            .Select(cp => new CategoryDto // Map each category to CategoryDto
        //            {
        //                Id = cp.Category.Id,
        //                Name_Ar = cp.Category.Name_Ar,
        //                Name_En = cp.Category.Name_En
        //            }).ToList(),
        //            Description_Ar = p.Description_Ar,
        //            Cover_Image = p.ProductsImages.FirstOrDefault(pi => pi.IsCover == true)!.Path,
        //            ImagesPath = p.ProductsImages.Where(pi => pi.IsCover == false).Select(pi => pi.Path).ToList(),
        //            Quantities = p.Quantity.Select(q => new GetProductOptionsResponse
        //            {
        //                Id = q.Id,
        //                Name_Ar = p.Unit_Of_Measurement_Ar + " " + q.Quantity_In_Unit,
        //                Name_En = p.Unit_Of_Measurement_En + " " + q.Quantity_In_Unit,
        //                Quantitiy_In_Unit = q.Quantity_In_Unit,
        //                Price = q.Price,
        //                Offer = q.Offer,
        //                Default = q.Default,
        //            }).ToList(),
        //        })
        //        .ToListAsync();

        //    return Ok(randomProducts);
        //}

        //[HttpGet("api/products")]
        //public async Task<IActionResult> GetAllProducts(
        //[FromQuery] int page = 1,
        //[FromQuery] int? categoryid = null,
        //[FromQuery] string? query = null) // Add name as an optional query parameter
        //{
        //    // Validate page
        //    if (page < 1)
        //        return BadRequest("Page must be greater than 0.");

        //    var pageSize = 12;

        //    // Base query
        //    var productsQuery = _context.Products
        //        .Include(p => p.ProductsImages)
        //        .Include(p => p.Quantity)
        //        .Include(p => p.CategoriesProducts)
        //            .ThenInclude(cp => cp.Category)
        //        .Include(p => p.Quantity)
        //        .Where(p => !p.IsEnded); // Only active products

        //    // Filter by category if categoryid is provided
        //    if (categoryid.HasValue)
        //    {
        //        productsQuery = productsQuery
        //            .Where(p => p.CategoriesProducts.Any(cp => cp.CategoryId == categoryid.Value));
        //    }

        //    // Filter by name if name is provided
        //    if (!string.IsNullOrEmpty(query))
        //    {
        //        productsQuery = productsQuery
        //            .Where(p => p.Name_Ar.Contains(query) || p.Name_En.Contains(query));
        //    }

        //    // Get the total count of products
        //    var totalCount = await productsQuery.CountAsync();


        //    // Get paginated products
        //    var products = await productsQuery
        //        .Skip((page - 1) * pageSize) // Skip items on previous pages
        //        .Take(pageSize) // Take items for the current page
        //        .Select(p => new GetProductResponse
        //        {
        //            Id = p.Id,
        //            Stock = p.Stock,
        //            Name_Ar = p.Name_Ar,
        //            Description_En = p.Description_En,
        //            Name_En = p.Name_En,
        //            Description_Ar = p.Description_Ar,
        //            Categories = p.CategoriesProducts
        //                .Select(cp => new CategoryDto
        //                {
        //                    Id = cp.Category.Id,
        //                    Name_Ar = cp.Category.Name_Ar,
        //                    Name_En = cp.Category.Name_En
        //                })
        //                .ToList(),
        //            Slug_Ar = p.Slug_Ar,
        //            Slug_En = p.Slug_En,
        //            Cover_Image = p.ProductsImages.FirstOrDefault(pi => pi.IsCover == true).Path,
        //            ImagesPath = p.ProductsImages.Where(pi => pi.IsCover == false).Select(pi => pi.Path).ToList(),
        //            Quantities = p.Quantity.Select(q => new GetProductOptionsResponse
        //            {
        //                Id = q.Id,
        //                Name_Ar = p.Unit_Of_Measurement_Ar + " " + q.Quantity_In_Unit,
        //                Name_En = p.Unit_Of_Measurement_En + " " + q.Quantity_In_Unit,
        //                Price = q.Price,
        //                Offer = q.Offer,
        //                Default = q.Default,
        //                Quantitiy_In_Unit = q.Quantity_In_Unit
        //            }) .ToList(),
        //        })
        //        .ToListAsync();

        //    // Create a paginated response
        //    var pagedResponse = PagedList<GetProductResponse>.Create(products, page, pageSize, totalCount);

        //    return Ok(pagedResponse);
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

        /////////


        [HttpGet("api/product/{slug}")]
        public async Task<IActionResult> GetProductBySlug(string slug)
        {
            var result = await _productService.GetProductBySlugAsync(slug);
            return HandleServiceResult(result);

        }

        [HttpPost("api/product/create")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
        {
            if (string.IsNullOrEmpty(request.Name_Ar) || string.IsNullOrEmpty(request.Name_En))
                return BadRequest("Arabic name and English name are required.");

            var form = await Request.ReadFormAsync();

            request.Quantities = ExtractFormData<ProductOptions>(
                        form,
                        "Quantities",
                        value => JsonConvert.DeserializeObject<ProductOptions>(value)
            );

            request.Categories_Ids = ExtractFormData<int>(
                        form,
                        "Category_Id",
                        value => int.TryParse(value, out int categoryId) ? categoryId : throw new ArgumentException("Invalid category ID")
                    );

            request.ProductImages = ExtractFiles(form, "Images");

            var result = await _productService.CreateProductAsync(request);

            return HandleServiceResult(result);
        }

        [HttpPost("api/product/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductRequest request)
        {
            var form = await Request.ReadFormAsync();
            request.Quantities = ExtractFormData<ProductOptions>
                (
                form,
                "Quantities",
                value => JsonConvert.DeserializeObject<ProductOptions>(value)
                );

            request.Categories_Ids = ExtractFormData<int>
              (
              form,
              "Category_Id",
                value => int.TryParse(value, out int result) ? result : throw new ArgumentException("Invalid integer value for Category.")
              );

            request.Product_New_Images = ExtractFiles(form, "Images");
            request.Product_Images_Paths = form.Keys
                   .Where(key => key.StartsWith("Images") && form[key].Count == 1 && !string.IsNullOrEmpty(form[key][0]))
                   .Select(key => form[key][0])
                   .OfType<string>() 
                   .Select(path => path.Split('/').Last())
                   .ToList();

            var result = await _productService.UpdateProductAsync(id, request);
            return HandleServiceResult(result);
        }

        [HttpGet("api/product/search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string query, [FromQuery] int? categoryid)
        {
            var result = await _productService.SearchProductsAsync(query, categoryid);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Deactivates a product by ID (soft delete)
        /// </summary>
        /// <param name="id">Product ID to deactivate</param>
        /// <returns>Operation result</returns>
        [HttpPost("api/product/delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Retrieves products by category slug with pagination
        /// </summary>
        /// <param name="slug">Category URL slug</param>
        /// <param name="page">Page number for pagination</param>
        /// <returns>Paged list of products in category</returns>
        [HttpGet("api/products/category/{slug}")]
        public async Task<IActionResult> GetProductsByCategorySlug(string slug, [FromQuery] int page = 1)
        {
            var result = await _productService.GetProductsByCategorySlugAsync(slug, page);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Retrieves all products with filtering and pagination
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="categoryid">Optional category filter</param>
        /// <param name="query">Optional search query</param>
        /// <returns>Paged list of products</returns>
        [HttpGet("api/products")]
        public async Task<IActionResult> GetAllProducts(
           [FromQuery] int page = 1,
           [FromQuery] int? categoryid = null,
           [FromQuery] string? query = null)
        {
            var result = await _productService.GetAllProductsAsync(page, categoryid, query);
            return HandleServiceResult(result);
        }

        [HttpGet("api/products/mostselling")]
        public async Task<IActionResult> GetMostSellingProducts()
        {
            var result = await _productService.GetMostSellingProductsAsync();
            return HandleServiceResult(result);
        }

        private IActionResult HandleServiceResult<T>(
            (bool isSuccess, ErrorResponse? errorResponse, T? response) result)
        {
            if (!result.isSuccess || result.errorResponse != null)
                return BadRequest(result.errorResponse);

            return Ok(result.response);
        }
        private static List<T> ExtractFormData<T>(
            IFormCollection form,
            string keyPrefix,
            Func<string, T> converter)
        {
            var result = new List<T>();

            foreach (var key in form.Keys)
            {
                if (key.StartsWith(keyPrefix))
                {
                    var value = form[key];
                    var convertedValue = converter(value);
                    result.Add(convertedValue);
                }
            }

            return result;
        }

        private static List<IFormFile> ExtractFiles(IFormCollection form, string keyPrefix)
        {
            var files = new List<IFormFile>();

            // Iterate through all files in the form
            foreach (var file in form.Files)
            {
                // Check if the file name starts with the key prefix and matches the exact pattern (e.g., "Images0", "Images1", etc.)
                if (file.Name.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase) &&
                    file.Name.Length > keyPrefix.Length && // Ensure there's something after the prefix
                    int.TryParse(file.Name.Substring(keyPrefix.Length), out _)) // Ensure the suffix is a number
                {
                    files.Add(file);
                }
            }

            return files;
        }
    }
}