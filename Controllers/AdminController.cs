using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DEV1.Data;
using DEV1.Models;

namespace DEV1.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private const string AdminKey = "12345678"; 

        public AdminController(AppDbContext context) => _context = context;

        private bool IsAdmin() => HttpContext.Session.GetString("AdminVerified") == "true";

        public IActionResult Login() => IsAdmin() ? RedirectToAction(nameof(Dashboard)) : View();

        [HttpPost]
        public IActionResult Login(string secretKey)
        {
            if (secretKey == AdminKey)
            {
                HttpContext.Session.SetString("AdminVerified", "true");
                return RedirectToAction(nameof(Dashboard));
            }
            ModelState.AddModelError("", "الرمز السري غير صحيح!");
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin()) return RedirectToAction(nameof(Login));

            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            ViewBag.TotalProducts = products.Count;
            ViewBag.LowStockCount = products.Count(p => p.Stock <= 2 && p.Stock > 0);
            ViewBag.OutOffStockCount = products.Count(p => p.Stock == 0);
            ViewBag.TotalOrders = await _context.Orders.CountAsync();

            return View(products);
        }

        // شاشة الطلبات مع ميزة البحث الذكي
        public async Task<IActionResult> OrdersList(string searchTerm)
        {
            if (!IsAdmin()) return RedirectToAction(nameof(Login));

            var query = _context.Orders.Include(o => o.Product).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // البحث باستخدام كود الطلب أو اسم الزبون
                query = query.Where(o => o.OrderCode.Contains(searchTerm) || o.CustomerName.Contains(searchTerm));
                ViewBag.SearchTerm = searchTerm;
            }

            var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string newStatus)
        {
            if (!IsAdmin()) return Json(new { success = false });

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                _context.Update(order);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        // إجراء حذف المنتج
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!IsAdmin()) return Json(new { success = false, message = "غير مصرح" });

            var product = await _context.Products.FindAsync(id);
            if (product == null) return Json(new { success = false, message = "المنتج غير موجود" });

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, message = "لا يمكن حذف المنتج لأنه مرتبط بطلبات سابقة للزبائن!" });
            }
        }

        public async Task<IActionResult> CreateProduct()
        {
            if (!IsAdmin()) return RedirectToAction(nameof(Login));
            ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (!IsAdmin()) return RedirectToAction(nameof(Login));

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(int productId, int newStock, decimal newPrice)
        {
            if (!IsAdmin()) return Json(new { success = false });

            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.Stock = newStock;
                product.Price = newPrice;
                _context.Update(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminVerified");
            return RedirectToAction("Index", "Products");
        }
    }
}