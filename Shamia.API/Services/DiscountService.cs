using Microsoft.EntityFrameworkCore;
using Shamia.API.Dtos;
using Shamia.API.Dtos.Request;
using Shamia.API.Dtos.Response;
using Shamia.API.Extension;
using Shamia.DataAccessLayer;
using Shamia.DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shamia.API.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly ShamiaDbContext _context;

        public DiscountService(ShamiaDbContext context)
        {
            _context = context;
        }

        public async Task<(bool isSuccess, ErrorResponse? errorMessage, (bool Active, decimal Percent)? result)> 
            IsValidDiscountAsync(string code)
        {
            var discount = await _context.Discounts
                .FirstOrDefaultAsync(c => c.Code == code);

            if (discount is null || discount.StarDate > DateTime.Now || discount.EndDate < DateTime.Now || !discount.Status)
                return (false, new ErrorResponse().CreateErrorResponse("Invalid discount code.", "كود الخصم غير صالح."), null);

            return (true, null, (discount.Status, discount.Percent));
        }

        public async Task<(bool isSuccess, ErrorResponse? errorMessage, PagedList<Discount>? result)> GetAllDiscountsAsync(int page)
        {
            const int pageSize = 20;

            var totalDiscounts = await _context.Discounts.CountAsync();

            var discounts = await _context.Discounts
                .OrderBy(d => d.DiscountId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedDiscounts = PagedList<Discount>.Create(discounts, page, pageSize, totalDiscounts);

            return (true, null, pagedDiscounts);
        }

        public async Task<(bool isSuccess, ErrorResponse? errorMessage, Discount? result)> GetDiscountByIdAsync(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
                return (false, new ErrorResponse().CreateErrorResponse("Discount not found.", "الخصم غير موجود."), null);

            return (true, null, discount);
        }

        public async Task<(bool isSuccess, ErrorResponse? errorMessage, int? discountId)> CreateDiscountAsync(CreateDiscountRequest request)
        {
            var discount = new Discount
            {
                Code = request.Code,
                StarDate = request.Start_Date,
                EndDate = request.End_Date,
                Percent = request.Percent,
                Status = true
            };

            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();

            return (true, null, discount.DiscountId);
        }

        public async Task<(bool isSuccess, ErrorResponse? errorMessage, string? result)> UpdateDiscountAsync(int id, Discount updatedDiscount)
        {
            if (id != updatedDiscount.DiscountId)
                return (false, new ErrorResponse().CreateErrorResponse("ID mismatch.", "عدم تطابق المعرف."), null);

            var existingDiscount = await _context.Discounts.FindAsync(id);
            if (existingDiscount == null)
                return (false, new ErrorResponse().CreateErrorResponse("Discount not found.", "الخصم غير موجود."), null);

            existingDiscount.Code = updatedDiscount.Code;
            existingDiscount.StarDate = updatedDiscount.StarDate;
            existingDiscount.EndDate = updatedDiscount.EndDate;
            existingDiscount.Percent = updatedDiscount.Percent;
            existingDiscount.Status = updatedDiscount.Status;

            _context.Entry(existingDiscount).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return (true, null, "Discount updated successfully.");
        }

        public async Task<(bool isSuccess, ErrorResponse? errorMessage, string? result)> DeleteDiscountAsync(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
                return (false, new ErrorResponse().CreateErrorResponse("Discount not found.", "الخصم غير موجود."), null);

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();

            return (true, null, "Discount deleted successfully.");
        }
    }
}