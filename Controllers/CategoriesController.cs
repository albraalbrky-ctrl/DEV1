using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEV1.Data;
using DEV1.Models;

namespace DEV1.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. عرض كل الفئات
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // 2. صفحة إنشاء فئة جديدة (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. حفظ الفئة الجديدة في قاعدة البيانات (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
    }
}