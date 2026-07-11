using Microsoft.EntityFrameworkCore;
using DEV1.Models;

namespace DEV1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; } // أضفنا جدول الطلبات هنا

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. بذر الفئات (Seed Categories)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "خواتم" },
                new Category { Id = 2, Name = "سلاسل" },
                new Category { Id = 3, Name = "أساور" }
            );
            // داخل دالة OnModelCreating في ملف AppDbContext.cs استبدل جزء المنتجات بهذا:
modelBuilder.Entity<Product>().HasData(
    new Product 
    { 
        Id = 1, 
        Name = "خاتم فضة عيار 925", 
        Description = "خاتم فضة استرليني بتصميم أنيق يناسب جميع المناسبات", 
        Price = 150.00m, 
        ImageUrl = "/images/ring1.jpg", 
        CategoryId = 1,
        Stock = 5 // متوفر منه 5 قطع
    },
    new Product 
    { 
        Id = 2, 
        Name = "سلسلة ذهبية ناعمة", 
        Description = "سلسلة مطلية بالذهب عيار 18 بتصميم ناعم وجذاب", 
        Price = 250.00m, 
        ImageUrl = "/images/necklace1.jpg", 
        CategoryId = 2,
        Stock = 0 // نفدت الكمية (للإختبار)
    },
    new Product 
    { 
        Id = 3, 
        Name = "إسوارة من اللؤلؤ الطبيعي", 
        Description = "إسوارة مصنوعة يدوياً من اللؤلؤ والمطاط المرن", 
        Price = 180.00m, 
        ImageUrl = "/images/bracelet1.jpg", 
        CategoryId = 3,
        Stock = 2 // متوفر قطعتين فقط
    }
);
        }
    }
}