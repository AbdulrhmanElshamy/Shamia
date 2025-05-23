﻿using Microsoft.EntityFrameworkCore;
using Shamia.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Shamia.DataAccessLayer
{
    public class ShamiaDbContext : IdentityDbContext<User>
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ProductImage> ProductsImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ProductOptions> ProductsOptions { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<OrderReceiverDetails> OrderReceiverDetails { get; set; }
        public DbSet<Notification> Notifications { get; set; }



        public DbSet<CategoriesProducts> CategoriesProducts { get; set; }

        


        public ShamiaDbContext(DbContextOptions<ShamiaDbContext> options)
           : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShamiaDbContext).Assembly);

        }
    }
}
