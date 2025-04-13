﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shamia.DataAccessLayer;

#nullable disable

namespace Shamia.DataAccessLayer.Migrations
{
    [DbContext(typeof(ShamiaDbContext))]
    [Migration("20250128110335_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.CategoriesProducts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ProductId");

                    b.ToTable("CategoriesProducts");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name_Ar")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name_En")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Slug_Ar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Slug_En")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Name_Ar")
                        .IsUnique();

                    b.HasIndex("Name_En")
                        .IsUnique();

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.ContactInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Instagram")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location_Ar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location_En")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone_Number")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TikTok")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WhatsApp")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ContactInfos");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Instagram = "instrgam.com",
                            Location_Ar = "الكويت",
                            Location_En = "Kuwait",
                            Phone_Number = "+96525476832",
                            TikTok = "tiktok.com",
                            WhatsApp = "whatsApp"
                        });
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.Discount", b =>
                {
                    b.Property<int>("DiscountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DiscountId"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Percent")
                        .HasColumnType("int");

                    b.Property<DateTime>("StarDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("DiscountId");

                    b.ToTable("Discounts");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.Order", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("DiscountId")
                        .HasColumnType("int");

                    b.Property<int?>("DiscountId1")
                        .HasColumnType("int");

                    b.Property<int>("InvoiceId")
                        .HasColumnType("int");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("OrderReceiverDetailsId")
                        .HasColumnType("int");

                    b.Property<int?>("PaymentId")
                        .HasColumnType("int");

                    b.Property<string>("Payment_Status_Ar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Payment_Status_En")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status_Ar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status_En")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Total_Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("Total_Amount_With_Discount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId1")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("DiscountId");

                    b.HasIndex("DiscountId1");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId1");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.OrderDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal?>("Offer")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("OrderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductOptionsId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductOptionsId1")
                        .HasColumnType("int");

                    b.Property<int>("Quantity_Of_Unit")
                        .HasColumnType("int");

                    b.Property<decimal>("Unit_Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.HasIndex("ProductOptionsId");

                    b.HasIndex("ProductOptionsId1");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.OrderReceiverDetails", b =>
                {
                    b.Property<int>("OrderReceiverDetailsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderReceiverDetailsId"));

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Recevier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("OrderReceiverDetailsId");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("OrderReceiverDetails");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("OrderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PaymentMethod_Ar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentMethod_En")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Payment_Amount")
                        .HasColumnType("money");

                    b.Property<DateTime>("Payment_Date")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description_Ar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description_En")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsEnded")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Name_Ar")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name_En")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Slug_Ar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Slug_En")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<string>("Unit_Of_Measurement_Ar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Unit_Of_Measurement_En")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Name_Ar")
                        .IsUnique();

                    b.HasIndex("Name_En")
                        .IsUnique();

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.ProductImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsCover")
                        .HasColumnType("bit");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductsImages");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.ProductOptions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Default")
                        .HasColumnType("bit");

                    b.Property<decimal?>("Offer")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity_In_Unit")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductsOptions");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ExpiresOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("JwtId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RevokedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created_At")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Role")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("customer");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasFilter("[UserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Shamia.DataAccessLayer.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Shamia.DataAccessLayer.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shamia.DataAccessLayer.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Shamia.DataAccessLayer.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.CategoriesProducts", b =>
                {
                    b.HasOne("Shamia.DataAccessLayer.Entities.Category", "Category")
                        .WithMany("CategoriesProducts")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shamia.DataAccessLayer.Entities.Product", "Product")
                        .WithMany("CategoriesProducts")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.Order", b =>
                {
                    b.HasOne("Shamia.DataAccessLayer.Entities.Discount", null)
                        .WithMany("Orders")
                        .HasForeignKey("DiscountId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Shamia.DataAccessLayer.Entities.Discount", "Discount")
                        .WithMany()
                        .HasForeignKey("DiscountId1");

                    b.HasOne("Shamia.DataAccessLayer.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shamia.DataAccessLayer.Entities.User", null)
                        .WithMany("Orders")
                        .HasForeignKey("UserId1");

                    b.Navigation("Discount");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.OrderDetails", b =>
                {
                    b.HasOne("Shamia.DataAccessLayer.Entities.Order", "Order")
                        .WithMany("OrdersDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Shamia.DataAccessLayer.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shamia.DataAccessLayer.Entities.ProductOptions", "ProductOptions")
                        .WithMany()
                        .HasForeignKey("ProductOptionsId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Shamia.DataAccessLayer.Entities.ProductOptions", null)
                        .WithMany("OrderDetailsList")
                        .HasForeignKey("ProductOptionsId1");

                    b.Navigation("Order");

                    b.Navigation("Product");

                    b.Navigation("ProductOptions");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.OrderReceiverDetails", b =>
                {
                    b.HasOne("Shamia.DataAccessLayer.Entities.Order", "Order")
                        .WithOne("ReceiverDetails")
                        .HasForeignKey("Shamia.DataAccessLayer.Entities.OrderReceiverDetails", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.Payment", b =>
                {
                    b.HasOne("Shamia.DataAccessLayer.Entities.Order", "Order")
                        .WithOne("Payment")
                        .HasForeignKey("Shamia.DataAccessLayer.Entities.Payment", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.ProductImage", b =>
                {
                    b.HasOne("Shamia.DataAccessLayer.Entities.Product", "Product")
                        .WithMany("ProductsImages")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.ProductOptions", b =>
                {
                    b.HasOne("Shamia.DataAccessLayer.Entities.Product", "Product")
                        .WithMany("Quantity")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.Category", b =>
                {
                    b.Navigation("CategoriesProducts");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.Discount", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.Order", b =>
                {
                    b.Navigation("OrdersDetails");

                    b.Navigation("Payment");

                    b.Navigation("ReceiverDetails");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.Product", b =>
                {
                    b.Navigation("CategoriesProducts");

                    b.Navigation("ProductsImages");

                    b.Navigation("Quantity");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.ProductOptions", b =>
                {
                    b.Navigation("OrderDetailsList");
                });

            modelBuilder.Entity("Shamia.DataAccessLayer.Entities.User", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
