using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEV1.Data;

namespace DEV1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // تم تعديل الدالة لتقبل فلاتر الأصناف الاختيارية
        public async Task<IActionResult> Index(int? categoryId)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
                // اختياري: حفظ اسم الصنف الحالي لعرضه في الواجهة
                var category = await _context.Categories.FindAsync(categoryId.Value);
                ViewBag.CategoryName = category?.Name;
            }

            var products = await query.ToListAsync();
            return View(products);
        }
    }
}